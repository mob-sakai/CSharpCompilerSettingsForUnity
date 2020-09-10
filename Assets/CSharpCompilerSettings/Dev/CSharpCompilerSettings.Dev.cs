using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Compilation;

namespace CSharpCompilierSettings
{
    [InitializeOnLoad]
    internal static class Dev
    {
        const string k_AssemblySrc = "Library/ScriptAssemblies/CSharpCompilerSettings.dll";
        const string k_AssemblyDst = "Packages/CSharpCompilerSettings/Plugins/CSharpCompilerSettings.dll";

        private const string k_DebugModeText = "Csc Settings/Debug Mode";
        private const string k_DebugModeSymbol = "CSC_SETTINGS_DEBUG";

        private const string k_DevelopModeText = "Csc Settings/Develop Mode";
        private const string k_DevelopModeSymbol = "CSC_SETTINGS_DEVELOP";

        [MenuItem(k_DebugModeText, false)]
        private static void DebugMode()
        {
            SwitchSymbol(k_DebugModeSymbol);
        }

        [MenuItem(k_DebugModeText, true)]
        private static bool DebugMode_Valid()
        {
            Menu.SetChecked(k_DebugModeText, HasSymbol(k_DebugModeSymbol));
            return true;
        }

        [MenuItem(k_DevelopModeText, false)]
        private static void DevelopMode()
        {
            SwitchSymbol(k_DevelopModeSymbol);
        }

        [MenuItem(k_DevelopModeText, true)]
        private static bool DevelopMode_Valid()
        {
            Menu.SetChecked(k_DevelopModeText, HasSymbol(k_DevelopModeSymbol));
            return true;
        }

        private static string[] GetSymbols()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';', ',');
        }

        private static void SetSymbols(string[] symbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols));
        }

        private static bool HasSymbol(string symbol)
        {
            return GetSymbols().Any(x => x == symbol);
        }

        private static void SwitchSymbol(string symbol)
        {
            var symbols = GetSymbols();
            SetSymbols(symbols.Any(x => x == symbol)
                ? symbols.Where(x => x != symbol).ToArray()
                : symbols.Concat(new[] {symbol}).ToArray()
            );
        }

        private static void CopyAssemblyToPackage(string assemblyPath, CompilerMessage[] messages)
        {
            if (k_AssemblySrc != assemblyPath || !messages.All(x => x.type != CompilerMessageType.Error)) return;

            UnityEngine.Debug.LogFormat("OnAssemblyCompilationFinished: Copy {0} to {1}", k_AssemblySrc, k_AssemblyDst);
            CopyFileIfUpdated(k_AssemblySrc, k_AssemblyDst);
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

        static Dev()
        {
            CompilationPipeline.assemblyCompilationFinished += CopyAssemblyToPackage;
        }
    }
}
