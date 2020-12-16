using Coffee.CSharpCompilerSettings;
using NUnit.Framework;

public class CscSettingsAssetTests
{
    private const string TestAsmdefPath = "Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef";

    [TestCase("{}", true)]
    [TestCase("{\"m_UseDefaultCompiler\":false}", false)]
    public void UseDefaultCompiler(string json, bool expected)
    {
        var asset = CscSettingsAsset.CreateFromJson(json);
        Assert.AreEqual(expected, asset.UseDefaultCompiler);
    }

    [TestCase("{}", false)]
    [TestCase("{\"m_UseDefaultCompiler\":false}", true)]
    public void UpgradeVersion(string json, bool expected)
    {
        var asset = CscSettingsAsset.CreateFromJson(json);
        Assert.AreEqual(expected, asset.ShouldToRecompile("Assets/test.asmdef"));
    }

    [Test]
    public void UpgradeVersion_SymbolModifier()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_ModifySymbols\":\"ADD;!REMOVE\"}");
        var expected = new[] {"ADD", "!REMOVE"};
        var actual = asset.GetSymbolModifier("Assets/test.asmdef").Split(';');
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestCase("{\"m_LanguageVersion\":700}", "7")]
    [TestCase("{\"m_LanguageVersion\":701}", "7.1")]
    [TestCase("{\"m_LanguageVersion\":702}", "7.2")]
    [TestCase("{\"m_LanguageVersion\":703}", "7.3")]
    [TestCase("{\"m_LanguageVersion\":800}", "8.0")]
    [TestCase("{\"m_LanguageVersion\":900}", "9.0")]
    [TestCase("{\"m_LanguageVersion\":2147483646}", "preview")]
    [TestCase("{\"m_LanguageVersion\":2147483647}", "latest")]
    public void LanguageVersion(string json, string expected)
    {
        var asset = CscSettingsAsset.CreateFromJson(json);
        Assert.AreEqual(expected, asset.LanguageVersion);
    }

    [TestCase("{\"m_LanguageVersion\":700,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;!CSHARP_7_1_OR_NEWER;!CSHARP_7_2_OR_NEWER;!CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":701,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;!CSHARP_7_2_OR_NEWER;!CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":702,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;!CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":703,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":800,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":900,\"m_UseDefaultCompiler\":false}", "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":2147483646,\"m_UseDefaultCompiler\":false}",
        "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;CSHARP_9_OR_NEWER;")]
    [TestCase("{\"m_LanguageVersion\":2147483647,\"m_UseDefaultCompiler\":false}",
        "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;")]
    public void AdditionalSymbol(string json, string expected)
    {
        var asset = CscSettingsAsset.CreateFromJson(json);
        Assert.AreEqual(expected, asset.GetSymbolModifier("Assets/test.asmdef"));
    }

    [TestCase("Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef", true)]
    [TestCase(null, false)]
    public void GetAtPath(string asmdefPath, bool valid)
    {
        var asset = CscSettingsAsset.GetAtPath(asmdefPath);
        if (valid)
            Assert.IsNotNull(asset);
        else
            Assert.IsNull(asset);
    }

    [TestCase(null, true)]
    [TestCase("Assets/CSharpCompilerSettingsTests.asmdef", false)]
    [TestCase("Packages/CSharpCompilerSettingsTests.asmdef", false)]
    public void AnalyzerFilter_Predefined(string asmdefPath, bool expected)
    {
        var filter = new AssemblyFilter(true, new string[0]);
        Assert.AreEqual(expected, filter.IsValid(asmdefPath));
    }

    [TestCase(null, false)]
    [TestCase("Assets/CSharpCompilerSettingsTests.asmdef", true)]
    [TestCase("Packages/CSharpCompilerSettingsTests.asmdef", false)]
    public void AnalyzerFilter_Filter(string asmdefPath, bool expected)
    {
        var filter = new AssemblyFilter(false, new[] {"Assets/", "!Packages/"});
        var actual = filter.IsValid(asmdefPath);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void ProjectSettings()
    {
        var settings = CscSettingsAsset.instance;
        Assert.AreEqual(true, settings.EnableDebugLog);
        Assert.AreEqual(true, settings.ShouldToUseAnalyzer("Assets/test.asmdef"));
        Assert.AreEqual(Nullable.Disable, settings.Nullable);
        Assert.AreEqual(new NugetPackage("Microsoft.Net.Compilers.Toolset", "3.8.0"), settings.CompilerPackage);
        CollectionAssert.AreEqual(new[] {new NugetPackage("ErrorProne.NET.CoreAnalyzers", "0.1.2", NugetPackage.CategoryType.Analyzer)}, settings.AnalyzerPackages);
    }
}
