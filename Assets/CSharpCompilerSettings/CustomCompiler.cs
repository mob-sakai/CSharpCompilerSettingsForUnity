using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal static class CustomCompiler
    {
        static CompilerInfo? s_compilerInfo;
        static string s_cachedPackageId;

        private static string PathCombine(params string[] components)
        {
            string path = components[0];
            for (int i = 1; i < components.Length; i++)
                path = Path.Combine(path, components[i]);
            return path;
        }

        private static readonly CompilerFilter[] CompilerFilters = new[]
        {
            new CompilerFilter(CompilerRuntime.NetCore, PathCombine("tasks", "netcoreapp3.1", "bincore", "csc.dll")),
            new CompilerFilter(CompilerRuntime.NetCore, PathCombine("tasks", "netcoreapp2.1", "bincore", "csc.dll")),
            new CompilerFilter(CompilerRuntime.NetFramework, PathCombine("tools", "csc.exe")),
        };

        public static CompilerInfo? GetInstalledPath(string packageId)
        {
            if (s_compilerInfo != null && File.Exists(s_compilerInfo.Value.Path) && s_cachedPackageId == packageId)
                return s_compilerInfo;

            try
            {
                s_compilerInfo = Install(packageId);
            }
            catch (Exception ex)
            {
                Core.LogException(ex);
            }

            s_cachedPackageId = packageId;
            return s_compilerInfo;
        }

        private static bool TryFindCompilerInPackageFolder(string packagePath, out CompilerInfo compilerInfo)
        {
            foreach (var filter in CompilerFilters)
            {
                var cscPath = Path.Combine(packagePath, filter.RelatedPath);
                if (!File.Exists(cscPath))
                    continue;

                compilerInfo = new CompilerInfo(filter.Runtime, cscPath);
                return true;
            }

            compilerInfo = default(CompilerInfo);
            return false;
        }

        private static CompilerInfo Install(string packageId)
        {
            var sep = Path.DirectorySeparatorChar;
            var url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
            var downloadPath = ("Temp/" + Path.GetFileName(Path.GetTempFileName())).Replace('/', sep);
            var installPath = ("Library/" + packageId).Replace('/', sep);

            // C# compiler is already installed.
            if (TryFindCompilerInPackageFolder(installPath, out var compilerInfo))
            {
                Core.LogDebug("C# compiler '{0}' is already installed at {1}", packageId, compilerInfo.Path);
                return compilerInfo;
            }

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            var cb = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
            try
            {
                Core.LogInfo("Install C# compiler '{0}'", packageId);

                // Download C# compiler package from nuget.
                {
                    Core.LogInfo("Download {0} from nuget: {1}", packageId, url);
                    EditorUtility.DisplayProgressBar("C# Compiler Installer", string.Format("Download {0} from nuget", packageId), 0.2f);

                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(url, downloadPath);
                        }
                    }
                    catch
                    {
                        Core.LogInfo("Download {0} from nuget (alternative): {1}", packageId, url);
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
                    EditorUtility.DisplayProgressBar("C# Compiler Installer", string.Format("Extract {0}", downloadPath), 0.4f);

                    var appPath = EditorApplication.applicationContentsPath.Replace('/', sep);
                    var args = string.Format("x {0} -o{1}", downloadPath, installPath);

                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsEditor:
                            ShellHelper.ExecuteCommand(appPath + "\\Tools\\7z.exe", args);
                            break;
                        case RuntimePlatform.OSXEditor:
                        case RuntimePlatform.LinuxEditor:
                            ShellHelper.ExecuteCommand(appPath + "/Tools/7za", args);
                            break;
                    }
                }

                Core.LogInfo("C# compiler '{0}' has been installed at {1}.", packageId, installPath);
            }
            catch
            {
                throw new Exception(string.Format("C# compiler '{0}' installation failed.", packageId));
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = cb;
                EditorUtility.ClearProgressBar();
            }

            if (TryFindCompilerInPackageFolder(installPath, out compilerInfo))
                return compilerInfo;

            throw new FileNotFoundException(string.Format("C# compiler '{0}' is not found in {1}", packageId, installPath));
        }
    }
}
