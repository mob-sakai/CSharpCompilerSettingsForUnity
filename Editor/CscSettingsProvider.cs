using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class CscSettingsProvider
    {
        private static SerializedObject serializedObject;

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            serializedObject = new SerializedObject(CscSettingsAsset.instance);
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
            if (serializedObject == null)
                serializedObject = new SerializedObject(CscSettingsAsset.instance);

            var spCompilerType = serializedObject.FindProperty("m_CompilerType");
            EditorGUILayout.PropertyField(spCompilerType);

            if (spCompilerType.intValue == (int) CompilerType.CustomPackage)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PackageName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PackageVersion"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LanguageVersion"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_EnableLogging"));

            // Controls
            using (new EditorGUI.DisabledScope(!serializedObject.hasModifiedProperties))
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Revert"))
                {
                    serializedObject = new SerializedObject(CscSettingsAsset.instance);
                }

                if (GUILayout.Button("Apply"))
                {
                    serializedObject.ApplyModifiedProperties();
                    File.WriteAllText(CscSettingsAsset.k_SettingsPath, JsonUtility.ToJson(serializedObject.targetObject, true));
                    RequestScriptCompilation();
                }
            }
        }

        public static void RequestScriptCompilation()
        {
            Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor")
                .Call("DirtyAllScripts");
        }
    }
}
