using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor.Compilation;
using System;
using System.Security.Cryptography;
using System.Linq;

namespace Coffee.CSharpCompilerSettings
{
    [InitializeOnLoad]
    internal static class InspectorGUI
    {
        private static string s_AssemblyNameToPublish;
        private static string s_AsmdefPathToPublish;
        private static GUIContent s_EnableText = new GUIContent("Enable C# Compiler Settings", "Enable C# compiler settings for this assembly.");

        private static GUIContent s_ModifySymbolsText = new GUIContent("Modify Symbols",
            "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");

        static GUIContent s_PublishText = new GUIContent("Publish as dll", "Publish this assembly as dll to the parent directory.");
        static GUIContent s_ReloadText = new GUIContent("Reload", "Reload C# compiler settings dll for this assembly.");
        static GUIContent s_HelpText = new GUIContent("Help", "Open C# compiler settings help page on browser.");
        static GUIContent s_ApplyText = new GUIContent("Apply");
        static GUIContent s_RevertText = new GUIContent("Revert");

        static void OnAssemblyCompilationFinished(string name, CompilerMessage[] messages)
        {
            try
            {
                // This assembly is requested to publish?
                var assemblyName = Path.GetFileNameWithoutExtension(name);
                if (s_AssemblyNameToPublish != assemblyName)
                    return;

                s_AssemblyNameToPublish = null;
                Core.LogInfo("Assembly compilation finished: <b>{0} is requested to publish.</b>", assemblyName);

                // No compilation error?
                if (messages.Any(x => x.type == CompilerMessageType.Error))
                    return;

                // Publish a dll to parent directory.
                var dst = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(s_AsmdefPathToPublish)), assemblyName + ".dll");
                var src = "Library/ScriptAssemblies/" + Path.GetFileName(dst);
                Core.LogInfo("<b>Publish assembly as dll:</b> " + dst);
                CopyFileIfUpdated(Path.GetFullPath(src), Path.GetFullPath(dst));

