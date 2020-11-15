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
            defines = Utils.ModifySymbols(defines, setting.AdditionalSymbols);
            var defineText = string.Join(";", defines);
            content = Regex.Replace(content, "<DefineConstants>(.*)</DefineConstants>", string.Format("<DefineConstants>{0}</DefineConstants>", defineText), RegexOptions.Multiline);

            if (!setting.UseDefaultCompiler)
            {
                // Language version.
                content = Regex.Replace(content, "<LangVersion>.*</LangVersion>", "<LangVersion>" + setting.LanguageVersion + "</LangVersion>", RegexOptions.Multiline);
            }

            // Nullable.
            var value = setting.Nullable.ToString().ToLower();
            if (Regex.IsMatch(content, "<Nullable>.*</Nullable>"))
            {
                content = Regex.Replace(content, "<Nullable>.*</Nullable>", "<Nullable>" + value + "</Nullable>");
            }
            else
            {
                content = Regex.Replace(content, "(\\s+)(<LangVersion>.*</LangVersion>)([\r\n]+)", "$1$2$3$1<Nullable>" + value + "</Nullable>$3");
            }

            // Additional contents.
            // content = Regex.Replace(content, "^</Project>", "<!-- C# Settings For Unity -->", RegexOptions.Singleline);
            content = Regex.Replace(content, "[\r\n]+</Project>[\r\n]*", "\r\n<!-- C# Settings For Unity -->");
            {
                content += NewLine + "  <ItemGroup>";
                {
                    // Add compiler package.
                    if (!setting.UseDefaultCompiler)
                        content = AddPackage(content, setting.CompilerPackage.Name, setting.CompilerPackage.Version);

                    // Add analyzer packages.
                    foreach (var package in setting.AnalyzerPackages)
                        content = AddPackage(content, package.Name, package.Version);
                }
                content += NewLine + "  </ItemGroup>";

                // Add rule set files.
                content += NewLine + "  <PropertyGroup>";
                {
                    // Ruleset.
                    var rulesets = new[] {"Assets/Default.ruleset"} // Add default rule set for project.
                        .Concat(string.IsNullOrEmpty(asmdefPath)
                            ? new[] {"Assets/" + assemblyName + ".ruleset"} // Add rule set for predefined assemblies (e.g. Assembly-CSharp.dll).
                            : Directory.GetFiles(Path.GetDirectoryName(asmdefPath), "*.ruleset")) // Add rule sets for asmdef.
                        .Where(File.Exists);

                    foreach (var ruleset in rulesets)
                        content = AddRuleSet(content, ruleset);
                }
                content += NewLine + "  </PropertyGroup>";
            }
            content += NewLine + "<!-- C# Settings For Unity -->" + NewLine + NewLine + "</Project>" + NewLine;

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

        private static string AddRuleSet(string content, string ruleset)
        {
            if (File.Exists(ruleset))
                content += NewLine + "    <CodeAnalysisRuleSet>" + ruleset.Replace('/', '\\') + "</CodeAnalysisRuleSet>";
            return content;
        }

        private static string NewLine = "\r\n";
    }
}
