using UnityEditor;

namespace Coffee.CSharpCompilerSettings
{
    internal class AutoImporter: ScriptableSingleton<AutoImporter>
    {
        public string assetPath;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if(string.IsNullOrEmpty(instance.assetPath)) return;

            AssetDatabase.ImportAsset(instance.assetPath);
            instance.assetPath = null;
        }

        public static void ImportOnFinishedCompilation(string assetPath)
        {
            instance.assetPath = assetPath;
        }
    }
}
