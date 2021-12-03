using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal static class Utils
    {
        public static bool DisableCompilation { get; set; }

        /// <summary>
        /// Combine the paths.
        /// </summary>
        public static string PathCombine(params string[] components)
        {
            return components.Aggregate(Path.Combine)
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Modify symbols
        /// </summary>
        public static string[] ModifySymbols(IEnumerable<string> defines, string modifySymbols)
        {
            var symbols = modifySymbols.Split(';', ',');
            var add = symbols.Where(x => 0 < x.Length && !x.StartsWith("!"));
            var remove = symbols.Where(x => 1 < x.Length && x.StartsWith("!")).Select(x => x.Substring(1));
            return defines
                .Union(add)
                .Except(remove)
                .Distinct()
                .ToArray();
        }

        public static void RequestCompilation(string assemblyName = null)
        {
            var unityVersions = Application.unityVersion.Split('.');
            if (2021 <= int.Parse(unityVersions[0]))
            {
                typeof(CompilationPipeline).Call("RequestScriptCompilation");
                return;
            }

            var editorCompilation = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor")
                .Get("Instance");

            if (string.IsNullOrEmpty(assemblyName))
            {
                if (DisableCompilation)
                    Logger.LogWarning("Skipped: Disable compilation");
                else
                    editorCompilation.Call("DirtyAllScripts");
                return;
            }

            var allScripts = editorCompilation.Get("allScripts") as Dictionary<string, string>;
            if (allScripts == null) return;

            var assemblyFilename = assemblyName + ".dll";
            var path = allScripts.FirstOrDefault(x => x.Value == assemblyFilename).Key;
            if (string.IsNullOrEmpty(path)) return;

            if (DisableCompilation)
                Logger.LogWarning("Skipped: Disable compilation");
            else
                editorCompilation.Call("DirtyScript", path, assemblyFilename);
        }

        /// <summary>
        /// Install NuGet package.
        /// </summary>
        /// <param name="packageId">Package Id</param>
        /// <returns>Installed directory path</returns>
        public static string InstallNugetPackage(string packageId)
        {
            var url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
            return InstallPackage(packageId, url);
        }

        /// <summary>
        /// Uninstall NuGet package.
        /// </summary>
        /// <param name="packageId">Package Id</param>
        public static void UninstallNugetPackage(string packageId)
        {
            var installPath = PathCombine("Library", "InstalledPackages", packageId);
            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);
        }

        /// <summary>
        /// Install package from url.
        /// </summary>
        /// <param name="packageId">Package Id</param>
        /// <param name="url">Package url</param>
        /// <returns>Installed directory path</returns>
        public static string InstallPackage(string packageId, string url)
        {
            var installPath = PathCombine("Library", "InstalledPackages", packageId);
            if (Directory.Exists(installPath))
                return installPath;

            try
            {
                Logger.LogInfo("Install package: {0}", packageId);
                EditorUtility.DisplayProgressBar("Package Installer", string.Format("Download {0} from {1}", packageId, url), 0.5f);
                var downloadPath = DownloadFile(url);
                EditorUtility.DisplayProgressBar("Package Installer", string.Format("Extract to {0}", installPath), 0.7f);
                ExtractArchive(downloadPath, installPath);

                Logger.LogInfo("Package '{0}' has been installed at {1}.", packageId, installPath);
            }
            catch
            {
                throw new Exception(string.Format("Package '{0}' installation failed.", packageId));
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (Directory.Exists(installPath))
                return installPath;

            throw new FileNotFoundException(string.Format("Package '{0}' is not found at {1}", packageId, installPath));
        }

        /// <summary>
        /// Download the file by specifying the URL.
        /// NOTE: In .Net Framework 3.5, TSL1.2 is not supported. So, download the file on command line instead.
        /// </summary>
        /// <param name="url">File url</param>
        /// <returns>Downloaded file path.</returns>
        public static string DownloadFile(string url)
        {
            var downloadPath = PathCombine("Temp", "DownloadedPackages", Path.GetFileName(url));

            // Clear cache.
            if (File.Exists(downloadPath))
                File.Delete(downloadPath);

            var cb = ServicePointManager.ServerCertificateValidationCallback;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(downloadPath));

                Logger.LogInfo("Download {0}", url);
                ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, downloadPath);
                }

                ServicePointManager.ServerCertificateValidationCallback = cb;
            }
            catch
            {
                ServicePointManager.ServerCertificateValidationCallback = cb;

                // NOTE: In .Net Framework 3.5, TSL1.2 is not supported.
                // So, download the file on command line instead.
                Logger.LogInfo("Download {0} (alternative)", url);
                var args = GetDownloadCommand(url, downloadPath, Application.platform);
                ExecuteCommand(args[0], args[1]);
            }

            return downloadPath;
        }

        public static string[] GetDownloadCommand(string url, string downloadPath, RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return new[] { "PowerShell.exe", string.Format("curl -O {0} {1}", downloadPath, url) };
                case RuntimePlatform.OSXEditor:
                    return new[] { "curl", string.Format("-o {0} -L {1}", downloadPath, url) };
                case RuntimePlatform.LinuxEditor:
                    return new[] { "wget", string.Format("-O {0} {1}", downloadPath, url) };

                default:
                    throw new NotSupportedException($"{Application.platform} is not supported");
            }
        }

        // Extract archive file.
        public static void ExtractArchive(string archivePath, string extractTo)
        {
            Logger.LogInfo("Extract archive {0} to {1}", archivePath, extractTo);
            var args = GetExtractArchiveCommand(archivePath, extractTo, Application.platform);
            ExecuteCommand(args[0], args[1]);
        }

        public static string[] GetExtractArchiveCommand(string archivePath, string extractTo, RuntimePlatform platform)
        {
            var contentsPath = EditorApplication.applicationContentsPath;
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                    Directory.CreateDirectory(Path.GetDirectoryName(extractTo));
                    return new[] { PathCombine(contentsPath, "Tools", "7z.exe"), string.Format("x {0} -o{1}", archivePath, extractTo) };
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    if (archivePath.EndsWith("tar.gz"))
                    {
                        Directory.CreateDirectory(extractTo);
                        return new[] { "tar", string.Format("-pzxf {0} -C {1}", archivePath, extractTo) };
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(extractTo));
                        return new[] { PathCombine(contentsPath, "Tools", "7za"), string.Format("x {0} -o{1}", archivePath, extractTo) };
                    }
                default:
                    throw new NotSupportedException($"{Application.platform} is not supported");
            }

        }

        /// <summary>
        /// Execute command.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="arguments">Arguments</param>
        private static void ExecuteCommand(string filename, string arguments)
        {
            Logger.LogInfo("Execute command: {0} {1}", filename, arguments);

            var p = Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            });

            // Don't consume 100% of CPU while waiting for process to exit
            if (Application.platform == RuntimePlatform.OSXEditor)
                while (!p.HasExited)
                    Thread.Sleep(100);
            else
                p.WaitForExit();

            if (p.ExitCode != 0)
            {
                var ex = new Exception(p.StandardError.ReadToEnd() + "\n\n" + p.StandardOutput.ReadToEnd());
                Logger.LogException(ex);
                throw ex;
            }
        }
    }
}
