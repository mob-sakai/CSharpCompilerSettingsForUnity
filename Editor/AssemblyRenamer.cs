using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class AssemblyRenamer
    {
        public static void Rename(string dll, string assemblyName)
        {
            var exe = Path.GetFullPath("Packages/com.coffee.csharp-compiler-settings/ChangeAssemblyName~/ChangeAssemblyName.exe");
            var cecilDll = Path.GetFullPath("Packages/com.coffee.csharp-compiler-settings/ChangeAssemblyName~/Unity.Cecil.dll");
            var contentsPath = EditorApplication.applicationContentsPath;
            var sep = Path.DirectorySeparatorChar;

            // Create compilation process.
            var psi = new ProcessStartInfo
            {
                Arguments = string.Format("\"{0}\" {1}", dll, assemblyName),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            if (!File.Exists(cecilDll))
            {
                File.Copy((contentsPath + "/Managed/Unity.Cecil.dll").Replace('/', sep), cecilDll.Replace('/', sep));
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                psi.FileName = exe;
            }
            else
            {
                psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono");
                psi.Arguments = exe + " " + psi.Arguments;
            }

            // Start compilation process.
            Core.LogDebug("Rename: Change assembly name\n  command={0} {1}\n", psi.FileName, psi.Arguments);
            var p = Process.Start(psi);
            p.Exited += (_, __) =>
            {
                if (p.ExitCode == 0)
                    Core.LogDebug("Rename: success.\n" + p.StandardOutput.ReadToEnd());
                else
                    Core.LogException("Rename: failure.\n" + p.StandardError.ReadToEnd());
            };
            p.EnableRaisingEvents = true;

            p.WaitForExit();
        }
    }
}
