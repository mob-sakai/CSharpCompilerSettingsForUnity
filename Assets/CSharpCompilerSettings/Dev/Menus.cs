using System.IO;
using System.Linq;
using UnityEditor;

namespace Coffee.CSharpCompilerSettings
{
    internal static class Menus
    {
        private const string k_DevelopModeText = "Csc Settings/Develop Mode";
        private const string k_DevelopModeSymbol = "CSC_SETTINGS_DEVELOP";

        private const string k_EditorAsmdefPath = "Packages/com.coffee.csharp-compiler-settings/Editor/CSharpCompilerSettings.Editor.asmdef";
        private const string k_EditorAsmdef = "Assets/CSharpCompilerSettings/Dev/CSharpCompilerSettings.Editor.asmdef~";
        private const string k_EditorAsmdefDevelop = "Assets/CSharpCompilerSettings/Dev/CSharpCompilerSettings.Editor.asmdef.Dev~";


        [MenuItem(k_DevelopModeText, false)]
        private static void DevelopMode()
        {
            SwitchSymbol(k_DevelopModeSymbol);
            if (HasSymbol(k_DevelopModeSymbol))
            {
                File.Copy(k_EditorAsmdefDevelop, k_EditorAsmdefPath, true);
            }
            else
            {
                File.Copy(k_EditorAsmdef, k_EditorAsmdefPath, true);
            }

            AssetDatabase.ImportAsset(k_EditorAsmdefPath);
            AssetDatabase.Refresh();
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
    }
}
