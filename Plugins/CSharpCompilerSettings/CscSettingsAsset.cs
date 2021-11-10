using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using LVersion = Coffee.CSharpCompilerSettings.CSharpLanguageVersion;

namespace Coffee.CSharpCompilerSettings
{
    internal enum Nullable
    {
        Disable,
        Enable,
        Warnings,
        Annotations,
    }

    internal enum CompilerType
    {
        BuiltIn,
        CustomPackage
    }

    [Serializable]
    public struct NugetPackage
    {
        public enum CategoryType
        {
            Compiler,
            Analyzer,
        }

        [SerializeField] private string m_Name;
        [SerializeField] private string m_Version;
        [SerializeField] private CategoryType m_Category;

        public string PackageId
        {
            get { return m_Name + "." + m_Version; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public string Version
        {
            get { return m_Version; }
        }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(m_Name) && !string.IsNullOrEmpty(m_Version); }
        }

        public CategoryType Category
        {
            get { return m_Category; }
        }

        public NugetPackage(string name, string version, CategoryType category = CategoryType.Compiler)
        {
            m_Name = name;
            m_Version = version;
            m_Category = category;
        }

        public override string ToString()
        {
            return PackageId;
        }
    }

    [Serializable]
    public struct AssemblyFilter
    {
        [Tooltip("Include predefined assemblies (Assembly-CSharp-firstpass.dll, Assembly-CSharp-Editor-firstpass.dll, Assembly-CSharp.dll and Assembly-CSharp-Editor.dll)")]
        [SerializeField]
        private bool m_PredefinedAssemblies;

        [Tooltip("Assemblies filter. Prefix '!' to exclude. (e.g. 'Assets/', '!Packages/')")]
        [SerializeField]
        private string[] m_IncludedAssemblies;

        public AssemblyFilter(bool predefinedAssemblies, string[] includedAssemblies)
        {
            m_PredefinedAssemblies = predefinedAssemblies;
            m_IncludedAssemblies = includedAssemblies;
        }

        public bool IsValid(string asmdefPath)
        {
            if (string.IsNullOrEmpty(asmdefPath)) return m_PredefinedAssemblies;
            return m_IncludedAssemblies.Any(x => 0 < x.Length && x[0] != '!' && asmdefPath.Contains(x))
                   && !m_IncludedAssemblies.Any(x => 1 < x.Length && x[0] == '!' && asmdefPath.Contains(x.Substring(1)));
        }
    }

    internal class CscSettingsAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public const string k_SettingsPath = "ProjectSettings/CSharpCompilerSettings.asset";

        [SerializeField] private int m_Version = 0;
        [SerializeField] private CompilerType m_CompilerType = CompilerType.BuiltIn;
        [SerializeField] private CSharpLanguageVersion m_LanguageVersion = CSharpLanguageVersion.Latest;
        [SerializeField] private bool m_EnableLogging = false;
        [SerializeField] private Nullable m_Nullable = Nullable.Disable;
        [SerializeField] private NugetPackage m_CompilerPackage = new NugetPackage("Microsoft.Net.Compilers", "3.5.0", NugetPackage.CategoryType.Compiler);
        [SerializeField] private AssemblyFilter m_CompilerFilter = new AssemblyFilter(true, new[] { "Assets/", "!Assets/Standard Assets/", "!Packages/" });
        [SerializeField] private NugetPackage[] m_AnalyzerPackages = new NugetPackage[0];
        [SerializeField] private AssemblyFilter m_AnalyzerFilter = new AssemblyFilter(true, new[] { "Assets/", "!Assets/Standard Assets/", "!Packages/" });
        [SerializeField] private string[] m_SymbolModifier = new string[0];
        [SerializeField] [Obsolete] private bool m_UseDefaultCompiler = true;
        [SerializeField] [Obsolete] private string m_PackageName = "Microsoft.Net.Compilers";
        [SerializeField] [Obsolete] private string m_PackageVersion = "3.5.0";
        [SerializeField] [Obsolete] private bool m_EnableNullable = false;
        [SerializeField] [Obsolete] private string m_ModifySymbols = "";

        private static CscSettingsAsset CreateFromProjectSettings()
        {
            s_Instance = File.Exists(k_SettingsPath)
                ? CreateFromJson(File.ReadAllText(k_SettingsPath))
                : CreateInstance<CscSettingsAsset>();
            s_Instance.IsProjectSetting = true;
            return s_Instance;
        }

        public bool IsProjectSetting { get; private set; }
        private static CscSettingsAsset s_Instance;
        public static CscSettingsAsset instance => s_Instance ? s_Instance : s_Instance = CreateFromProjectSettings();
        public NugetPackage CompilerPackage => m_CompilerPackage;
        public Nullable Nullable => m_Nullable;
        public NugetPackage[] AnalyzerPackages => m_AnalyzerPackages;
        public bool EnableDebugLog => m_EnableLogging;
        public bool UseDefaultCompiler => m_CompilerType == CompilerType.BuiltIn || !m_CompilerPackage.IsValid;

        public string LanguageVersion
        {
            get
            {
                switch (m_LanguageVersion)
                {
                    case CSharpLanguageVersion.CSharp7: return "7";
                    case CSharpLanguageVersion.CSharp7_1: return "7.1";
                    case CSharpLanguageVersion.CSharp7_2: return "7.2";
                    case CSharpLanguageVersion.CSharp7_3: return "7.3";
                    case CSharpLanguageVersion.CSharp8: return "8.0";
                    case CSharpLanguageVersion.CSharp9: return "9.0";
                    case CSharpLanguageVersion.Preview: return "preview";
                    default: return "latest";
                }
            }
        }

