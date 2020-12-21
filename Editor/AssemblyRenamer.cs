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

            // Create compilation process.
            var psi = new ProcessStartInfo
            {
                Arguments = string.Format("\"{0}\" {1}", dll, assemblyName),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

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
            Logger.LogDebug("Rename: Change assembly name ({0})\n  command={1} {2}\n", assemblyName, psi.FileName, psi.Arguments);
            var p = Process.Start(psi);
            p.Exited += (_, __) =>
            {
                if (p.ExitCode == 0)
                    Logger.LogDebug("Rename: success ({0})\n{1}", assemblyName, p.StandardOutput.ReadToEnd());
                else
                    Logger.LogException("Rename: failure ({0})\n{1}\n\n{2}", assemblyName, p.StandardError.ReadToEnd(), p.StandardOutput.ReadToEnd());
            };
            p.EnableRaisingEvents = true;

            p.WaitForExit();
        }
    }
}
