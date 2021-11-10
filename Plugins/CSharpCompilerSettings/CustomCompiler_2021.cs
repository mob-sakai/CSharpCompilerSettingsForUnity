using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.Compilation;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEditor;

namespace Coffee.CSharpCompilerSettings
{
    internal class CustomCompiler_2021 : ScriptableSingleton<CustomCompiler_2021>, ICustomCompiler
    {
        private static readonly int logIdentifier = typeof(Core).FullName.GetHashCode();
        private static readonly object cscOutputParser = Type.GetType("UnityEditor.Scripting.Compilers.MicrosoftCSharpCompilerOutputParser, UnityEditor").New();
        private object[] assemblies;
        private string dagName;
        [SerializeField] private bool hasCompileError;
        [SerializeField] private bool isInitialized;

        public bool IsValid()
        {
            var unityVersions = Application.unityVersion.Split('.');
            return 2021 <= int.Parse(unityVersions[0]);
        }

        public void Dispose()
        {
            typeof(CompilationPipeline).RemoveEvent<object>("compilationStarted", OnCompilationStarted);
            typeof(CompilationPipeline).RemoveEvent<object>("compilationFinished", OnCompilationFinished);
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
        }

        public void Register()
        {
            typeof(CompilationPipeline).AddEvent<object>("compilationStarted", OnCompilationStarted);
            typeof(CompilationPipeline).AddEvent<object>("compilationFinished", OnCompilationFinished);
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;

            // Request recompilation at once.
            if (!isInitialized)
            {
                Logger.LogInfo("This is first compilation. Request script compilation again.");
                Utils.RequestCompilation();
            }
        }

        public void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (hasCompileError && state == PlayModeStateChange.ExitingEditMode)
            {
                EditorApplication.isPlaying = false;
                typeof(SceneView).Call("ShowCompileErrorNotification");
            }
        }

        public CompilerInfo GetDefaultCompiler()
        {
            var ext = (Application.platform == RuntimePlatform.WindowsEditor) ? ".exe" : "";
            var appContents = EditorApplication.applicationContentsPath;
            if (Directory.Exists(Utils.PathCombine(appContents, "DotNetSdkRoslyn")))
            {
                var dotnet = Utils.PathCombine(appContents, "NetCoreRuntime", "dotnet" + ext);
                var csc = Utils.PathCombine(appContents, "DotNetSdkRoslyn", "csc.dll");
                return new CompilerInfo("", csc, dotnet);
            }
            else if (Directory.Exists(Utils.PathCombine(appContents, "Tools", "Roslyn")))
            {
                var dotnet = Utils.PathCombine(appContents, "Tools", "netcorerun", "netcorerun" + ext);
                var csc = Utils.PathCombine(appContents, "Tools", "Roslyn", "csc.dll");
                return new CompilerInfo("", csc, dotnet);
            }

            throw new FileNotFoundException("Default CompilerInfo is not found.");
        }
        private void DebugRemoveLogEntriesByIdentifier()
        {
            hasCompileError = false;
            typeof(Debug).Call("RemoveLogEntriesByIdentifier", logIdentifier);
        }

        private void DebugLogCompilerMessage(string outputs, bool hasError)
        {
            var parsedMessages = cscOutputParser.Call("Parse", outputs.Split('\r', '\n'), hasError) as IList;
            foreach (var m in parsedMessages)
            {
                var isError = (int)m.Get("type") == 0;
                typeof(Debug).Call("LogCompilerMessage", m.Get("message"), m.Get("file"), m.Get("line"), m.Get("column"), true, isError, logIdentifier);
            }
        }

        private string GetOutDir(string dagName)
        {
            if (dagName.EndsWith("E")) return "Library/ScriptAssemblies";

            var unityVersions = Application.unityVersion.Split('.');
            if (int.Parse(unityVersions[0]) == 2021)
                return "Library/PlayerScriptAssemblies";
            else
                return "Library/Bee/PlayerScriptAssemblies";
        }

        private void Recompile(object assembly, string dagName)
        {
            var assemblyName = assembly.Get("name") as string;
            var asmdefPath = assembly.Get("asmdef") as string;

            // Check.
            if (!Core.ShouldToRecompile(assemblyName, asmdefPath)) return;

            // Check compiler.
            var cscSetting = Core.GetSettings();
            var compilerInfo = cscSetting.UseDefaultCompiler
                ? GetDefaultCompiler()
                : CompilerInfo.GetInstalledInfo(cscSetting.CompilerPackage.PackageId);
            if (!compilerInfo.IsValid)
            {
                Logger.LogWarning("  <color=#bbbb44><Skipped> C# compiler '{0}' is not installed. Use default compiler instead.</color>", compilerInfo.Path);
                compilerInfo = GetDefaultCompiler();
            }

            // Modify response file.
            var responseFile = $"Library/Bee/artifacts/{dagName}.dag/{assemblyName}.rsp";
            var modResponseFile = $"{responseFile}.mod";
            var text = File.ReadAllText(responseFile);
            text = Core.ModifyResponseFile(cscSetting, assemblyName, asmdefPath, text);
            text = text.Replace($"/out:\"Library/Bee/artifacts/{dagName}.dag/", $"/out:\"{GetOutDir(dagName)}/");
            File.WriteAllText(modResponseFile, text);

            // Setup recompile process.
            var psi = new ProcessStartInfo();
            compilerInfo.Setup(psi, modResponseFile, Application.platform);

            using (var program = Type.GetType("UnityEditor.Utils.Program, UnityEditor").New(psi) as IDisposable)
            {
                // Start recompile.
                Logger.LogDebug("<color=#22aa22><b>Recompile ({0})</b></color> Start command: {1} {2}\n\nResponse file:\n{3}", assemblyName, psi.FileName, psi.Arguments, text);
                program.Call("Start");
                program.Call("WaitForExit");

                // Check outputs.
                var outputs = program.Call("GetAllOutput") as string;
                var exitCode = (int)program.Get("ExitCode");
                hasCompileError |= exitCode != 0;

                // Log compiler messages.
                program.Call("LogProcessStartInfo");
                Logger.LogDebug("<color=#22aa22><b>Recompile ({0})</b></color> Finished with code {1}\n\nOutputs:\n{2}", assemblyName, exitCode, outputs);

                DebugLogCompilerMessage(outputs, exitCode != 0);
            }
        }

        private void OnCompilationStarted(object state)
        {
            DebugRemoveLogEntriesByIdentifier();

            // Store assembly infos.
            var driver = state.Get("Driver");
            dagName = driver.Get("m_DagName") as string;
            var mdata = driver.Get("DataForBuildProgram").Get("m_Data") as Dictionary<string, object>;
            var cdata = mdata.Values.FirstOrDefault(x => x.GetType().FullName.EndsWith("ScriptCompilationData"));
            assemblies = cdata.Get("assemblies") as object[];
        }

        private void OnAssemblyCompilationFinished(string dllPath, CompilerMessage[] messages)
        {
            if (messages.Any(x => x.type == CompilerMessageType.Error)) return;
            var assemblyName = Path.GetFileNameWithoutExtension(dllPath);

            try
            {
                var assembly = assemblies.First(x => (string)x.Get("name") == assemblyName);
                Recompile(assembly, dagName);
            }
            catch (Exception e)
            {
                hasCompileError = true;
                Debug.LogException(e);
            }
        }

        private void OnCompilationFinished(object state)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        Recompile(assembly, dagName);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            if (Application.isBatchMode && hasCompileError)
            {
                Console.WriteLine("::compilation error:: exit 1");
                Application.Quit(1);
            }
        }
    }
}
