using Coffee.CSharpCompilerSettings;
using NUnit.Framework;

public class CscSettingsAssetTests
{
    private const string TestAsmdefPath = "Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef";

    [Test]
    public void UpgradeVersion_BuiltIn()
    {
        var asset = CscSettingsAsset.CreateFromJson("{}");
        var expected = false;
        var actual = asset.ShouldToRecompile;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void UpgradeVersion_CustomCompiler()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_UseDefaultCompiler\":false}");
        var expected = true;
        var actual = asset.ShouldToRecompile;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void UpgradeVersion_SymbolModifier()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_ModifySymbols\":\"ADD;!REMOVE\"}");
        var expected = new[] {"ADD", "!REMOVE"};
        var actual = asset.AdditionalSymbols.Split(';');
        CollectionAssert.AreEqual(expected, actual);
    }

    [Test]
    public void LanguageVersion_73()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":703}");
        var expected = "7.3";
        var actual = asset.LanguageVersion;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void LanguageVersion_80()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":800}");
        var expected = "8.0";
        var actual = asset.LanguageVersion;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void LanguageVersion_Latest()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":2147483647}");
        var expected = "latest";
        var actual = asset.LanguageVersion;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void AdditionalSymbol_73()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":703,\"m_UseDefaultCompiler\":false}");
        var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
        var actual = asset.AdditionalSymbols;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void AdditionalSymbol_80()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":800,\"m_UseDefaultCompiler\":false}");
        var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
        var actual = asset.AdditionalSymbols;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void AdditionalSymbol_Latest()
    {
        var asset = CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":2147483647,\"m_UseDefaultCompiler\":false}");
        var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
        var actual = asset.AdditionalSymbols;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GetAtPath()
    {
        var asset = CscSettingsAsset.GetAtPath(TestAsmdefPath);
        var expected = true;
        var actual = asset.ShouldToRecompile;
        Assert.AreEqual(expected, actual);
    }
}
