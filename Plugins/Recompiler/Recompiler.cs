using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Coffee.CSharpCompilerSettings.Recompiler
{
    [Serializable]
    internal class RecompileInfo
    {
        [SerializeField] private string m_TriggerAssembly;
        [SerializeField] private string m_AsmdefGuid;
        [SerializeField] private string m_RspGuid;
        [SerializeField] private string m_DllGuid;

        public RecompileInfo(string triggerAssembly, string asmdefGuid, string rspGuid, string dllGuid)
        {
            m_TriggerAssembly = triggerAssembly;
            m_AsmdefGuid = asmdefGuid;
            m_RspGuid = rspGuid;
            m_DllGuid = dllGuid;
        }

        public bool Contains(string[] assets)
        {
            var asmdefDir = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(m_AsmdefGuid));
            return assets
                .Select(Path.GetDirectoryName)
                .Any(x => x == asmdefDir);
        }

        public bool IsTrigger(string assemblyPath)
        {
            return m_TriggerAssembly == Path.GetFileName(assemblyPath);
        }

        public void Compile()
        {
            var appContents = EditorApplication.applicationContentsPath;
            var pluginSourceAsmdefPath = AssetDatabase.GUIDToAssetPath(m_AsmdefGuid);
            var pluginSourceDir = Path.GetDirectoryName(Path.GetFullPath(pluginSourceAsmdefPath));
            pluginSourceDir = pluginSourceDir.Replace(Environment.CurrentDirectory + Path.DirectorySeparatorChar, "").Replace('\\', '/');

            // Update rsp file.
            var rspPath = AssetDatabase.GUIDToAssetPath(m_RspGuid);
            var dllPath = AssetDatabase.GUIDToAssetPath(m_DllGuid);
            var rsp = File.ReadAllText(rspPath);
            rsp = rsp.Replace("${APP_CONTENTS}", appContents);
            rsp = rsp.Replace("${PLUGIN_SOURCE}", pluginSourceDir);
            rsp = rsp.Replace("${DLL_PATH}", Path.GetFullPath(dllPath));
            rspPath = "Temp/" + Path.GetFileName(rspPath);
            File.WriteAllText(rspPath, rsp);

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
                psi.Arguments = "/shared /noconfig @" + rspPath;
            }
            else
            {
                psi.FileName = Path.Combine(appContents, "MonoBleedingEdge/bin/mono");
                psi.Arguments = cscToolExe + " /noconfig @" + rspPath;
            }

            // Start compilation process.
            var assemblyName = Path.GetFileName(dllPath);
            Debug.LogFormat("<b>Recompile [{0}]: start compilation.</b>\n  command = {1} {2}\n", assemblyName, psi.FileName, psi.Arguments);
            var p = Process.Start(psi);
            p.Exited += (_, __) =>
            {
                if (p.ExitCode == 0)
                    Debug.LogFormat("<b><color=#22aa22>Recompile [{0}]: success.</color></b>", assemblyName);
                else
                    Debug.LogErrorFormat("<b><color=#aa2222>Recompile [{0}]: failure.</color></b>\n{1}\n\n{2}", assemblyName, p.StandardError.ReadToEnd(), p.StandardOutput.ReadToEnd());
            };
            p.EnableRaisingEvents = true;
        }
    }

    internal class RecompileTrigger : ScriptableSingleton<RecompileTrigger>
    {
        [SerializeField] private List<RecompileInfo> requestedCompilerInfo = new List<RecompileInfo>();

        private static readonly RecompileInfo[] s_CompilerInfos = new[]
        {
            new RecompileInfo(
                "CSharpCompilerSettings_.dll", // assembly name
                "6ed54bcbddd234d55b95cd3dd4df4bec", // asmdef
                "4fddfccbef8964f1bb68095098ca98ef", // rsp
                "461bbb389771b4c5f98a24881230d53c" // output dll
            ),
            new RecompileInfo(
                "ExternalCSharpCompiler_.dll", // assembly name
                "2f9e4ebd0ad7849d8bd9f05523cb83a1", // asmdef
                "1f7ac2134afaf418fb768769dffcb086", // rsp
                "ed0e47e40f75e438191d703455024b55" // output dll
            ),
        };

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            CompilationPipeline.assemblyCompilationFinished += RequestToRecompileIfNeeded;

            foreach (var info in instance.requestedCompilerInfo.Distinct())
            {
                info.Compile();
            }

            instance.requestedCompilerInfo.Clear();
        }

        private static void RequestToRecompileIfNeeded(string assemblyPath, CompilerMessage[] messages)
        {
            Debug.Log(">>>>" + assemblyPath);
            // Skip: Compile error.
            if (messages.Any(x => x.type == CompilerMessageType.Error)) return;

            // Skip: CompilerInfo is not found.
            var info = s_CompilerInfos.FirstOrDefault(x => x.IsTrigger(assemblyPath));
            if (info == null) return;

            instance.requestedCompilerInfo.Add(info);
        }

        [MenuItem("Csc Settings/Recompile All")]
        private static void RecompileAll()
        {
            foreach (var info in s_CompilerInfos)
            {
                info.Compile();
            }
        }
    }
}