                EditorApplication.delayCall += () => AssetDatabase.ImportAsset(dst);
            }
            catch (Exception e)
            {
                Core.LogException(e);
            }
        }

        public static void CopyFileIfUpdated(string src, string dst)
        {
            src = Path.GetFullPath(src);
            if (!File.Exists(src))
                return;

            dst = Path.GetFullPath(dst);
            if (File.Exists(dst))
            {
                using (var srcFs = new FileStream(src, FileMode.Open))
                using (var dstFs = new FileStream(dst, FileMode.Open))
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    if (md5.ComputeHash(srcFs).SequenceEqual(md5.ComputeHash(dstFs)))
                        return;
                }
            }

            var dir = Path.GetDirectoryName(dst);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(src, dst, true);
        }

        static InspectorGUI()
        {
            s_ModifySymbolsText = new GUIContent("Modify Symbols",
                "When compiling this assembly, add or remove specific symbols separated with semicolons (;) or commas (,).\nSymbols starting with '!' will be removed.\n\ne.g. 'SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...'");
            s_EnableText = new GUIContent("Enable C# Compiler Settings", "Enable C# compiler settings for this assembly.");
            s_PublishText = new GUIContent("Publish as dll", "Publish this assembly as dll to the parent directory.");
            s_HelpText = new GUIContent("Help", "Open C# compiler settings help page on browser.");

            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;

            // Select asmdef asset on load.
            var activeObject = Selection.activeObject;
            if (activeObject is AssemblyDefinitionAsset)
            {
                Selection.activeObject = null;
                EditorApplication.delayCall += () => Selection.activeObject = activeObject;
            }

            Selection.selectionChanged += () => { _assetPath = null; };
        }

        private static SerializedObject _serializedObject;
        private static bool _hasPortableDll = false;
        private static bool _changed = false;
        private static string _assetPath;
        private static string[] _ignoredAssetPaths = {
            "Assets/CSharpCompilerSettings/Dev/CSharpCompilerSettings.Dev.asmdef",
            "Assets/CSharpCompilerSettings/CSharpCompilerSettings.asmdef",
            "Packages/com.coffee.csharp-compiler-settings/Editor/CSharpCompilerSettings.Editor.asmdef",
        };


        private static void OnPostHeaderGUI(Editor editor)
        {
            var importer = editor.target as AssemblyDefinitionImporter;
            if (!importer || 1 < editor.targets.Length || _ignoredAssetPaths.Contains(importer.assetPath))
                return;

            if (_assetPath == null || _assetPath != importer.assetPath)
            {
                _assetPath = importer.assetPath;
                _serializedObject = new SerializedObject(CscSettingsAsset.CreateFromJson(importer.userData));
                _hasPortableDll = Core.HasPortableDll(importer.assetPath);
                _changed = false;
            }

            // Enable.
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();
            _hasPortableDll = EditorGUILayout.ToggleLeft(s_EnableText, _hasPortableDll, EditorStyles.boldLabel);
            if (_hasPortableDll)
            {
                var spCompilerType = _serializedObject.FindProperty("m_CompilerType");
                EditorGUILayout.PropertyField(spCompilerType);

                if (spCompilerType.intValue == (int) CompilerType.CustomPackage)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_PackageName"));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_PackageVersion"));
                    EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_LanguageVersion"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_ModifySymbols"));
            }

            _changed |= EditorGUI.EndChangeCheck();

            // Controls
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(!_changed))
                {
                    if (GUILayout.Button(s_RevertText))
                    {
                        _assetPath = null;
                    }

                    if (GUILayout.Button(s_ApplyText))
                    {
                        _assetPath = null;
                        _serializedObject.ApplyModifiedProperties();
                        importer.userData = JsonUtility.ToJson(_serializedObject.targetObject);
                        importer.SaveAndReimport();

                        if (_hasPortableDll != Core.HasPortableDll(importer.assetPath))
                            EnablePortableDll(importer.assetPath, _hasPortableDll);
                    }
                }

                using (new EditorGUI.DisabledScope(!_hasPortableDll))
                {
                    if (GUILayout.Button(s_ReloadText))
                    {
                        _assetPath = null;
                        EnablePortableDll(importer.assetPath, true);
                        AutoImporter.ImportOnFinishedCompilation(importer.assetPath);
                    }
                }

                if (GUILayout.Button(s_PublishText))
                {
                    _assetPath = null;
                    s_AsmdefPathToPublish = importer.assetPath;
                    s_AssemblyNameToPublish = Core.GetAssemblyName(importer.assetPath);
                    Core.LogInfo("<b><color=#22aa22>Request to publish dll:</color> {0}</b>", s_AssemblyNameToPublish);

                    importer.SaveAndReimport();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private static void EnablePortableDll(string asmdefPath, bool enabled)
        {
            if (enabled)
            {
                var src = "Packages/com.coffee.csharp-compiler-settings/Plugins/CSharpCompilerSettings.dll";
                var guid = Directory.GetFiles(Path.GetDirectoryName(asmdefPath))
                    .Select(x => Regex.Match(x, "CSharpCompilerSettings_([0-9a-zA-Z]{32}).dll").Groups[1].Value)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x));

                if (string.IsNullOrEmpty(guid))
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                var tmpDst = "Temp/" + Path.GetFileName(Path.GetTempFileName());
                var dst = Path.GetDirectoryName(asmdefPath) + "/CSharpCompilerSettings_" + guid + ".dll";

                // Copy dll with renaming assembly name.
                File.Copy(src, tmpDst, true);
                AssemblyRenamer.Rename(tmpDst, Path.GetFileNameWithoutExtension(dst));
                CopyFileIfNeeded(tmpDst, dst);

                // Copy meta.
                File.Copy(src + ".meta~", tmpDst + ".meta", true);
                var meta = File.ReadAllText(tmpDst + ".meta");
                meta = Regex.Replace(meta, "<<GUID>>", guid);
                File.WriteAllText(tmpDst + ".meta", meta);
                CopyFileIfNeeded(tmpDst + ".meta", dst + ".meta");

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

        private static void CopyFileIfNeeded(string src, string dst)
        {
            if (File.Exists(dst))
            {
                using (var md5 = MD5.Create())
                using (var srcStream = File.OpenRead(src))
                using (var dstStream = File.OpenRead(dst))
                    if (md5.ComputeHash(srcStream).SequenceEqual(md5.ComputeHash(dstStream)))
                    {
                        Core.LogInfo("  <color=#bbbb44><Skipped> No difference: {0}</color>", dst);
                        return;
                    }
            }

            File.Copy(src, dst, true);
        }
    }
}
