using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class CscSettingsProvider
    {
        private static SerializedObject serializedObject;
        private static SerializedProperty s_EnableLogging;
        private static SerializedProperty s_AnalyzerFilter;
        private static SerializedProperty s_PredefinedAssemblies;
        private static ReorderableList s_RoAnalyzerPackages;
        private static ReorderableList s_RoIncludedAssemblies;

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            serializedObject = new SerializedObject(CscSettingsAsset.instance);
            s_EnableLogging = serializedObject.FindProperty("m_EnableLogging");
            s_AnalyzerFilter = serializedObject.FindProperty("m_AnalyzerFilter");
            s_PredefinedAssemblies = s_AnalyzerFilter.FindPropertyRelative("m_PredefinedAssemblies");

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
                    sp.FindPropertyRelative("m_Category").intValue = (int)NugetPackage.CategoryType.Analyzer;
                }
            };

            var includedAssemblies = s_AnalyzerFilter.FindPropertyRelative("m_IncludedAssemblies");
            s_RoIncludedAssemblies = new ReorderableList(serializedObject, includedAssemblies);
            s_RoIncludedAssemblies.drawHeaderCallback = rect =>
            {
                EditorGUI.PrefixLabel(rect, new GUIContent(includedAssemblies.displayName));

                rect.x += rect.width - 100;
                rect.width = 100;
                EditorGUI.LabelField(rect, "* Prefix '!' to exclude.", EditorStyles.miniLabel);
            };
            s_RoIncludedAssemblies.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            s_RoIncludedAssemblies.drawElementCallback = (rect, index, active, focused) =>
            {
                var sp = includedAssemblies.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, sp, GUIContent.none);
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
            EditorGUILayout.LabelField("Compiler", EditorStyles.boldLabel);
            InspectorGUI.DrawCompilerPackage(serializedObject);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Analyzer", EditorStyles.boldLabel);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                using (new GUILayout.VerticalScope())
                {
                    NugetPackageCatalog.CurrentCategory = NugetPackage.CategoryType.Analyzer;
                    s_RoAnalyzerPackages.DoLayoutList();
                    s_RoIncludedAssemblies.DoLayoutList();
                    GUILayout.Space(-18);
                    EditorGUILayout.PropertyField(s_PredefinedAssemblies);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(s_EnableLogging);

            // Controls
            InspectorGUI.DrawControl(serializedObject.hasModifiedProperties,
                onRevert: () => { serializedObject = new SerializedObject(CscSettingsAsset.instance); },
                onApply: () =>
                {
                    serializedObject.ApplyModifiedProperties();
                    File.WriteAllText(CscSettingsAsset.k_SettingsPath, JsonUtility.ToJson(serializedObject.targetObject, true));
                    Utils.RequestCompilation();
                }
            );
        }
    }
}
