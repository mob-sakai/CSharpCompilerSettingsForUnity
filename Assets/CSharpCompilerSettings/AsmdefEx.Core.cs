#if UNITY_EDITOR && !PUBLISH_AS_DLL
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

namespace Coffee.CSharpCompilierSettings
{
    internal static class ReflectionExtensions
    {
        const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static object Inst(this object self)
        {
            return (self is Type) ? null : self;
        }

        private static Type Type(this object self)
        {
            return (self as Type) ?? self.GetType();
        }

        public static object New(this Type self, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetConstructor(types)
                .Invoke(args);
        }

        public static object Call(this object self, string methodName, params object[] args)
        {
            var types = args.Select(x => x.GetType()).ToArray();
            return self.Type().GetMethod(methodName, types)
                .Invoke(self.Inst(), args);
        }

        public static object Call(this object self, Type[] genericTypes, string methodName, params object[] args)
        {
            return self.Type().GetMethod(methodName, FLAGS)
                .MakeGenericMethod(genericTypes)
                .Invoke(self.Inst(), args);
        }

        public static object Get(this object self, string memberName, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName, FLAGS)[0];
            return mi is PropertyInfo
                ? (mi as PropertyInfo).GetValue(self.Inst(), new object[0])
                : (mi as FieldInfo).GetValue(self.Inst());
        }

