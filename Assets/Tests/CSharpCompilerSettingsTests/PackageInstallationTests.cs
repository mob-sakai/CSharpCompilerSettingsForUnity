using System.Diagnostics;
using System.IO;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PackageInstallationTests
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

    [Test]
    public void Install_OpenSesameNetCompilerToolset350_Setup()
    {
        var packageId = "OpenSesame.Net.Compilers.Toolset.3.5.0";
        var info = CompilerInfo.GetInstalledInfo(packageId);
        var psi = new ProcessStartInfo("", "");
        info.Setup(psi, "rspFile");
        var dotnetExe = Application.platform == RuntimePlatform.WindowsEditor ? "dotnet.exe" : "dotnet";
        var expectedFilename = Path.GetFullPath(Utils.PathCombine("Library", "InstalledPackages", "dotnet-runtime-3.1.8", dotnetExe));
        var dllPath = Utils.PathCombine("Library", "InstalledPackages", packageId, "tasks", "netcoreapp3.1", "bincore", "csc.dll");

        var expectedArguments = dllPath + " /noconfig @rspFile";

        Assert.AreEqual(expectedFilename, psi.FileName);
        Assert.AreEqual(expectedArguments, psi.Arguments);
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
}
