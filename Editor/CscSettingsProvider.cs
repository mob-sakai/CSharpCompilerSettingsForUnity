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
        private static SerializedProperty s_IncludedAssemblies;
        private static ReorderableList s_RoAnalyzerPackages = null;


        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            serializedObject = new SerializedObject(CscSettingsAsset.instance);
            s_EnableLogging = serializedObject.FindProperty("m_EnableLogging");
            s_AnalyzerFilter = serializedObject.FindProperty("m_AnalyzerFilter");
            s_PredefinedAssemblies = s_AnalyzerFilter.FindPropertyRelative("m_PredefinedAssemblies");
            s_IncludedAssemblies = s_AnalyzerFilter.FindPropertyRelative("m_IncludedAssemblies");

            s_RoAnalyzerPackages = new ReorderableList(serializedObject, serializedObject.FindProperty("m_AnalyzerPackages"));
            s_RoAnalyzerPackages.drawHeaderCallback = rect => EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Analyzer Packages"));
            s_RoAnalyzerPackages.elementHeight = NugetPackageDrawer.Height;
            s_RoAnalyzerPackages.drawElementCallback = (rect, index, active, focused) =>
            {
                var sp = s_RoAnalyzerPackages.serializedProperty.GetArrayElementAtIndex(index);
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
                    EditorGUILayout.PropertyField(s_IncludedAssemblies);
                    GUILayout.Space(-16);
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
