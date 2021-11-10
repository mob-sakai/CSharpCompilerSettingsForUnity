using System;
using System.Diagnostics;
using System.IO;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

internal class PackageInstallationTests
{
    [Test]
    public void Install_MicrosoftNetCompilers3x0()
    {
        var packageId = "Microsoft.Net.Compilers.3.x.0";
        LogAssert.ignoreFailingMessages = true;
        Assert.That(() => CompilerInfo.GetInstalledInfo(packageId), Throws.TypeOf<System.Exception>());
    }

    [Test]
    public void Install_MicrosoftNetCompilers350()
    {
        var packageId = "Microsoft.Net.Compilers.3.5.0";
        var expected = Utils.PathCombine("Library", "InstalledPackages", packageId, "tools", "csc.exe");
        var info = CompilerInfo.GetInstalledInfo(packageId);
        var runtime = Utils.PathCombine(EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono");
        Assert.AreEqual(expected, info.Path);
        Assert.AreEqual(packageId, info.PackageId);
        Assert.AreEqual(true, info.IsValid);
        Assert.AreEqual(runtime, info.RuntimePath);
    }

    [Test]
    public void Install_MicrosoftNetCompilersToolset350()
    {
        var packageId = "Microsoft.Net.Compilers.Toolset.3.5.0";
        var expected = Utils.PathCombine("Library", "InstalledPackages", packageId, "tasks", "netcoreapp3.1", "bincore", "csc.dll");
        var info = CompilerInfo.GetInstalledInfo(packageId);
        var runtime = Utils.PathCombine("Library", "InstalledPackages", "dotnet-runtime-3.1.8", "dotnet");
        Assert.AreEqual(expected, info.Path);
        Assert.AreEqual(packageId, info.PackageId);
        Assert.AreEqual(true, info.IsValid);
        Assert.AreEqual(runtime, info.RuntimePath);
    }

    [Test]
    public void Install_OpenSesameNetCompilers350()
    {
        var packageId = "OpenSesame.Net.Compilers.3.5.0";
        var expected = Utils.PathCombine("Library", "InstalledPackages", packageId, "tools", "csc.exe");
        var info = CompilerInfo.GetInstalledInfo(packageId);
        var runtime = Utils.PathCombine(EditorApplication.applicationContentsPath, "MonoBleedingEdge", "bin", "mono");
        Assert.AreEqual(expected, info.Path);
        Assert.AreEqual(packageId, info.PackageId);
        Assert.AreEqual(true, info.IsValid);
        Assert.AreEqual(runtime, info.RuntimePath);
    }

    [Test]
    public void Install_OpenSesameNetCompilerToolset350()
    {
        var packageId = "OpenSesame.Net.Compilers.Toolset.3.5.0";
        var expected = Utils.PathCombine("Library", "InstalledPackages", packageId, "tasks", "netcoreapp3.1", "bincore", "csc.dll");
        var info = CompilerInfo.GetInstalledInfo(packageId);
        var runtime = Utils.PathCombine("Library", "InstalledPackages", "dotnet-runtime-3.1.8", "dotnet");
        Assert.AreEqual(expected, info.Path);
        Assert.AreEqual(packageId, info.PackageId);
        Assert.AreEqual(true, info.IsValid);
        Assert.AreEqual(runtime, info.RuntimePath);
    }

    [TestCase(RuntimePlatform.WindowsEditor, "Library/InstalledPackages/dotnet", "Library/InstalledPackages/OpenSesame.Net.Compilers.Toolset.3.5.0/tasks/netcoreapp3.1/bincore/csc.dll /noconfig @rspFile")]
    [TestCase(RuntimePlatform.OSXEditor, "Library/InstalledPackages/dotnet", "Library/InstalledPackages/OpenSesame.Net.Compilers.Toolset.3.5.0/tasks/netcoreapp3.1/bincore/csc.dll /noconfig @rspFile")]
    public void Install_OpenSesameNetCompilerToolset350_Setup(RuntimePlatform platform, string expectedFilename, string expectedArguments)
    {
        var packageId = "OpenSesame.Net.Compilers.Toolset.3.5.0";
        var dotnetExe = Application.platform == RuntimePlatform.WindowsEditor
            ? "dotnet.exe"
            : "dotnet";
        var info = new CompilerInfo(packageId, "Library/InstalledPackages/OpenSesame.Net.Compilers.Toolset.3.5.0/tasks/netcoreapp3.1/bincore/csc.dll", "Library/InstalledPackages/" + dotnetExe);
        var psi = new ProcessStartInfo("", "");
        info.Setup(psi, "rspFile", platform);

        Assert.AreEqual(Path.GetFullPath(expectedFilename), psi.FileName);
        Assert.AreEqual(expectedArguments, psi.Arguments);
    }

    [TestCase(RuntimePlatform.WindowsEditor, "Library/InstalledPackages/OpenSesame.Net.Compilers.3.5.0/tools/csc.exe", "/shared /noconfig @rspFile")]
    [TestCase(RuntimePlatform.OSXEditor, "Library/InstalledPackages/mono", "Library/InstalledPackages/OpenSesame.Net.Compilers.3.5.0/tools/csc.exe /noconfig @rspFile")]
    public void Install_OpenSesameNetCompiler350_Setup(RuntimePlatform platform, string expectedFilename, string expectedArguments)
    {
        var packageId = "OpenSesame.Net.Compilers.3.5.0";
        var info = new CompilerInfo(packageId, "Library/InstalledPackages/OpenSesame.Net.Compilers.3.5.0/tools/csc.exe", "Library/InstalledPackages/mono");
        var psi = new ProcessStartInfo("", "");
        info.Setup(psi, "rspFile", platform);

        Assert.AreEqual(Path.GetFullPath(expectedFilename), psi.FileName);
        Assert.AreEqual(expectedArguments, psi.Arguments);
    }

    [TestCase(RuntimePlatform.WindowsEditor, "https://dotnetcli.azureedge.net/dotnet/Runtime/3.1.8/dotnet-runtime-3.1.8-win-x64.zip")]
    [TestCase(RuntimePlatform.OSXEditor, "https://dotnetcli.azureedge.net/dotnet/Runtime/3.1.8/dotnet-runtime-3.1.8-osx-x64.tar.gz")]
    [TestCase(RuntimePlatform.LinuxEditor, "https://dotnetcli.azureedge.net/dotnet/Runtime/3.1.8/dotnet-runtime-3.1.8-linux-x64.tar.gz")]
    [TestCase(RuntimePlatform.Android, null)]
    [TestCase(RuntimePlatform.IPhonePlayer, null)]
    public void Install_DotnetUrl(RuntimePlatform platform, string expected)
    {
        var version = "3.1.8";

        if (string.IsNullOrEmpty(expected))
        {
            Assert.That(() => RuntimeInfo.GetDotnetDownloadUrl(version, platform), Throws.TypeOf<NotSupportedException>());
        }
        else
        {
            var actual = RuntimeInfo.GetDotnetDownloadUrl(version, platform);
            Assert.AreEqual(expected, actual);
        }
    }

    [Test]
    public void Install_Dotnet()
    {
        var version = "3.1.8";
        var packageId = "dotnet-runtime-" + version;
        var dotnetExe = Application.platform == RuntimePlatform.WindowsEditor ? "dotnet.exe" : "dotnet";
        var expected = Utils.PathCombine("Library", "InstalledPackages", packageId, dotnetExe);
        var info = RuntimeInfo.GetInstalledDotnetInfo(version);
        Assert.AreEqual(expected, info.Path);
        Assert.AreEqual(packageId, info.PackageId);
    }

    [Test]
    public void Install_Analyzer()
    {
        var packageId = "ErrorProne.NET.CoreAnalyzers.0.1.2";
        var dir = "Library/InstalledPackages/" + packageId + "/analyzers/dotnet/cs/";
        var expected = new[]
        {
            dir + "ErrorProne.NET.Core.dll",
            dir + "ErrorProne.Net.CoreAnalyzers.dll",
            dir + "RuntimeContracts.dll",
        };
        Utils.UninstallNugetPackage(packageId);

        var info = AnalyzerInfo.GetInstalledInfo(packageId);
        CollectionAssert.AreEqual(expected, info.DllFiles);
        Assert.AreEqual(packageId, info.PackageId);
    }

    [Test]
    public void Install_NugetPackage()
    {
        var name = "StyleCop.Analyzers";
        var version = "1.1.118";
        var packageId = name + "." + version;
        var package = new NugetPackage(name, version, NugetPackage.CategoryType.Analyzer);
        Assert.AreEqual(packageId, package.PackageId);
        Assert.AreEqual(name, package.Name);
        Assert.AreEqual(version, package.Version);
        Assert.AreEqual(true, package.IsValid);
        Assert.AreEqual(NugetPackage.CategoryType.Analyzer, package.Category);
        Assert.AreEqual(packageId, package.ToString());
    }

    [TestCase(RuntimePlatform.WindowsEditor, "Temp/zipfile.zip", "Library/ExtractTo", "${CONTENT_PATH}/Tools/7z.exe", "x Temp/zipfile.zip -oLibrary/ExtractTo")]
    [TestCase(RuntimePlatform.WindowsEditor, "Temp/zipfile.tar.gz", "Library/ExtractTo", "${CONTENT_PATH}/Tools/7z.exe", "x Temp/zipfile.tar.gz -oLibrary/ExtractTo")]
    [TestCase(RuntimePlatform.OSXEditor, "Temp/zipfile.zip", "Library/ExtractTo", "${CONTENT_PATH}/Tools/7za", "x Temp/zipfile.zip -oLibrary/ExtractTo")]
    [TestCase(RuntimePlatform.OSXEditor, "Temp/zipfile.tar.gz", "Library/ExtractTo", "tar", "-pzxf Temp/zipfile.tar.gz -C Library/ExtractTo")]
    [TestCase(RuntimePlatform.LinuxEditor, "Temp/zipfile.zip", "Library/ExtractTo", "${CONTENT_PATH}/Tools/7za", "x Temp/zipfile.zip -oLibrary/ExtractTo")]
    [TestCase(RuntimePlatform.LinuxEditor, "Temp/zipfile.tar.gz", "Library/ExtractTo", "tar", "-pzxf Temp/zipfile.tar.gz -C Library/ExtractTo")]
    [TestCase(RuntimePlatform.Android, "Temp/zipfile.zip", "Library/ExtractTo", null, null)]
    [TestCase(RuntimePlatform.Android, "Temp/zipfile.tar.gz", "Library/ExtractTo", null, null)]
    public void GetExtractArchiveCommand(RuntimePlatform platform, string archivePath, string extractTo, string arg1, string arg2)
    {
        if (string.IsNullOrEmpty(arg1))
        {
            Assert.That(() => Utils.GetExtractArchiveCommand(archivePath, extractTo, platform), Throws.TypeOf<NotSupportedException>());
        }
        else
        {
            var args = Utils.GetExtractArchiveCommand(archivePath, extractTo, platform);
            arg1 = arg1.Replace("${CONTENT_PATH}", EditorApplication.applicationContentsPath);
            CollectionAssert.AreEqual(args, new [] {arg1, arg2});
        }
    }


    [TestCase(RuntimePlatform.WindowsEditor, "https://cdn.com/archive.zip", "Library/DownloadTo", "PowerShell.exe", "curl -O Library/DownloadTo https://cdn.com/archive.zip")]
    [TestCase(RuntimePlatform.OSXEditor, "https://cdn.com/archive.zip", "Library/DownloadTo", "curl", "-o Library/DownloadTo -L https://cdn.com/archive.zip")]
    [TestCase(RuntimePlatform.LinuxEditor, "https://cdn.com/archive.zip", "Library/DownloadTo", "wget", "-O Library/DownloadTo https://cdn.com/archive.zip")]
    [TestCase(RuntimePlatform.Android, "https://cdn.com/archive.zip", "Library/DownloadTo", null, null)]
    public void GetDownloadCommand(RuntimePlatform platform, string url, string downloadPath, string arg1, string arg2)
    {
        if (string.IsNullOrEmpty(arg1))
        {
            Assert.That(() => Utils.GetDownloadCommand(url, downloadPath, platform), Throws.TypeOf<NotSupportedException>());
        }
        else
        {
            var args = Utils.GetDownloadCommand(url, downloadPath, platform);
            arg1 = arg1.Replace("${CONTENT_PATH}", EditorApplication.applicationContentsPath);
            CollectionAssert.AreEqual(args, new [] {arg1, arg2});
        }
    }
}
