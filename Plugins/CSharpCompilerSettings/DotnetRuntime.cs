using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal static class DotnetRuntime
    {
        static string s_InstallPath;

        public static string GetInstalledPath()
        {
            if (!string.IsNullOrEmpty(s_InstallPath) && File.Exists(s_InstallPath))
                return s_InstallPath;

            try
            {
                s_InstallPath = Install();
            }
            catch (Exception ex)
            {
                Core.LogException(ex);
            }

            return s_InstallPath;
        }

        private static string GetDotnetDownloadUrl()
        {
            const string pattern = "https://dotnetcli.azureedge.net/dotnet/Runtime/{0}/dotnet-runtime-{0}-{1}-x64.{2}";


            // todo: replace hardcoded 3.1.8 with maximum available version
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return string.Format(pattern, "3.1.8", "win", "zip");
                case RuntimePlatform.OSXEditor:
                    return string.Format(pattern, "3.1.8", "osx", "tar.gz");
                case RuntimePlatform.LinuxEditor:
                    return string.Format(pattern, "3.1.8", "linux", "tar.gz");
                default:
                    throw new NotSupportedException($"{Application.platform} is not supported");
            }
        }

        private static string Install()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var sep = Path.DirectorySeparatorChar;
            var url = GetDotnetDownloadUrl();
            var downloadPath = Path.Combine("Temp", Path.GetFileName(Path.GetTempFileName()));
            var installPath = Path.Combine("Library", "DotNetRuntime");
            var dotnetPath = Path.Combine(installPath, Application.platform == RuntimePlatform.WindowsEditor
                ? "dotnet.exe"
                : "dotnet");

            // C# compiler is already installed.
            if (File.Exists(dotnetPath))
            {
                Core.LogDebug($"dotnet runtime is already installed at {dotnetPath}");
                return dotnetPath;
            }

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            var cb = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
            try
            {
                Core.LogInfo("Install dotnet runtime");

                // Download C# compiler package from nuget.
                {
                    Core.LogInfo("Download {0}", url);
                    EditorUtility.DisplayProgressBar("Dotnet Installer", "Downloading", 0.2f);

                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url, downloadPath);
                        }
                    }
                    catch
                    {
                        Core.LogInfo("Download {0} (alternative)", url);
                        switch (Application.platform)
                        {
                            case RuntimePlatform.WindowsEditor:
                                ShellHelper.ExecuteCommand("PowerShell.exe", string.Format("curl -O {0} {1}", downloadPath, url));
                                break;
                            case RuntimePlatform.OSXEditor:
                                ShellHelper.ExecuteCommand("curl", string.Format("-o {0} -L {1}", downloadPath, url));
                                break;
                            case RuntimePlatform.LinuxEditor:
                                ShellHelper.ExecuteCommand("wget", string.Format("-O {0} {1}", downloadPath, url));
                                break;
                        }
                    }
                }

                // Extract nuget package (unzip).
                {
                    Core.LogInfo("Extract {0} to {1} with 7z", downloadPath, installPath);
                    EditorUtility.DisplayProgressBar("Dotnet Installer", string.Format("Extract {0}", downloadPath), 0.4f);
                    ExtractArchive(downloadPath, installPath);
                }

                Core.LogInfo($"Dotnet has been installed at {installPath}.");
            }
            catch
            {
                throw new Exception(string.Format("Dotnet installation failed."));
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = cb;
                EditorUtility.ClearProgressBar();
            }

            if (File.Exists(dotnetPath))
                return dotnetPath;

            throw new FileNotFoundException(string.Format("Dotnet is not found at {0}", dotnetPath));
        }

        private static void ExtractArchive(string archivePath, string extractToPath)
        {
            var appPath = EditorApplication.applicationContentsPath.Replace('/', Path.DirectorySeparatorChar);

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    var args = string.Format("x {0} -o{1}", archivePath, extractToPath);
                    ShellHelper.ExecuteCommand(appPath + "\\Tools\\7z.exe", args);
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    // 7za doesn't preserve permission but tar does
                    Directory.CreateDirectory(extractToPath);
                    args = string.Format("-pzxf {0} -C {1}", archivePath, extractToPath);
                    ShellHelper.ExecuteCommand("tar", args);
                    break;
            }
        }
    }
}
