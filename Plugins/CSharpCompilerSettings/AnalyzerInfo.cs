using System.IO;

namespace Coffee.CSharpCompilerSettings
{
    internal struct AnalyzerInfo
    {
        public string PackageId { get; }
        public string[] DllFiles { get; }

        private AnalyzerInfo(string packageId, string[] dllFiles)
        {
            PackageId = packageId;
            DllFiles = dllFiles;
        }

        public static AnalyzerInfo GetInstalledInfo(string packageId)
        {
            var path = Utils.InstallNugetPackage(packageId);
            if (string.IsNullOrEmpty(path)) return new AnalyzerInfo(packageId, new string[0]);

            var dir = Utils.PathCombine(path, "analyzers", "dotnet", "cs");
            return new AnalyzerInfo(packageId, Directory.GetFiles(dir, "*.dll"));
        }
    }
}
