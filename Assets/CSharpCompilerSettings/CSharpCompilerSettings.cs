using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using LVersion = Coffee.CSharpCompilierSettings.CSharpLanguageVersion;

namespace Coffee.CSharpCompilierSettings
{
    internal enum CSharpLanguageVersion
    {
        CSharp7 = 700,
        CSharp7_1 = 701,
        CSharp7_2 = 702,
        CSharp7_3 = 703,
        CSharp8 = 800,
        CSharp9 = 900,
        Preview = int.MaxValue - 1,
        Latest = int.MaxValue,
    }

    internal class CscSettings : ScriptableObject
    {
        public const string k_SettingsPath = "ProjectSettings/CSharpCompilerSettings.asset";

        [SerializeField] private bool m_UseDefaultCompiler = true;
        [SerializeField] private string m_PackageName = "Microsoft.Net.Compilers";
        [SerializeField] private string m_PackageVersion = "3.5.0";
        [SerializeField] private CSharpLanguageVersion m_LanguageVersion = CSharpLanguageVersion.Latest;

        internal static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }

        private static CscSettings Create()
        {
            s_Instance = CreateInstance<CscSettings>();
            if (File.Exists(k_SettingsPath))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(k_SettingsPath), s_Instance);
            s_Instance.OnValidate();
            return s_Instance;
        }

        private static CscSettings s_Instance;

        public static CscSettings instance
        {
            get { return s_Instance ? s_Instance : s_Instance = Create(); }
        }

        public string PackageId
        {
            get { return m_PackageName + "." + m_PackageVersion; }
        }

        public bool UseDefaultCompiler
        {
            get { return m_UseDefaultCompiler; }
        }

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
                    case CSharpLanguageVersion.CSharp8: return "8";
                    case CSharpLanguageVersion.CSharp9: return "9";
                    case CSharpLanguageVersion.Preview: return "preview";
                    default: return "latest";
                }
            }
        }

        private void OnValidate()
        {
            if (!s_Instance) return;
            File.WriteAllText(k_SettingsPath, JsonUtility.ToJson(s_Instance, true));
        }
    }

    internal class CscSettingsProvider
    {
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            var serializedObject = CscSettings.GetSerializedObject();
            var keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(serializedObject);
            return new SettingsProvider("Project/C# Compiler", SettingsScope.Project)
            {
                label = "C# Compiler",
                keywords = keywords,
                guiHandler = OnGUI,
            };
        }

        private static void OnGUI(string searchContext)
        {
            var serializedObject = CscSettings.GetSerializedObject();
            var spUseDefaultCompiler = serializedObject.FindProperty("m_UseDefaultCompiler");
            var spPackageName = serializedObject.FindProperty("m_PackageName");
            var spPackageVersion = serializedObject.FindProperty("m_PackageVersion");
            var spLanguageVersion = serializedObject.FindProperty("m_LanguageVersion");

            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(spUseDefaultCompiler);
                if (ccs.changed)
                    Core.RequestScriptCompilation();
            }

            EditorGUILayout.PropertyField(spPackageName);
            EditorGUILayout.PropertyField(spPackageVersion);
            EditorGUILayout.PropertyField(spLanguageVersion);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
