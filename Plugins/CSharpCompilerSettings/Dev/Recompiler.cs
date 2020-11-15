using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coffee.CSharpCompilerSettings
{
    internal class Recompiler : ScriptableSingleton<Recompiler>
    {
        private const string k_AssemblySrc = "Library/ScriptAssemblies/CSharpCompilerSettings_.dll";
        private const string k_ResponseFileSrc = "Packages/com.coffee.csharp-compiler-settings/Plugins/CSharpCompilerSettings/Dev/rsp";
        private const string k_ResponseFileDst = "Temp/CSharpCompilerSettings.rsp";

        [SerializeField] private bool requested;

#if CSC_SETTINGS_DEVELOP
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            CompilationPipeline.assemblyCompilationFinished += RequestToRecompileIfNeeded;

            if (!instance.requested) return;

            instance.requested = false;
            Recompile();
        }

        private static void RequestToRecompileIfNeeded(string assemblyPath, CompilerMessage[] messages)
        {
            if (k_AssemblySrc != assemblyPath || !messages.All(x => x.type != CompilerMessageType.Error)) return;

            instance.requested = true;
        }
#endif

        [MenuItem("Csc Settings/Recompile")]
        private static void Recompile()
        {
            var appContents = EditorApplication.applicationContentsPath;
            var pluginSourceAsmdefPath = AssetDatabase.GUIDToAssetPath("6ed54bcbddd234d55b95cd3dd4df4bec");
            var pluginSourceDir = Path.GetDirectoryName(Path.GetFullPath(pluginSourceAsmdefPath));
            pluginSourceDir = pluginSourceDir.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, "").Replace('\\', '/');


            // Update rsp file.
            var rsp = File.ReadAllText(k_ResponseFileSrc);
            rsp = rsp.Replace("${APP_CONTENTS}", appContents);
            rsp = rsp.Replace("${PLUGIN_SOURCE}", pluginSourceDir);
            rsp = rsp.Replace("${DLL_PATH}", Path.GetFullPath(AssetDatabase.GUIDToAssetPath("461bbb389771b4c5f98a24881230d53c")));
            File.WriteAllText(k_ResponseFileDst, rsp);

            // Detect csc tool exe.
            var mono = Application.platform != RuntimePlatform.WindowsEditor;
            var cscToolExe = appContents + "/Tools/RoslynNet46/csc.exe".Replace('/', Path.DirectorySeparatorChar);
            if (!File.Exists(cscToolExe))
                cscToolExe = appContents + "/Tools/Roslyn/csc.exe".Replace('/', Path.DirectorySeparatorChar);
            if (!File.Exists(cscToolExe))
            {
                cscToolExe = appContents + "/Tools/Roslyn/csc".Replace('/', Path.DirectorySeparatorChar);
                mono = false;
            }

            // Create compilation process.
            var psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            if (!mono)
            {
                psi.FileName = Path.GetFullPath(cscToolExe);
                psi.Arguments = "/shared /noconfig @" + k_ResponseFileDst;
            }
            else
            {
                psi.FileName = Path.Combine(appContents, "MonoBleedingEdge/bin/mono");
                psi.Arguments = cscToolExe + " /noconfig @" + k_ResponseFileDst;
            }

            // Start compilation process.
            Debug.LogFormat("Recompile: CSharpCompilerSettings.dll\n  command={0} {1}\n", psi.FileName, psi.Arguments);
            var p = Process.Start(psi);
            p.Exited += (_, __) =>
            {
                if (p.ExitCode == 0)
                    Debug.Log("Recompile: success.");
                else
                    Debug.LogErrorFormat("Recompile: failure.\n{0}\n\n{1}", p.StandardError.ReadToEnd(), p.StandardOutput.ReadToEnd());
            };
            p.EnableRaisingEvents = true;
        }
    }
}
