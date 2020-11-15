using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal struct CompilerInfo
    {
        public string PackageId { get; }
        public string Path { get; }
        public string RuntimePath { get; }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(RuntimePath); }
        }

        private CompilerInfo(string packageId, string path, string runtimePath)
        {
            PackageId = packageId;
            Path = path;
            RuntimePath = runtimePath;
        }

        public static CompilerInfo GetInstalledInfo(string packageId)
        {
            var path = Utils.InstallNugetPackage(packageId);
            if (string.IsNullOrEmpty(path)) return new CompilerInfo(packageId, "", "");

            // DotNet version (Runtime: dotnet)
            foreach (var dll in Directory.GetFiles(path, "csc.dll", SearchOption.AllDirectories))
            {
                var dotnet = RuntimeInfo.GetInstalledDotnetInfo("3.1.8");
                return new CompilerInfo(packageId, dll, dotnet.Path);
            }

            // Net Framework version (Runtime: mono)
            foreach (var dll in Directory.GetFiles(path, "csc.exe", SearchOption.AllDirectories))
            {
                var mono = RuntimeInfo.GetInstalledMonoInfo();
                return new CompilerInfo(packageId, dll, mono.Path);
            }

            return new CompilerInfo(packageId, "", "");
        }

        public void Setup(ProcessStartInfo psi, string responseFile)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
                && Path.EndsWith(".exe"))
            {
                psi.FileName = System.IO.Path.GetFullPath(Path);
                psi.Arguments = "/shared /noconfig @" + responseFile;
            }
            else
            {
                psi.FileName = System.IO.Path.GetFullPath(RuntimePath);
                psi.Arguments = Path + " /noconfig @" + responseFile;
            }
        }
    }
}
