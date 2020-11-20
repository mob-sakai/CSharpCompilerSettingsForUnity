using System.IO;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;

public class CoreTests
{
  private const string TestAsmdefPath = "Assets/Tests/CSharpCompilerSettingsTests/CSharpCompilerSettingsTests.asmdef";

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
