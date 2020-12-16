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
using Assembly = System.Reflection.Assembly;

namespace Coffee.CSharpCompilerSettings
{
    [InitializeOnLoad]
    internal static class Core
    {
        private static bool IsGlobal { get; }

        public static void DirtyScriptsIfNeeded()
        {
            var assemblyName = GetAssemblyName(FindAsmdef());
            if (!IsGlobal && string.IsNullOrEmpty(assemblyName)) return;

            var filepath = "Temp/" + typeof(Core).Assembly.GetName().Name + ".loaded";
            if (File.Exists(filepath)) return;
            File.WriteAllText(filepath, "");

            Utils.RequestCompilation(IsGlobal ? null : assemblyName);
        }

        public static string GetAssemblyName(string asmdefPath)
        {
            if (string.IsNullOrEmpty(asmdefPath)) return null;

            var m = Regex.Match(File.ReadAllText(asmdefPath), "\"name\":\\s*\"([^\"]*)\"");
            return m.Success ? m.Groups[1].Value : "";
        }

        public static string GetPortableDllPath(string asmdefPath)
        {
            if (string.IsNullOrEmpty(asmdefPath)) return null;

            return Directory.GetFiles(Path.GetDirectoryName(asmdefPath))
                .FirstOrDefault(x => Regex.IsMatch(x, "CSharpCompilerSettings_[0-9a-zA-Z]{32}.dll"));
        }

        public static bool IsInSameDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            var dir = Path.GetFullPath(Path.GetDirectoryName(path)).ToLower();
            var coreAssemblyLocationDir = Path.GetFullPath(Path.GetDirectoryName(typeof(Core).Assembly.Location)).ToLower();
            return dir == coreAssemblyLocationDir;
        }

        public static string FindAsmdef(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                var assemblyPath = typeof(Core).Assembly.Location;
                assemblyPath = AssetDatabase.FindAssets("t:DefaultAsset " + Path.GetFileNameWithoutExtension(assemblyPath))
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .FirstOrDefault(x => x.EndsWith(".dll"));
                path = Path.GetDirectoryName(assemblyPath);
            }

            var asmdefPath = Directory.GetFiles(path, "*.asmdef")
                .FirstOrDefault(x => x.EndsWith(".asmdef")) ?? "";