        public static void Set(this object self, string memberName, object value, MemberInfo mi = null)
        {
            mi = mi ?? self.Type().GetMember(memberName, FLAGS)[0];
            if (mi is PropertyInfo)
                (mi as PropertyInfo).SetValue(self.Inst(), value, new object[0]);
            else
                (mi as FieldInfo).SetValue(self.Inst(), value);
        }
    }

    internal enum CompilerType
    {
        NetFramework,
        NetCore,
    }

    internal readonly struct CompilerInfo
    {
        public readonly CompilerType Type;
        public readonly string Path;

        public CompilerInfo(CompilerType type, string path)
        {
            Type = type;
            Path = path;
        }
    }

    internal readonly struct CompilerFilter
    {
        public readonly CompilerType Type;
        public readonly string RelatedPath;

        public CompilerFilter(CompilerType type, string relatedPath)
        {
            Type = type;
            RelatedPath = relatedPath;
        }
    }

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
                Core.LogExeption(ex);
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

    internal static class CustomCompiler
    {
        static CompilerInfo? s_compilerInfo;
        static string s_cachedPackageId;

        private static readonly CompilerFilter[] CompilerFilters = new[]
        {
            new CompilerFilter(CompilerType.NetCore, Path.Combine("tasks", "netcoreapp3.1", "bincore", "csc.dll")),
            new CompilerFilter(CompilerType.NetCore, Path.Combine("tasks", "netcoreapp2.1", "bincore", "csc.dll")),
            new CompilerFilter(CompilerType.NetFramework, Path.Combine("tools", "csc.exe")),
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
                Core.LogExeption(ex);
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

                compilerInfo = new CompilerInfo(filter.Type, cscPath);
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

    internal class ShellHelper
    {
        public static void ExecuteCommand(string exe, string args)
        {
            Core.LogInfo("Execute command: {0} {1}", exe, args);

            var p = Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
            });

            // Don't consume 100% of CPU while waiting for process to exit
            if (Application.platform == RuntimePlatform.OSXEditor)
                while (!p.HasExited)
                    Thread.Sleep(100);
            else
                p.WaitForExit();

            if (p.ExitCode != 0)
            {
                var ex = new Exception(p.StandardError.ReadToEnd());
                Core.LogExeption(ex);
                throw ex;
            }
        }
    }

    [InitializeOnLoad]
    internal static class Core
    {
        private const string k_LogHeader = "<b><color=#aa2222>[CscSettings]</color></b> ";
        private static bool LogDebugEnabled { get; }

        public static void LogDebug(string format, params object[] args)
        {
            if (LogDebugEnabled)
                LogInfo(format, args);
        }

        public static void LogInfo(string format, params object[] args)
        {
            if (args == null || args.Length == 0)
                UnityEngine.Debug.Log(k_LogHeader + format);
            else
                UnityEngine.Debug.LogFormat(k_LogHeader + format, args);
        }

        public static void LogExeption(Exception e)
        {
            UnityEngine.Debug.LogException(new Exception(k_LogHeader + e.Message, e.InnerException));
        }

        public static void RequestScriptCompilation()
        {
            Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor")
                .Call("DirtyAllScripts");
        }

        private static void ChangeCompilerProcess(object compiler, object scriptAssembly, CscSettings setting)
        {
            var tProgram = Type.GetType("UnityEditor.Utils.Program, UnityEditor");
            var tScriptCompilerBase = Type.GetType("UnityEditor.Scripting.Compilers.ScriptCompilerBase, UnityEditor");
            var fiProcess = tScriptCompilerBase.GetField("process", BindingFlags.NonPublic | BindingFlags.Instance);
            var psi = compiler.Get("process", fiProcess).Call("GetProcessStartInfo") as ProcessStartInfo;
            LogDebug("current process: {0}", (psi.FileName + " " + psi.Arguments));

            var command = (psi.FileName + " " + psi.Arguments).Replace('\\', '/')
                .Replace(EditorApplication.applicationContentsPath.Replace('\\', '/'), "@APP_CONTENTS@");
            var isDefaultCsc = Regex.IsMatch(command, "@APP_CONTENTS@/[^ ]*(mcs|csc)");

            // csc is not Unity default. It is already modified.
            if (!isDefaultCsc)
            {
                LogDebug("  <color=#bbbb44><Skipped> current csc is not Unity default. It is already modified.</color>");
                return;
            }

            var compilerInfo = CustomCompiler.GetInstalledPath(setting.PackageId);

            // csc is not installed.
            if (!compilerInfo.HasValue)
            {
                LogDebug("  <color=#bbbb44><Skipped> custom csc is not installed.</color>");
                return;
            }

            // Kill current process.
            compiler.Call("Dispose");

            var responseFile = Regex.Replace(psi.Arguments, "^.*@(.+)$", "$1");
            var text = File.ReadAllText(responseFile);
            text = Regex.Replace(text, "[\r\n]+", "\n");
            text = Regex.Replace(text, "^-", "/");
            text = Regex.Replace(text, "\n/langversion:[^\n]+\n", "\n/langversion:" + setting.LanguageVersion + "\n");
            text = Regex.Replace(text, "\n/debug\n", "\n/debug:portable\n");
            text += "\n/preferreduilang:en-US";

            // Change exe file path.
            LogDebug("Change csc to {0}", compilerInfo);
            if (compilerInfo.Value.Type == CompilerType.NetFramework)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    psi.FileName = Path.GetFullPath(compilerInfo.Value.Path);
                    psi.Arguments = "/shared /noconfig @" + responseFile;
                }
                else
                {
                    psi.FileName = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono");
                    psi.Arguments = compilerInfo.Value.Path + " /noconfig @" + responseFile;
                }
            }
            else
            {
                psi.FileName = Path.GetFullPath(DotnetRuntime.GetInstalledPath());
                psi.Arguments = compilerInfo.Value.Path + " /noconfig @" + responseFile;
            }

            text = Regex.Replace(text, "\n", Environment.NewLine);
            File.WriteAllText(responseFile, text);

            LogDebug("Restart compiler process: {0} {1}", psi.FileName, psi.Arguments);
            var program = tProgram.New(psi);
            program.Call("Start");
            compiler.Set("process", program, fiProcess);
        }

        private static void OnAssemblyCompilationStarted(string name)
        {
            try
            {
                LogDebug("Assembly compilation started: {0}", name);
                if (Path.GetFileNameWithoutExtension(name) == "CSharpCompilerSettings")
                {
                    LogDebug("  <color=#bbbb44><Skipped> Assembly 'CSharpCompilerSettings' requires default csc.</color>");
                    return;
                }

                var settings = CscSettings.instance;
                if (settings.UseDefaultCompiler)
                    return;

                var assemblyName = Path.GetFileNameWithoutExtension(name);
                if (assemblyName == typeof(Core).Assembly.GetName().Name)
                    return;

                var tEditorCompilationInterface = Type.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface, UnityEditor");
                var compilerTasks = tEditorCompilationInterface.Get("Instance").Get("compilationTask").Get("compilerTasks") as IDictionary;
                var scriptAssembly = compilerTasks.Keys.Cast<object>().FirstOrDefault(x => (x.Get("Filename") as string) == assemblyName + ".dll");
                if (scriptAssembly == null)
                    return;

                // Create new compiler to recompile.
                LogDebug("Assembly compilation started: <b>{0} should be recompiled.</b>", assemblyName);
                ChangeCompilerProcess(compilerTasks[scriptAssembly], scriptAssembly, settings);
            }
            catch (Exception e)
            {
                LogExeption(e);
            }
        }

        static Core()
        {
            LogDebugEnabled = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';', ',')
                .Any(x => x == "CSC_SETTINGS_DEBUG");

            if (LogDebugEnabled)
            {
                var sb = new StringBuilder("<b>InitializeOnLoad</b>. Loaded assemblies:\n");
                foreach (var asm in Type.GetType("UnityEditor.EditorAssemblies, UnityEditor").Get("loadedAssemblies") as Assembly[])
                {
                    var name = asm.GetName().Name;
                    var path = asm.Location;
                    if (path.Contains(Path.GetDirectoryName(EditorApplication.applicationPath)))
                        sb.AppendFormat("  > {0}:\t{1}\n", name, "APP_PATH/.../" + Path.GetFileName(path));
                    else
                        sb.AppendFormat("  > <color=#22aa22><b>{0}</b></color>:\t{1}\n", name, path.Replace(Environment.CurrentDirectory, "."));
                }

                LogDebug(sb.ToString());
            }

            var assembly = typeof(Core).Assembly;
            var assemblyName = assembly.GetName().Name;
            var location = assembly.Location.Replace(Environment.CurrentDirectory, ".");
            LogInfo("Start watching assembly compilation: assembly = {0} ({1})", assemblyName, location);
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        }
    }
}
#endif
