using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

namespace Coffee.CSharpCompilerSettings
{
    internal class CustomCompiler_Legacy : ScriptableSingleton<CustomCompiler_Legacy>, ICustomCompiler
    {
        [SerializeField] private bool isInitialized;

        public bool IsValid()
        {
            var unityVersions = Application.unityVersion.Split('.');
            return int.Parse(unityVersions[0]) <= 2020;
        }

        public void Dispose()
        {
            typeof(CompilationPipeline).RemoveEvent<string>("assemblyCompilationStarted", OnAssemblyCompilationStarted);
        }

        public void Register()
        {
            typeof(CompilationPipeline).AddEvent<string>("assemblyCompilationStarted", OnAssemblyCompilationStarted);

            // Request recompilation at once.
            if (!isInitialized)
            {
                isInitialized = true;
                if (Regex.IsMatch(Application.unityVersion, "2019.(1|2)"))
                {
                    Logger.LogInfo("This is first compilation. Request script compilation again.");
                    Utils.RequestCompilation();
                }
            }
        }

        private static void ChangeCompilerProcess(object compiler, object scriptAssembly, CscSettingsAsset setting)
        {
            var tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            var tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            var fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);
            var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
            var oldCommand = (psi.FileName + " " + psi.Arguments).Replace('\\', '/');
            var command = oldCommand.Replace(EditorApplication.applicationContentsPath.Replace('\\', '/'), "@APP_CONTENTS@");
            var isDefaultCsc = Regex.IsMatch(command, "@APP_CONTENTS@/[^ ]*(mcs|csc)");
            var assemblyName = Path.GetFileNameWithoutExtension(scriptAssembly.Get("Filename") as string);
            var asmdefDir = scriptAssembly.Get("OriginPath") as string;
            var asmdefPath = string.IsNullOrEmpty(asmdefDir) ? "" : Core.FindAsmdef(asmdefDir);

            // csc is not Unity default. It is already modified.
            if (!isDefaultCsc)
            {
                Logger.LogWarning("  <color=#bbbb44><Skipped> current csc is not Unity default. It is already modified.</color>");
                return;
            }

            // Kill current process.
            compiler.Call("Dispose");

            // Response file.
            var responseFile = Regex.Replace(psi.Arguments, "^.*@(.+)$", "$1");

            // Change to custom compiler.
            if (setting.ShouldToUseCustomCompiler(asmdefPath))
            {
                var compilerInfo = CompilerInfo.GetInstalledInfo(setting.CompilerPackage.PackageId);

                // csc is not installed. Restart current process.
                if (!compilerInfo.IsValid)
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> C# compiler '{0}' is not installed. Restart compiler process: {1}</color>", compilerInfo.Path, oldCommand);

                    var currentProgram = tProgram.New(psi);
                    currentProgram.Call("Start");
                    compiler.Set("process", currentProgram, fiProcess);
                    return;
                }

                // Change exe file path.
                compilerInfo.Setup(psi, responseFile, Application.platform);
            }

            // Modify response file.
            var text = File.ReadAllText(responseFile);
            text = Core.ModifyResponseFile(setting, assemblyName, asmdefPath, text);
            File.WriteAllText(responseFile, text);

            // Logging
            if (CscSettingsAsset.instance.EnableDebugLog)
                Logger.LogDebug("Response file '{0}' has been modified:\n{1}", responseFile, Regex.Replace(text, "\n/reference.*", "") + "\n\n* The references are skipped because it was too long.");

            // Restart compiler process.
            Logger.LogDebug("Restart compiler process: {0} {1}\n  old command = {2}", psi.FileName, psi.Arguments, oldCommand);
            var program = tProgram.New(psi);
            program.Call("Start");
            compiler.Set("process", program, fiProcess);
        }

        private void OnAssemblyCompilationStarted(string name)
        {
            try
            {
                var assemblyName = Path.GetFileNameWithoutExtension(name);
                var tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
                var compilerTasks = tEditorCompilationInterface.Get("Instance").Get("compilationTask").Get("compilerTasks") as IDictionary;
                var scriptAssembly = compilerTasks.Keys.Cast<object>().FirstOrDefault(x => (x.Get("Filename") as string) == assemblyName + ".dll");
                if (scriptAssembly == null)
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> scriptAssembly <b>'{0}'</b> is not found.</color>", assemblyName);
                    return;
                }

                var asmdefDir = scriptAssembly.Get("OriginPath") as string;
                var asmdefPath = string.IsNullOrEmpty(asmdefDir) ? "" : Core.FindAsmdef(asmdefDir);
                if (!Core.ShouldToRecompile(assemblyName, asmdefPath)) return;

                var settings = Core.GetSettings();

                // Create new compiler to recompile.
                Logger.LogDebug("<color=#22aa22>Assembly compilation started: <b>{0} should be recompiled.</b></color>\nsettings = {1}", assemblyName, JsonUtility.ToJson(settings));
                ChangeCompilerProcess(compilerTasks[scriptAssembly], scriptAssembly, settings);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }
    }
}
