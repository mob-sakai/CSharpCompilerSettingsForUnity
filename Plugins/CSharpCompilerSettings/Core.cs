using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Coffee.CSharpCompilerSettings
{
    internal interface ICustomCompiler : IDisposable
    {
        bool IsValid();
        void Register();
    }

    internal static class Core
    {
        private static bool IsGlobal { get; set; }

        private static bool IsDevelopAssembly
        {
            get { return typeof(Core).Assembly.GetName().Name == "CSharpCompilerSettings_"; }
        }

        private static readonly ICustomCompiler[] customCompilers = new ICustomCompiler[]
        {
            CustomCompiler_Legacy.instance,
        };

        private static ICustomCompiler currentCustomCompiler { get; set; }

        public static bool ShouldToRecompile(string assemblyName, string asmdef)
        {
            if (assemblyName == typeof(Core).Assembly.GetName().Name)
            {
                // Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> requires default csc.</color>", assemblyName);
                return false;
            }
            else if (IsGlobal && GetPortableDllPath(asmdef) != null)
            {
                // Logger.LogWarning("  <color=#bbbb44><Skipped> Local CSharpCompilerSettings.*.dll for <b>'{0}'</b> is found.</color>", assemblyName);
                return false;
            }
            else if (!IsGlobal && !IsInSameDirectory(asmdef))
            {
                // Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> is not target.</color>", assemblyName);
                return false;
            }
            else if (!GetSettings().ShouldToRecompile(asmdef))
            {
                // Logger.LogWarning("  <color=#bbbb44><Skipped> Assembly <b>'{0}'</b> does not need to be recompiled.</color>", assemblyName);
                return false;
            }
            return true;
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
            text = Regex.Replace(text, "[\r\n]+", "\n", RegexOptions.Multiline);
            text = Regex.Replace(text, "^-", "/", RegexOptions.Multiline);
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


        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            IsGlobal = typeof(Core).Assembly.GetName().Name == "CSharpCompilerSettings" || IsDevelopAssembly;

            // Setup logger.
            if (IsGlobal)
            {
                Logger.Setup(
                    IsDevelopAssembly
                        ? "<b><color=#bb4444>[CscSettings(dev)]</color></b> "
                        : "<b><color=#bb4444>[CscSettings]</color></b> ",
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

            // This is global assembly, but the dev assembly is found: do nothing.
            if (IsGlobal && !IsDevelopAssembly && (Type.GetType("UnityEditor.EditorAssemblies, UnityEditor").Get("loadedAssemblies") as Assembly[]).Any(asm => asm.GetName().Name == "CSharpCompilerSettings_"))
            {
                Logger.LogWarning("This is global assembly, but the dev assembly is found: ignored.");
                return;
            }

            // Register callback.
            currentCustomCompiler?.Dispose();
            currentCustomCompiler = customCompilers.FirstOrDefault(c => c.IsValid());
            currentCustomCompiler?.Register();
            Logger.LogDebug("<color=#22aa22><b>InitializeOnLoad:</b></color> A custom compiler registered: {0}", currentCustomCompiler);

            // Install custom compiler package before compilation.
            var settings = GetSettings();
            if (!settings.UseDefaultCompiler)
                CompilerInfo.GetInstalledInfo(settings.CompilerPackage.PackageId);

            // Install analyzer packages before compilation.
            if (IsGlobal)
                foreach (var package in settings.AnalyzerPackages.Where(x => x.IsValid))
                    AnalyzerInfo.GetInstalledInfo(package.PackageId);
        }
    }
}
