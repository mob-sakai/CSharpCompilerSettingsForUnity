using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class CscSettingsProvider
    {
        private static SerializedObject serializedObject;
        private static SerializedProperty s_EnableLogging;

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            serializedObject = new SerializedObject(CscSettingsAsset.instance);
            s_EnableLogging = serializedObject.FindProperty("m_EnableLogging");
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

            {
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