        public string GetSymbolModifier(string asmdefPath)
        {
            var sb = new StringBuilder();
            if (ShouldToUseCustomCompiler(asmdefPath))
            {
                var v = m_LanguageVersion;
                if (v == LVersion.Preview) v = LVersion.CSharp9;
                if (v == LVersion.Latest) v = LVersion.CSharp8;

                sb.Append(LVersion.CSharp7 <= v ? "CSHARP_7_OR_NEWER;" : "!CSHARP_7_OR_NEWER;");
                sb.Append(LVersion.CSharp7_1 <= v ? "CSHARP_7_1_OR_NEWER;" : "!CSHARP_7_1_OR_NEWER;");
                sb.Append(LVersion.CSharp7_2 <= v ? "CSHARP_7_2_OR_NEWER;" : "!CSHARP_7_2_OR_NEWER;");
                sb.Append(LVersion.CSharp7_3 <= v ? "CSHARP_7_3_OR_NEWER;" : "!CSHARP_7_3_OR_NEWER;");
                sb.Append(LVersion.CSharp8 <= v ? "CSHARP_8_OR_NEWER;" : "!CSHARP_8_OR_NEWER;");
                sb.Append(LVersion.CSharp9 <= v ? "CSHARP_9_OR_NEWER;" : "!CSHARP_9_OR_NEWER;");

                sb.Append("CUSTOM_COMPILE;");
            }

            if (!IsProjectSetting && 0 < m_SymbolModifier.Length)
            {
                sb.Append(m_SymbolModifier.Aggregate((a, b) => a + ";" + b));
            }

            return sb.ToString();
        }

        public bool IsSupportNullable
        {
            get { return LVersion.CSharp8 <= m_LanguageVersion; }
        }

        public static CscSettingsAsset GetAtPath(string path)
        {
            try
            {
                return string.IsNullOrEmpty(Core.GetPortableDllPath(path))
                    ? null
                    : CreateFromJson(AssetImporter.GetAtPath(path).userData);
            }
            catch
            {
                return null;
            }
        }

        public bool ShouldToRecompile(string asmdefPath)
        {
            return 0 < GetSymbolModifier(asmdefPath).Length
                   || ShouldToUseCustomCompiler(asmdefPath)
                   || ShouldToUseAnalyzer(asmdefPath);
        }

        public bool ShouldToUseCustomCompiler(string asmdefPath)
        {
            return m_CompilerType == CompilerType.CustomPackage
                   && m_CompilerPackage.IsValid
                   && (!IsProjectSetting || m_CompilerFilter.IsValid(asmdefPath));
        }

        public bool ShouldToUseAnalyzer(string asmdefPath)
        {
            return m_AnalyzerPackages.Any(x => x.IsValid) && m_AnalyzerFilter.IsValid(asmdefPath);
        }

        public static CscSettingsAsset CreateFromJson(string json = "")
        {
            var setting = CreateInstance<CscSettingsAsset>();
            JsonUtility.FromJsonOverwrite(json, setting);

            // In Unity 2020.2 or later, some fields will be not deserialized on first compilation.
            var serializedJson = JsonUtility.ToJson(setting);
            if (!serializedJson.Contains("\"m_CompilerPackage\":"))
            {
                var m = Regex.Match(json, "\"m_CompilerPackage\":\\s*({[^}]+})");
                if (m.Success)
                {
                    setting.m_CompilerPackage = JsonUtility.FromJson<NugetPackage>(m.Groups[1].Value);
                }
            }

            if (!serializedJson.Contains("\"m_CompilerFilter\":"))
            {
                var m = Regex.Match(json, "\"m_CompilerFilter\":\\s*({[^}]+})");
                if (m.Success)
                {
                    setting.m_CompilerFilter = JsonUtility.FromJson<AssemblyFilter>(m.Groups[1].Value);
                }
            }

            return setting;
        }

        public void OnBeforeSerialize()
        {
#pragma warning disable 0612
            m_UseDefaultCompiler = m_CompilerType == CompilerType.BuiltIn;
            m_EnableNullable = m_Nullable != Nullable.Disable;
            m_PackageName = m_CompilerPackage.Name;
            m_PackageVersion = m_CompilerPackage.Version;
            m_ModifySymbols = 0 < m_SymbolModifier.Length ? m_SymbolModifier.Aggregate((a, b) => a + ";" + b) : "";
#pragma warning restore 0612
        }

        public void OnAfterDeserialize()
        {
#pragma warning disable 0612
            if (m_Version < 110)
            {
                m_Version = 110;
                m_CompilerType = m_UseDefaultCompiler
                    ? CompilerType.BuiltIn
                    : CompilerType.CustomPackage;
            }

            if (m_Version < 130)
            {
                m_Version = 130;
                m_Nullable = m_EnableNullable
                    ? Nullable.Enable
                    : Nullable.Disable;
            }

            if (m_Version < 140)
            {
                m_Version = 140;
                m_CompilerPackage = new NugetPackage(m_PackageName, m_PackageVersion, NugetPackage.CategoryType.Compiler);
                m_SymbolModifier = m_ModifySymbols
                    .Split(',', ';')
                    .Where(x => 0 < x.Length)
                    .ToArray();
            }
#pragma warning restore 0612
        }
    }
}
