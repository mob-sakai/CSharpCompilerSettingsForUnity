using System.IO;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;

public class CoreTests
{
    private const string testAsmdefPath = "Packages/com.coffee.csharp-compiler-settings/Tests/CSharpCompilerSettingsTests.asmdef";

    [Test]
    public void GetAssemblyName()
    {
        var asmdefPath = testAsmdefPath;
        var expected = "CSharpCompilerSettingsTests";
        var actual = Core.GetAssemblyName(asmdefPath);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void HasPortableDll()
    {
        var asmdefPath = testAsmdefPath;
        var expected = Path.GetDirectoryName(testAsmdefPath) + "/CSharpCompilerSettings_99ecaef0f755437eac2e947059cbe2d4.dll";
        var actual = Core.GetPortableDllPath(asmdefPath);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void ModifyDefineSymbols_Add()
    {
        var modified = "DDD;EEE";
        var defined = new[] {"AAA", "BBB", "CCC"};
        var expected = new[] {"AAA", "BBB", "CCC", "DDD", "EEE"};
        var actual = Utils.ModifySymbols(defined, modified);
        CollectionAssert.AreEqual(expected, actual);
    }

    [Test]
    public void ModifyDefineSymbols_Remove()
    {
        var modified = "!BBB;!CCC";
        var defined = new[] {"AAA", "BBB", "CCC", "DDD", "EEE"};
        var expected = new[] {"AAA", "DDD", "EEE"};
        var actual = Utils.ModifySymbols(defined, modified);
        CollectionAssert.AreEqual(expected, actual);
    }

    [Test]
    public void ModifyDefineSymbols_AddRemove()
    {
        var modified = "BBB;!CCC;DDD";
        var defined = new[] {"AAA", "BBB", "CCC"};
        var expected = new[] {"AAA", "BBB", "DDD"};
        var actual = Utils.ModifySymbols(defined, modified);
        CollectionAssert.AreEqual(expected, actual);
    }
}
