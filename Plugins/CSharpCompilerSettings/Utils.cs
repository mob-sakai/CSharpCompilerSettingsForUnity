using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal static class Utils
    {
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
            var editorCompilation = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor")
                .Get("Instance");

            if (string.IsNullOrEmpty(assemblyName))
            {
                editorCompilation.Call("DirtyAllScripts");
                return;
            }

            var allScripts = editorCompilation.Get("allScripts") as Dictionary<string, string>;
            var assemblyFilename = assemblyName + ".dll";
            var path = allScripts.FirstOrDefault(x => x.Value == assemblyFilename).Key;
            if (string.IsNullOrEmpty(path)) return;

            editorCompilation.Call("DirtyScript", path, assemblyFilename);
        }

        /// <summary>
        /// Combine the paths.
        /// </summary>
        public static void ClearCache()
        {
            if (Directory.Exists("Library/InstalledPackages"))
                Directory.Delete("Library/InstalledPackages", true);

            if (Directory.Exists("Temp/DownloadedPackages"))
                Directory.Delete("Temp/DownloadedPackages", true);
        }

        /// <summary>
        /// Install NuGet package.
        /// </summary>
        /// <param name="packageId">Package Id</param>
        /// <returns>Installed directory path</returns>
        public static string InstallNugetPackage(string packageId)
        {
            Regex.IsMatch(packageId, @".*\.\d+\.\d+\.*");

            var url = "https://globalcdn.nuget.org/packages/" + packageId.ToLower() + ".nupkg";
            return InstallPackage(packageId, url);
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

                Logger.LogInfo("Download {0} (alternative)", url);

                // NOTE: In .Net Framework 3.5, TSL1.2 is not supported.
                // So, download the file on command line instead.
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                        ExecuteCommand("PowerShell.exe", string.Format("curl -O {0} {1}", downloadPath, url));
                        break;
                    case RuntimePlatform.OSXEditor:
                        ExecuteCommand("curl", string.Format("-o {0} -L {1}", downloadPath, url));
                        break;
                    case RuntimePlatform.LinuxEditor:
                        ExecuteCommand("wget", string.Format("-O {0} {1}", downloadPath, url));
                        break;
                }
            }

            return downloadPath;
        }

        /// <summary>
        /// Extract archive file.
        /// </summary>
        /// <param name="archivePath">Archive file path</param>
        /// <param name="extractTo">Extraction path</param>
        public static void ExtractArchive(string archivePath, string extractTo)
        {
            Logger.LogInfo("Extract archive {0} to {1}", archivePath, extractTo);
            var contentsPath = EditorApplication.applicationContentsPath;
            var args = string.Format("x {0} -o{1}", archivePath, extractTo);

            Directory.CreateDirectory(Path.GetDirectoryName(extractTo));

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    ExecuteCommand(PathCombine(contentsPath, "Tools", "7z.exe"), args);
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    if (archivePath.EndsWith("tar.gz"))
                    {
                        // 7za doesn't preserve permission but tar does
                        Directory.CreateDirectory(extractTo);
                        ExecuteCommand("tar", string.Format("-pzxf {0} -C {1}", archivePath, extractTo));
                    }
                    else
                        ExecuteCommand(PathCombine(contentsPath, "Tools", "7za"), args);

                    break;
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
