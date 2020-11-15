using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    [CustomPropertyDrawer(typeof(NugetPackage))]
    internal class NugetPackageDrawer : PropertyDrawer
    {
        public static readonly float Height = 2 * (EditorGUIUtility.singleLineHeight + 2);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var p = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indentLevel = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 50;
            {
                var spName = property.FindPropertyRelative("m_Name");
                var spVersion = property.FindPropertyRelative("m_Version");

                DrawPackageNameField(new Rect(p.x, p.y, p.width, 16), spName, spVersion);
                DrawPackageVersionField(new Rect(p.x, p.y + 18, p.width, 16), spName, spVersion);
            }
            EditorGUI.indentLevel = indentLevel;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }

        private void DrawPackageNameField(Rect rect, SerializedProperty spName, SerializedProperty spVersion)
        {
            // Text field.
            var r = new Rect(rect.x, rect.y, rect.width - 18, 16);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(r, spName);
            if (EditorGUI.EndChangeCheck())
                spVersion.stringValue = "";

            // Draw button.
            r = new Rect(rect.x + rect.width - 14, rect.y + 4, 14, 12);
            if (!GUI.Button(r, EditorGUIUtility.IconContent("icon dropdown"), EditorStyles.label)) return;

            GUI.FocusControl("");

            // Create menu.
            var menu = new GenericMenu();
            foreach (var pkgName in NugetPackageCatalog.GetPackageNames())
                menu.AddItem(new GUIContent(pkgName), spName.stringValue == pkgName, () =>
                {
                    spName.stringValue = pkgName;
                    spVersion.stringValue = "";
                });

            menu.ShowAsContext();
        }

        private static void DrawPackageVersionField(Rect rect, SerializedProperty spName, SerializedProperty spVersion)
        {
            // Draw button.
            var selectedVersion = string.IsNullOrEmpty(spVersion.stringValue) ? "-" : spVersion.stringValue;
            var buttonRect = EditorGUI.PrefixLabel(rect, new GUIContent(spVersion.displayName, spVersion.tooltip));
            if (!GUI.Button(buttonRect, selectedVersion, EditorStyles.popup)) return;

            // Refresh package versions.
            var pkgName = spName.stringValue;
            NugetPackageCatalog.RefreshAvailableVersions(pkgName);

            // Create menu.
            var menu = new GenericMenu();
            foreach (var pkgVersion in NugetPackageCatalog.GetPackagePreReleaseVersions(pkgName))
                menu.AddItem(new GUIContent("Pre Releases/" + pkgVersion), selectedVersion == pkgVersion, () => spVersion.stringValue = pkgVersion);
            if (0 < menu.GetItemCount())
                menu.AddSeparator("");
            foreach (var pkgVersion in NugetPackageCatalog.GetPackageVersions(pkgName))
                menu.AddItem(new GUIContent(pkgVersion), selectedVersion == pkgVersion, () => spVersion.stringValue = pkgVersion);

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("No available versions"));
            menu.ShowAsContext();
        }
    }

    internal class NugetPackageCatalog : ScriptableSingleton<NugetPackageCatalog>
    {
#pragma warning disable 649
        [Serializable]
        private class CatalogRoot
        {
            public CatalogPage[] items;

            [Serializable]
            internal class CatalogPage
            {
                public Package[] items;

                [Serializable]
                internal class Package
                {
                    public PackageDetails catalogEntry;

                    [Serializable]
                    internal class PackageDetails
                    {
                        public string id;
                        public string version;
                    }
                }
            }
        }
#pragma warning restore 649


        public static NugetPackage.CategoryType CurrentCategory = NugetPackage.CategoryType.Compiler;

        public List<NugetPackage> m_Packages = new List<NugetPackage>()
        {
            new NugetPackage("Microsoft.Net.Compilers.Toolset", "3.5.0", NugetPackage.CategoryType.Compiler),
            new NugetPackage("Microsoft.Net.Compilers", "3.5.0", NugetPackage.CategoryType.Compiler),
            new NugetPackage("OpenSesame.Net.Compilers.Toolset", "3.5.0", NugetPackage.CategoryType.Compiler),
            new NugetPackage("OpenSesame.Net.Compilers", "3.5.0", NugetPackage.CategoryType.Compiler),
            new NugetPackage("Roslynator.Analyzers", "3.0.0", NugetPackage.CategoryType.Analyzer),
            new NugetPackage("Roslynator.CodeAnalysis.Analyzers", "1.0.0", NugetPackage.CategoryType.Analyzer),
            new NugetPackage("Roslynator.Formatting.Analyzers", "1.0.0", NugetPackage.CategoryType.Analyzer),
            new NugetPackage("ErrorProne.NET.CoreAnalyzers", "0.1.2", NugetPackage.CategoryType.Analyzer),
            new NugetPackage("StyleCop.Analyzers", "1.0.0", NugetPackage.CategoryType.Analyzer),
        };

        public static IEnumerable<string> GetPackageNames()
        {
            return instance.m_Packages
                .Where(x => x.Category == CurrentCategory)
                .Select(x => x.Name)
                .Distinct();
        }

        public static IEnumerable<string> GetPackageVersions(string packageName)
        {
            return instance.m_Packages
                .Where(x => x.Name == packageName && x.Category == CurrentCategory)
                .Select(x => x.Version)
                .Where(x => !SemVersion.Parse(x).IsPrerelease);
        }

        public static IEnumerable<string> GetPackagePreReleaseVersions(string packageName)
        {
            return instance.m_Packages
                .Where(x => x.Name == packageName && x.Category == CurrentCategory)
                .Select(x => x.Version)
                .Where(x => SemVersion.Parse(x).IsPrerelease);
        }

        public static void RefreshAvailableVersions(string packageName)
        {
            var filename = "Temp/DownloadedPackages/" + packageName + ".json/index";
            if (!File.Exists(filename))
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Package Installer", string.Format("Fetch package versions: {0}", packageName), 0.5f);
                    var file = Utils.DownloadFile("https://api.nuget.org/v3/registration5-gz-semver2/" + packageName.ToLower() + "/index.json");
                    Utils.ExtractArchive(file, Path.GetDirectoryName(filename));
                }
                catch
                {
                    var dir = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.WriteAllText(filename, "{}");
                    Logger.LogException("Package '{0}' is not found.", packageName);
                    return;
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            var json = File.ReadAllText(filename);
            var obj = JsonUtility.FromJson<CatalogRoot>(json);
            if (obj.items == null || obj.items.Length == 0) return;

            instance.m_Packages = instance.m_Packages
                .Concat(obj.items
                    .SelectMany(x => x.items)
                    .Select(x => new NugetPackage(x.catalogEntry.id, x.catalogEntry.version, CurrentCategory)))
                .Distinct()
                .OrderBy(x => x.Name)
                .ThenByDescending(x => SemVersion.Parse(x.Version))
                .ToList();
        }
    }
}
