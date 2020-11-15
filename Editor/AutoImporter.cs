using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace Coffee.CSharpCompilerSettings
{
    internal class AutoImporter : ScriptableSingleton<AutoImporter>
    {
        public string assetPath;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (string.IsNullOrEmpty(instance.assetPath)) return;

            AssetDatabase.ImportAsset(instance.assetPath);
            instance.assetPath = null;
        }

        public static void ImportOnFinishedCompilation(string assetPath)
        {
            instance.assetPath = assetPath;
        }

        public static void RequestPublishDll(string asmdefPath, string assemblyName)
        {
            Action<string, CompilerMessage[]> callback = null;

            callback = (name, messages) =>
            {
                // This assembly is requested to publish?
                var compiledAssemblyName = Path.GetFileNameWithoutExtension(name);
                if (assemblyName != compiledAssemblyName)
                    return;

                CompilationPipeline.assemblyCompilationFinished -= callback;
                Logger.LogInfo("Assembly compilation finished: <b>{0} is requested to publish.</b>", compiledAssemblyName);

                // No compilation error?
                if (messages.Any(x => x.type == CompilerMessageType.Error))
                    return;

                // Publish a dll to parent directory.
                var dst = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(asmdefPath)), compiledAssemblyName + ".dll");
                var src = "Library/ScriptAssemblies/" + Path.GetFileName(dst);
                Logger.LogInfo("<b>Publish assembly as dll:</b> " + dst);
                EditorUtils.CopyFileIfNeeded(Path.GetFullPath(src), Path.GetFullPath(dst));

                EditorApplication.delayCall += () => AssetDatabase.ImportAsset(dst);
            };

            CompilationPipeline.assemblyCompilationFinished += callback;
            Logger.LogInfo("<b><color=#22aa22>Request to publish dll:</color> {0}</b>", assemblyName);
        }
    }
}
