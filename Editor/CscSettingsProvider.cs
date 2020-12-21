using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class CscSettingsProvider
    {
        private static SerializedObject serializedObject;
        private static SerializedProperty s_EnableLogging;
        private static SerializedProperty s_CompilerFilter;
        private static SerializedProperty s_AnalyzerFilter;
        private static ReorderableList s_RoAnalyzerPackages;

        private static GUIContent s_HeaderCompiler = new GUIContent("Compiler");
        private static GUIContent s_HeaderAnalyzer = new GUIContent("Analyzer");
        private static GUIContent s_HeaderDebug = new GUIContent("Debug");


        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            serializedObject = new SerializedObject(CscSettingsAsset.instance);
            s_EnableLogging = serializedObject.FindProperty("m_EnableLogging");
            s_CompilerFilter = serializedObject.FindProperty("m_CompilerFilter");
            s_AnalyzerFilter = serializedObject.FindProperty("m_AnalyzerFilter");

            var analyzerPackages = serializedObject.FindProperty("m_AnalyzerPackages");
            s_RoAnalyzerPackages = new ReorderableList(serializedObject, analyzerPackages);
            s_RoAnalyzerPackages.drawHeaderCallback = rect => EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Analyzer Packages"));
            s_RoAnalyzerPackages.elementHeight = NugetPackageDrawer.Height;
            s_RoAnalyzerPackages.drawElementCallback = (rect, index, active, focused) =>
            {
                var sp = analyzerPackages.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, sp, GUIContent.none);
            };
            s_RoAnalyzerPackages.onChangedCallback = list =>
            {
                for (var i = 0; i < analyzerPackages.arraySize; i++)
                {
                    var sp = analyzerPackages.GetArrayElementAtIndex(i);
                    sp.FindPropertyRelative("m_Category").intValue = (int) NugetPackage.CategoryType.Analyzer;
                }
            };

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
            EditorGUILayout.LabelField(s_HeaderCompiler, EditorStyles.boldLabel);
            if (InspectorGUI.DrawCompilerPackage(serializedObject))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(4);
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(s_CompilerFilter, GUIContent.none, true);
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(s_HeaderAnalyzer, EditorStyles.boldLabel);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(4);
                using (new GUILayout.VerticalScope())
                {
                    NugetPackageCatalog.CurrentCategory = NugetPackage.CategoryType.Analyzer;
                    s_RoAnalyzerPackages.DoLayoutList();
                    EditorGUILayout.PropertyField(s_AnalyzerFilter, GUIContent.none, true);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(s_HeaderDebug, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(s_EnableLogging);

            // Controls
            InspectorGUI.DrawControl(serializedObject.hasModifiedProperties,
                onRevert: () => { serializedObject = new SerializedObject(CscSettingsAsset.instance); },
                onApply: () =>
                {
                    serializedObject.ApplyModifiedProperties();
                    File.WriteAllText(CscSettingsAsset.k_SettingsPath, JsonUtility.ToJson(serializedObject.targetObject, true));
                    Utils.RequestCompilation();
                },
                onReload: () =>
                {
                    if (!EditorUtility.DisplayDialog("Reload all portable CSharpCompilerSettings assemblies", "Reload all CSharpCompilerSettings_***.dll for asmdef files.", "OK", "Cancel")) return;

                    AssetDatabase.FindAssets("t:DefaultAsset CSharpCompilerSettings_")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Where(x => Regex.IsMatch(Path.GetFileName(x), "CSharpCompilerSettings_([0-9a-zA-Z]{32}).dll"))
                        .ToList()
                        .ForEach(x => InspectorGUI.EnablePortableDll(x, true));
                }
            );
        }
    }
}
