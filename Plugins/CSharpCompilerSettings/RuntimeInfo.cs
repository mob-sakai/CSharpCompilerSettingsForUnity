using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal struct RuntimeInfo
    {
        public string PackageId { get; }
        public string Path { get; }

        private RuntimeInfo(string packageId, string path)
        {
            PackageId = packageId;
            Path = path;
        }

        public static RuntimeInfo GetInstalledDotnetInfo(string version)
        {
            var packageId = "dotnet-runtime-" + version;
            var url = GetDotnetDownloadUrl(version);
            var installPath = Utils.InstallPackage(packageId, url);
            if (string.IsNullOrEmpty(installPath)) return new RuntimeInfo(packageId, "");

            var dotnetPath = System.IO.Path.Combine(installPath, Application.platform == RuntimePlatform.WindowsEditor
                ? "dotnet.exe"
                : "dotnet");
            return new RuntimeInfo(packageId, dotnetPath);
        }

        private static string GetDotnetDownloadUrl(string version)
        {
            const string pattern = "https://dotnetcli.azureedge.net/dotnet/Runtime/{0}/dotnet-runtime-{0}-{1}-x64.{2}";

            // todo: replace hardcoded 3.1.8 with maximum available version
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return string.Format(pattern, version, "win", "zip");
                case RuntimePlatform.OSXEditor:
                    return string.Format(pattern, version, "osx", "tar.gz");
                case RuntimePlatform.LinuxEditor:
                    return string.Format(pattern, version, "linux", "tar.gz");
                default:
                    throw new NotSupportedException($"{Application.platform} is not supported");
            }
        }

        public static RuntimeInfo GetInstalledMonoInfo()
        {
            return new RuntimeInfo("mono", Utils.PathCombine(EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono"));
        }
    }
}
