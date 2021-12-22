using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Coffee.CSharpCompilerSettings
{
    internal class CSharpProjectModifier : AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            // Find asmdef.
            var assemblyName = Path.GetFileNameWithoutExtension(path);
            var asmdefPath = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset " + Path.GetFileNameWithoutExtension(path))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .FirstOrDefault(x => Path.GetFileName(x) == assemblyName + ".asmdef");

            Logger.LogDebug("<color=orange>OnGeneratedCSProject</color> {0} -> {1} ({2})", path, assemblyName, asmdefPath);
            var setting = CscSettingsAsset.GetAtPath(asmdefPath) ?? CscSettingsAsset.instance;

            // Modify define symbols.
            var defines = Regex.Match(content, "<DefineConstants>(.*)</DefineConstants>").Groups[1].Value.Split(';', ',');
            defines = Utils.ModifySymbols(defines, setting.GetSymbolModifier(asmdefPath));
            var defineText = string.Join(";", defines);
            content = Regex.Replace(content, "<DefineConstants>(.*)</DefineConstants>", string.Format("<DefineConstants>{0}</DefineConstants>", defineText), RegexOptions.Multiline);

            if (setting.ShouldToUseCustomCompiler(asmdefPath))
            {
                // Language version.
                content = Regex.Replace(content, "<LangVersion>.*</LangVersion>", "<LangVersion>" + setting.LanguageVersion + "</LangVersion>", RegexOptions.Multiline);

                // Nullable.
                if (!setting.IsSupportNullable)
                {
                    var value = setting.Nullable.ToString().ToLower();
                    if (Regex.IsMatch(content, "<Nullable>.*</Nullable>"))
                    {
                        content = Regex.Replace(content, "<Nullable>.*</Nullable>", "<Nullable>" + value + "</Nullable>");
                    }
                    else
                    {
                        content = Regex.Replace(content, "(\\s+)(<LangVersion>.*</LangVersion>)([\r\n]+)", "$1$2$3$1<Nullable>" + value + "</Nullable>$3");
                    }
                }
            }

            // Additional contents.
            content = Regex.Replace(content, NewLine + AdditionalContentComment + ".*" + AdditionalContentComment, "", RegexOptions.Singleline);
            content = Regex.Replace(content, "[\r\n]+</Project>[\r\n]*", NewLine + AdditionalContentComment);
            {
                content += NewLine + "  <ItemGroup>";
                {
                    // Add compiler package.
                    if (!setting.UseDefaultCompiler)
                        content = AddPackage(content, setting.CompilerPackage.Name, setting.CompilerPackage.Version);

                    // Add analyzer packages.
                    foreach (var package in setting.AnalyzerPackages)
                        content = AddAnalyzer(content, package.PackageId);
                }
                content += NewLine + "  </ItemGroup>";

                // Add rule set files.
                content += NewLine + "  <PropertyGroup>";
                {
                    // Ruleset.
                    var rulesets = new[] { "Assets/Default.ruleset" } // Add default rule set for project.
                        .Concat(string.IsNullOrEmpty(asmdefPath)
                            ? new[] { "Assets/" + assemblyName + ".ruleset" } // Add rule set for predefined assemblies (e.g. Assembly-CSharp.dll).
                            : Directory.GetFiles(Path.GetDirectoryName(asmdefPath), "*.ruleset")) // Add rule sets for asmdef.
                        .Where(File.Exists);

                    foreach (var ruleset in rulesets)
                        content = AddRuleSet(content, ruleset);
                }
                content += NewLine + "  </PropertyGroup>";
            }
            content += NewLine + AdditionalContentComment + NewLine + "</Project>" + NewLine;

            return content;
        }

        private static string AddPackage(string content, string name, string version)
        {
            content += NewLine + "    <PackageReference Include=\"" + name + "\" Version=\"" + version + "\">";
            content += NewLine + "      <PrivateAssets>all</PrivateAssets>";
            content += NewLine + "      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>";
            content += NewLine + "    </PackageReference>";
            return content;
        }

        private static string AddAnalyzer(string content, string packageId)
        {
            var info = AnalyzerInfo.GetInstalledInfo(packageId);
            foreach (var dll in info.DllFiles)
                content += NewLine + "    <Analyzer Include=\"" + dll.Replace('/', '\\') + "\" />";
            return content;
        }

        private static string AddRuleSet(string content, string ruleset)
        {
            if (File.Exists(ruleset))
                content += NewLine + "    <CodeAnalysisRuleSet>" + ruleset.Replace('/', '\\') + "</CodeAnalysisRuleSet>";
            return content;
        }

        private const string NewLine = "\r\n";
        private const string AdditionalContentComment = "<!-- C# Settings For Unity -->";

#if UNITY_2021_1_OR_NEWER
        private static FileSystemWatcher _watcher;

        private static void OnCSProjectCreated(string file)
        {
            if (string.IsNullOrEmpty(file) || Path.GetExtension(file) != ".csproj" || !File.Exists(file))
                return;

            // 
            var codeEditor = Unity.CodeEditor.CodeEditor.Editor.CurrentCodeEditor;
            if (codeEditor?.GetType().Name != "VSCodeScriptEditor")
                return;

            try
            {
                var text = File.ReadAllText(file, System.Text.Encoding.UTF8);
                var beforeHash = text.GetHashCode();
                text = OnGeneratedCSProject(file, text);
                var afterHash = text.GetHashCode();
                if (beforeHash != afterHash)
                {
                    Logger.LogDebug("<color=orange>Modify CSProject</color> {0}", Path.GetFileName(file));
                    File.WriteAllText(file, text);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        [InitializeOnLoadMethod]
        private static void WatchCSProject()
        {
#if !UNITY_EDITOR_WIN
            Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "enabled");
#endif
            var resultDir = Path.GetFullPath(".");
            foreach (var file in Directory.GetFiles(resultDir, "*.csproj"))
                EditorApplication.delayCall += () => OnCSProjectCreated(Path.Combine(resultDir, file));

            _watcher?.Dispose();
            _watcher = new FileSystemWatcher()
            {
                Path = resultDir,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
                Filter = "*.csproj",
            };

            _watcher.Created += (s, e) => EditorApplication.delayCall += () => OnCSProjectCreated(e.FullPath);
            _watcher.Changed += (s, e) => EditorApplication.delayCall += () => OnCSProjectCreated(e.FullPath);
        }
#endif
    }
}