            return asmdefPath.Replace('\\', '/').Replace(Environment.CurrentDirectory.Replace('\\', '/') + "/", "");
        }

        public static CscSettingsAsset GetSettings()
        {
            return IsGlobal
                ? CscSettingsAsset.instance
                : CscSettingsAsset.GetAtPath(FindAsmdef()) ?? ScriptableObject.CreateInstance<CscSettingsAsset>();
        }

        public static string ModifyResponseFile(CscSettingsAsset setting, string assemblyName, string asmdefPath, string text)
        {
            var asmdefDir = string.IsNullOrEmpty(asmdefPath) ? null : Path.GetDirectoryName(asmdefPath);
            text = Regex.Replace(text, "[\r\n]+", "\n");
            text = Regex.Replace(text, "^-", "/");
            text = Regex.Replace(text, "\n/debug\n", "\n/debug:portable\n");
            text += "\n/preferreduilang:en-US";

            // Custom compiler.
            if (setting.ShouldToUseCustomCompiler(asmdefPath))
            {
                // Change language version.
                text = Regex.Replace(text, "\n/langversion:[^\n]+\n", "\n/langversion:" + setting.LanguageVersion + "\n");

                // Nullable.
                text = Regex.Replace(text, "\n/nullable.*", "");
                if (setting.IsSupportNullable)
                {
                    text += "\n/nullable:" + setting.Nullable.ToString().ToLower();
                }
            }

            // Modify scripting define symbols.
            var symbolModifier = setting.GetSymbolModifier(asmdefPath);
            if (!string.IsNullOrEmpty(symbolModifier))
            {
                var defines = Regex.Matches(text, "^/define:(.*)$", RegexOptions.Multiline)
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value);
                text = Regex.Replace(text, "[\r\n]+/define:[^\r\n]+", "");
                var modifiedDefines = Utils.ModifySymbols(defines, symbolModifier);
                foreach (var d in modifiedDefines)
                    text += "\n/define:" + d;
            }

            // Analyzer.
            var globalSettings = CscSettingsAsset.instance;
            if (globalSettings.ShouldToUseAnalyzer(asmdefPath))
            {
                // Analyzer dlls.
                foreach (var package in globalSettings.AnalyzerPackages)
                {
                    var analyzerInfo = AnalyzerInfo.GetInstalledInfo(package.PackageId);
                    foreach (var dll in analyzerInfo.DllFiles)
                        text += string.Format("\n/analyzer:\"{0}\"", dll);
                }

                // Ruleset.
                var rulesets = new[] {"Assets/Default.ruleset"}
                    .Concat(string.IsNullOrEmpty(asmdefDir)
                        ? new[] {"Assets/" + assemblyName + ".ruleset"}
                        : Directory.GetFiles(asmdefDir, "*.ruleset"))
                    .Where(File.Exists);

                foreach (var ruleset in rulesets)
                    text += string.Format("\n/ruleset:\"{0}\"", ruleset);

                // Editor Config.
                // var configs = new[]
                //     {
                //         ".editorconfig",
                //         Utils.PathCombine(asmdefDir ?? "Assets", ".editorconfig")
                //     }
                //     .Where(File.Exists);
                // foreach (var config in configs)
                //     text += string.Format("\n/analyzerconfig:\"{0}\"", config);
            }

            // Replace NewLine and save.
            text = Regex.Replace(text, "\n", Environment.NewLine);

            return text;
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
            var asmdefPath = string.IsNullOrEmpty(asmdefDir) ? "" : FindAsmdef(asmdefDir);

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
                compilerInfo.Setup(psi, responseFile);
            }

            // Modify response file.
            var text = File.ReadAllText(responseFile);
            text = ModifyResponseFile(setting, assemblyName, asmdefPath, text);
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

        public static void OnAssemblyCompilationStarted(string name)
        {
            try
            {
                var assemblyName = Path.GetFileNameWithoutExtension(name);
                if (assemblyName == typeof(Core).Assembly.GetName().Name)
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> requires default csc.</color>", assemblyName);
                    return;
                }

                var tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
                var compilerTasks = tEditorCompilationInterface.Get("Instance").Get("compilationTask").Get("compilerTasks") as IDictionary;
                var scriptAssembly = compilerTasks.Keys.Cast<object>().FirstOrDefault(x => (x.Get("Filename") as string) == assemblyName + ".dll");
                if (scriptAssembly == null)
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> scriptAssembly <b>'{0}'</b> is not found.</color>", assemblyName);
                    return;
                }

                var asmdefDir = scriptAssembly.Get("OriginPath") as string;
                var asmdefPath = string.IsNullOrEmpty(asmdefDir) ? "" : FindAsmdef(asmdefDir);
                if (IsGlobal && GetPortableDllPath(asmdefPath) != null)
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> Local CSharpCompilerSettings.*.dll for <b>'{0}'</b> is found.</color>", assemblyName);
                    return;
                }

                if (!IsGlobal && !IsInSameDirectory(asmdefPath))
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> is not target.</color>", assemblyName);
                    return;
                }

                var globalSettings = CscSettingsAsset.instance;
                var settings = GetSettings();
                if (!globalSettings.ShouldToRecompile(asmdefPath))
                {
                    Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> does not need to be recompiled.</color>", assemblyName);
                    return;
                }

                // Create new compiler to recompile.
                Logger.LogDebug("<color=#22aa22>Assembly compilation started: <b>{0} should be recompiled.</b></color>\nsettings = {1}", assemblyName, JsonUtility.ToJson(settings));
                ChangeCompilerProcess(compilerTasks[scriptAssembly], scriptAssembly, settings);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        static Core()
        {
            var coreAssemblyName = typeof(Core).Assembly.GetName().Name;
            if (coreAssemblyName == "CSharpCompilerSettings_") return;

            IsGlobal = coreAssemblyName == "CSharpCompilerSettings";

            // Setup logger.
            if (IsGlobal)
            {
                Logger.Setup(
                    "<b><color=#bb4444>[CscSettings]</color></b> ",
                    () => CscSettingsAsset.instance.EnableDebugLog
                );
            }
            else
            {
                var targetAssemblyName = GetAssemblyName(FindAsmdef());
                Logger.Setup(
                    "<b><color=#bb4444>[CscSettings (<color=orange>" + targetAssemblyName + "</color>)]</color></b> ",
                    () => CscSettingsAsset.instance.EnableDebugLog
                );

                if (string.IsNullOrEmpty(targetAssemblyName))
                    Logger.LogException("Target assembly is not found. {0}", typeof(Core).Assembly.Location.Replace(Environment.CurrentDirectory, "."));
            }

            // Dump loaded assemblies
            if (CscSettingsAsset.instance.EnableDebugLog)
            {
                var sb = new System.Text.StringBuilder("<color=#22aa22><b>InitializeOnLoad,</b></color> the loaded assemblies:\n");
                foreach (var asm in Type.GetType("UnityEditor.EditorAssemblies, UnityEditor").Get("loadedAssemblies") as Assembly[])
                {
                    var name = asm.GetName().Name;
                    var path = asm.Location;
                    if (path.Contains(Path.GetDirectoryName(EditorApplication.applicationPath)))
                        sb.AppendFormat("  > {0}:\t{1}\n", name, "APP_PATH/.../" + Path.GetFileName(path));
                    else
                        sb.AppendFormat("  > <color=#22aa22><b>{0}</b></color>:\t{1}\n", name, path.Replace(Environment.CurrentDirectory, "."));
                }

                Logger.LogDebug(sb.ToString());
            }

            // Register callback.
            Logger.LogDebug("<color=#22aa22><b>InitializeOnLoad:</b></color> start watching assembly compilation.");
            CompilationPipeline.assemblyCompilationStarted -= OnAssemblyCompilationStarted;
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;

            // Install custom csc before compilation.
            var settings = GetSettings();
            if (!settings.UseDefaultCompiler)
                CompilerInfo.GetInstalledInfo(settings.CompilerPackage.PackageId);

            if (IsGlobal)
                foreach (var package in settings.AnalyzerPackages.Where(x => x.IsValid))
                    AnalyzerInfo.GetInstalledInfo(package.PackageId);

            // If Unity 2020.2 or newer, request re-compilation.
            var version = Application.unityVersion.Split('.');
            var major = int.Parse(version[0]);
            var minor = int.Parse(version[1]);
            if (2021 <= major || (major == 2020 && 2 <= minor))
                DirtyScriptsIfNeeded();
        }
    }
}
