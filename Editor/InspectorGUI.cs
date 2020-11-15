using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coffee.CSharpCompilerSettings
{
    [InitializeOnLoad]
    internal static class InspectorGUI
    {
        private const string k_BaseDll = "Packages/com.coffee.csharp-compiler-settings/Plugins/CSharpCompilerSettings.dll";

        private static GUIContent s_EnableText = new GUIContent("Enable C# Compiler Settings", "Enable C# compiler settings for this assembly.");

        private static GUIContent s_ModifySymbolsText = new GUIContent("Modify Symbols",
            "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");

        static GUIContent s_PublishText = new GUIContent("Publish as dll", "Publish this assembly as dll to the parent directory.");
        static GUIContent s_ReloadText = new GUIContent("Reload", "Reload C# compiler settings dll for this assembly.");
        static GUIContent s_HelpText = new GUIContent("Help", "Open C# compiler settings help page on browser.");
        static GUIContent s_ApplyText = new GUIContent("Apply");
        static GUIContent s_RevertText = new GUIContent("Revert");

        private static SerializedObject _serializedObject;
        private static bool _hasPortableDll = false;
        private static bool _changed = false;
        private static string _assetPath;

        private static string[] _ignoredAssetPaths =
        {
            "Assets/CSharpCompilerSettings/Dev/CSharpCompilerSettings.Dev.asmdef",
            "Assets/CSharpCompilerSettings/CSharpCompilerSettings_.asmdef",
            "Packages/com.coffee.csharp-compiler-settings/Editor/CSharpCompilerSettings.Editor.asmdef",
        };

        static InspectorGUI()
        {
            s_ModifySymbolsText = new GUIContent("Modify Symbols",
                "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");
            s_EnableText = new GUIContent("Enable C# Compiler Settings", "Enable C# compiler settings for this assembly.");
            s_PublishText = new GUIContent("Publish as dll", "Publish this assembly as dll to the parent directory.");
            s_HelpText = new GUIContent("Help", "Open C# compiler settings help page on browser.");

            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;

            // Select asmdef asset on load.
            var activeObject = Selection.activeObject;
            if (activeObject is AssemblyDefinitionAsset)
            {
                Selection.activeObject = null;
                EditorApplication.delayCall += () => Selection.activeObject = activeObject;
            }

            Selection.selectionChanged += () => { _assetPath = null; };
        }

        private static void OnPostHeaderGUI(Editor editor)
        {
            var importer = editor.target as AssemblyDefinitionImporter;
            if (!importer || 1 < editor.targets.Length || _ignoredAssetPaths.Contains(importer.assetPath))
                return;

            if (_assetPath == null || _assetPath != importer.assetPath)
            {
                _assetPath = importer.assetPath;
                _serializedObject = new SerializedObject(CscSettingsAsset.CreateFromJson(importer.userData));
                _hasPortableDll = Core.GetPortableDllPath(importer.assetPath) != null;
                _changed = false;
            }

            // Enable.
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();
            _hasPortableDll = EditorGUILayout.ToggleLeft(s_EnableText, _hasPortableDll, EditorStyles.boldLabel);
            if (_hasPortableDll)
            {
                DrawCompilerPackage(_serializedObject);
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_ModifySymbols"));
            }

            _changed |= EditorGUI.EndChangeCheck();

            // Controls
            DrawControl(_changed,
                onRevert: () => _assetPath = null,
                onApply: () =>
                {
                    _assetPath = null;
                    _serializedObject.ApplyModifiedProperties();
                    importer.userData = JsonUtility.ToJson(_serializedObject.targetObject);
                    importer.SaveAndReimport();

                    var hasPortableDll = Core.GetPortableDllPath(importer.assetPath) != null;
                    if (_hasPortableDll != hasPortableDll)
                        EnablePortableDll(importer.assetPath, _hasPortableDll);
                },
                onReload: () =>
                {
                    _assetPath = null;
                    EnablePortableDll(importer.assetPath, true);
                    AutoImporter.ImportOnFinishedCompilation(importer.assetPath);
                },
                onPublish: () =>
                {
                    _assetPath = null;
                    AutoImporter.RequestPublishDll(importer.assetPath, Core.GetAssemblyName(importer.assetPath));
                    importer.SaveAndReimport();
                }
            );

            EditorGUILayout.EndVertical();
        }

        public static void DrawCompilerPackage(SerializedObject so)
        {
            var spCompilerType = so.FindProperty("m_CompilerType");
            EditorGUILayout.PropertyField(spCompilerType);

            if (spCompilerType.intValue == (int) CompilerType.CustomPackage)
            {
                EditorGUI.indentLevel++;
                NugetPackageCatalog.CurrentCategory = NugetPackage.CategoryType.Compiler;
                EditorGUILayout.PropertyField(so.FindProperty("m_CompilerPackage"), GUIContent.none);
                EditorGUILayout.PropertyField(so.FindProperty("m_LanguageVersion"));
                EditorGUILayout.PropertyField(so.FindProperty("m_Nullable"));
                EditorGUI.indentLevel--;
            }
        }

        public static void DrawControl(bool changed, Action onRevert = null, Action onApply = null, Action onReload = null, Action onPublish = null)
        {
            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(!changed))
                {
                    if (onRevert != null && GUILayout.Button(s_RevertText))
                        onRevert();

                    if (onApply != null && GUILayout.Button(s_ApplyText))
                        onApply();
                }

                if (onReload != null && GUILayout.Button(s_ReloadText))
                    onReload();

                if (onPublish != null && GUILayout.Button(s_PublishText))
                    onPublish();
            }
        }

        /// <summary>
        /// Install/Uninstall a portable CSharpCompilerSettings.dll in the asmdef directory.
        /// In portable mode, a unique suffix is added to the assembly name and file name.
        /// </summary>
        /// <param name="asmdefPath">Path of the asmdef</param>
        /// <param name="enabled">Condition</param>
        private static void EnablePortableDll(string asmdefPath, bool enabled)
        {
            if (enabled)
            {
                var guid = Directory.GetFiles(Path.GetDirectoryName(asmdefPath))
                    .Select(x => Regex.Match(x, "CSharpCompilerSettings_([0-9a-zA-Z]{32}).dll").Groups[1].Value)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x));

                if (string.IsNullOrEmpty(guid))
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                var tmpDst = "Temp/" + Path.GetFileName(Path.GetTempFileName());
                var dst = Path.GetDirectoryName(asmdefPath) + "/CSharpCompilerSettings_" + guid + ".dll";

                // Copy dll with renaming assembly name.
                File.Copy(k_BaseDll, tmpDst, true);
                AssemblyRenamer.Rename(tmpDst, Path.GetFileNameWithoutExtension(dst));
                EditorUtils.CopyFileIfNeeded(tmpDst, dst);

                // Copy meta.
                File.Copy(k_BaseDll + ".meta~", tmpDst + ".meta", true);
                var meta = File.ReadAllText(tmpDst + ".meta");
                meta = Regex.Replace(meta, "<<GUID>>", guid);
                File.WriteAllText(tmpDst + ".meta", meta);
                EditorUtils.CopyFileIfNeeded(tmpDst + ".meta", dst + ".meta");

                // Request to compile.
                EditorApplication.delayCall += AssetDatabase.Refresh;
            }
            else
            {
                var dir = Path.GetDirectoryName(asmdefPath);
                foreach (var path in Directory.GetFiles(dir, "*.dll")
                    .Where(x => Regex.IsMatch(x, "CSharpCompilerSettings_[0-9a-zA-Z]{32}.dll"))
                )
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(SplitAttribute))]
    public class SplitDrawer : PropertyDrawer
    {
        private ReorderableList _ro = null;
        private List<string> _list;
        private static GUIContent s_PrefixHint = new GUIContent("* Prefix '!' to exclude");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) return EditorGUIUtility.singleLineHeight;

            var s = property.stringValue;
            var c = (attribute as SplitAttribute).separater;
            var count = s.Length - s.Replace(c.ToString(), "").Length + 1;
            return count * (EditorGUIUtility.singleLineHeight + 2) + 36;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use Split with string.");
                return;
            }

            var separater = (attribute as SplitAttribute).separater;
            if (_ro == null)
            {
                _list = property.stringValue.Split(separater).ToList();

                _ro = new ReorderableList(_list, typeof(string));
                _ro.drawHeaderCallback = rect =>
                {
                    EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
                    rect.x += rect.width - 100;
                    rect.width = 100;
                    EditorGUI.LabelField(rect, s_PrefixHint, EditorStyles.miniLabel);
                };
                _ro.onAddCallback = list => list.list.Add("");
                _ro.elementHeight = EditorGUIUtility.singleLineHeight + 2;
                _ro.drawElementCallback = (rect, index, active, focused) =>
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    _list[index] = EditorGUI.TextField(rect, _list[index]);
                };
            }

            EditorGUI.BeginChangeCheck();
            _ro.DoList(position);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = 0 < _list.Count
                    ? _list.Aggregate((a, b) => a + ";" + b)
                    : "";
            }
        }
    }
}
