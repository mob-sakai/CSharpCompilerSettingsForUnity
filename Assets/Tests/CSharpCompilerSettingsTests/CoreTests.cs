using System.IO;
using System.Text.RegularExpressions;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = Coffee.CSharpCompilerSettings.Logger;

public class CoreTests : IPrebuildSetup, IPostBuildCleanup
{
    private const string TestAsmdefPath = "Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef";

    public void Setup()
    {
        Logger.Setup("[TEST] ", () => true);
        Utils.DisableCompilation = true;
    }

    public void Cleanup()
    {
        Utils.DisableCompilation = false;
    }

    [Test]
    public void GetAssemblyName()
    {
        var asmdefPath = TestAsmdefPath;
        var expected = "CSharpCompilerSettingsTests";
        var actual = Core.GetAssemblyName(asmdefPath);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void HasPortableDll()
    {
        var asmdefPath = TestAsmdefPath;
        var expected = Path.GetDirectoryName(TestAsmdefPath) + "/CSharpCompilerSettings_271b3c708daf4cf4ab4e8c9ffd124a72.dll";
        var actual = Core.GetPortableDllPath(asmdefPath);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void DirtyScriptsIfNeeded()
    {
        Core.DirtyScriptsIfNeeded();
    }

    [Test]
    public void Initialize()
    {
        Core.Initialize();
    }

    [TestCase("Temp/CSharpCompilerSettings_.dll", "Assembly <b>'CSharpCompilerSettings_'</b> requires default csc.")]
    public void OnAssemblyCompilationStarted(string outpath, string log)
    {
        LogAssert.Expect(LogType.Warning, new Regex(log));
        Core.OnAssemblyCompilationStarted(outpath);
    }

    [TestCase("", false)]
    [TestCase("Packages/com.coffee.csharp-compiler-settings/Plugins/CSharpCompilerSettings/CSharpCompilerSettings.asmdef", false)]
    public void IsInSameDirectory(string path, bool expected)
    {
        var actual = Core.IsInSameDirectory(path);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetSettings()
    {
        var settings = Core.GetSettings();
        Assert.AreEqual(true, settings.EnableDebugLog);
        Assert.AreEqual(true, settings.ShouldToRecompileToAnalyze(null));
    }

    [Test]
    public void ModifyResponseFile()
    {
        var responseFile = "Assets/Tests/CSharpCompilerSettingsTests/responseFile.txt";
        var assemblyName = "CSharpCompilerSettingsTests";
        var asmdefPath = "Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef";
        var text = File.ReadAllText(responseFile);
        text = Core.ModifyResponseFile(CscSettingsAsset.GetAtPath(asmdefPath), assemblyName, asmdefPath, text);

        StringAssert.Contains("/define:ADD_SYMBOL", text);
        StringAssert.Contains("/nullable:disable", text);
        StringAssert.Contains("/analyzer:\"Library/InstalledPackages/ErrorProne.NET.CoreAnalyzers.0.1.2/analyzers/dotnet/cs/ErrorProne.NET.Core.dll", text);
        StringAssert.Contains("/ruleset:\"Assets/Default.ruleset\"", text);
    }

    [TestCase("AAA;BBB;CCC", "DDD;EEE", "AAA;BBB;CCC;DDD;EEE")]
    [TestCase("AAA;BBB;CCC;DDD;EEE", "!BBB;!CCC", "AAA;DDD;EEE")]
    [TestCase("AAA;BBB;CCC", "BBB;!CCC;DDD", "AAA;BBB;DDD")]
    public void ModifyDefineSymbols(string defined, string modified, string expected)
    {
        var actual = Utils.ModifySymbols(defined.Split(';'), modified);
        CollectionAssert.AreEqual(expected.Split(';'), actual);
    }
}
