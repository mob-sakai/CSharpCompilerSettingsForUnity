using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace CSharpCompilerSettings.Tests
{
    public class Core
    {
        [Test]
        public void GetAssemblyName()
        {
            var asmdefPath = "Assets/Tests/CSharpCompilerSettings.Test/CSharpCompilerSettings.Test.asmdef";
            var expected = "CSharpCompilerSettings.Test";
            var actual = Coffee.CSharpCompilerSettings.Core.GetAssemblyName(asmdefPath);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void HasPortableDll_False()
        {
            var asmdefPath = "Assets/Tests/CSharpCompilerSettings.Test/CSharpCompilerSettings.Test.asmdef";
            var expected = false;
            var actual = Coffee.CSharpCompilerSettings.Core.HasPortableDll(asmdefPath);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void HasPortableDll_True()
        {
            var asmdefPath = "Assets/Tests/ModifySymbols/ModifySymbols.Tests.asmdef";
            var expected = true;
            var actual = Coffee.CSharpCompilerSettings.Core.HasPortableDll(asmdefPath);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void ModifyDefineSymbols_Add()
        {
            var modified = "DDD;EEE";
            var defined = new[] {"AAA", "BBB", "CCC"};
            var expected = new[] {"AAA", "BBB", "CCC", "DDD", "EEE"};
            var actual = Coffee.CSharpCompilerSettings.Core.ModifyDefineSymbols(defined, modified);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ModifyDefineSymbols_Remove()
        {
            var modified = "!BBB;!CCC";
            var defined = new[] {"AAA", "BBB", "CCC", "DDD", "EEE"};
            var expected = new[] {"AAA", "DDD", "EEE"};
            var actual = Coffee.CSharpCompilerSettings.Core.ModifyDefineSymbols(defined, modified);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ModifyDefineSymbols_AddRemove()
        {
            var modified = "BBB;!CCC;DDD";
            var defined = new[] {"AAA", "BBB", "CCC"};
            var expected = new[] {"AAA", "BBB", "DDD"};
            var actual = Coffee.CSharpCompilerSettings.Core.ModifyDefineSymbols(defined, modified);
            CollectionAssert.AreEqual(expected, actual);
        }
    }

    public class CscSettingsAsset
    {
        [Test]
        public void UpgradeVersion_BuiltIn()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{}");
            var expected = false;
            var actual = asset.ShouldToRecompile;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpgradeVersion_CustomCompiler()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_UseDefaultCompiler\":false}");
            var expected = true;
            var actual = asset.ShouldToRecompile;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LanguageVersion_73()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":703}");
            var expected = "7.3";
            var actual = asset.LanguageVersion;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LanguageVersion_80()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":800}");
            var expected = "8.0";
            var actual = asset.LanguageVersion;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LanguageVersion_Latest()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":2147483647}");
            var expected = "latest";
            var actual = asset.LanguageVersion;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AdditionalSymbol_73()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":703,\"m_UseDefaultCompiler\":false}");
            var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;!CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
            var actual = asset.AdditionalSymbols;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AdditionalSymbol_80()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":800,\"m_UseDefaultCompiler\":false}");
            var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
            var actual = asset.AdditionalSymbols;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AdditionalSymbol_Latest()
        {
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.CreateFromJson("{\"m_LanguageVersion\":2147483647,\"m_UseDefaultCompiler\":false}");
            var expected = "CSHARP_7_OR_NEWER;CSHARP_7_1_OR_NEWER;CSHARP_7_2_OR_NEWER;CSHARP_7_3_OR_NEWER;CSHARP_8_OR_NEWER;!CSHARP_9_OR_NEWER;";
            var actual = asset.AdditionalSymbols;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAtPath()
        {
            var asmdefPath = "Assets/Tests/ModifySymbols/ModifySymbols.Tests.asmdef";
            var asset = Coffee.CSharpCompilerSettings.CscSettingsAsset.GetAtPath(asmdefPath);
            var expected = true;
            var actual = asset.ShouldToRecompile;
            Assert.AreEqual(expected, actual);
        }
    }

    public class DotnetRuntime
    {
        [Test]
        public void Install()
        {
            var installPath = Path.Combine("Library", "DotNetRuntime");
            var dotnetPath = Path.Combine(installPath, Application.platform == RuntimePlatform.WindowsEditor
                ? "dotnet.exe"
                : "dotnet");

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            installPath = Coffee.CSharpCompilerSettings.DotnetRuntime.GetInstalledPath();

            Assert.AreEqual(installPath, dotnetPath);
        }
    }

    public class CustomCompiler
    {
        [Test]
        public void Install_MicrosoftNetCompilers350()
        {
            var packageId = "Microsoft.Net.Compilers.3.5.0";
            var expected = PathCombine("Library", packageId, "tools", "csc.exe");
            var actual = InstallPackageTest(packageId);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Install_MicrosoftNetCompilersToolset350()
        {
            var packageId = "Microsoft.Net.Compilers.Toolset.3.5.0";
            var expected = PathCombine("Library", packageId, "tasks", "netcoreapp3.1", "bincore", "csc.dll");
            var actual = InstallPackageTest(packageId);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Install_OpenSesameNetCompilers350()
        {
            var packageId = "OpenSesame.Net.Compilers.3.5.0";
            var expected = PathCombine("Library", packageId, "tools", "csc.exe");
            var actual = InstallPackageTest(packageId);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Install_OpenSesameNetCompilerToolset350()
        {
            var packageId = "OpenSesame.Net.Compilers.Toolset.3.5.0";
            var expected = PathCombine("Library", packageId, "tasks", "netcoreapp3.1", "bincore", "csc.dll");
            var actual = InstallPackageTest(packageId);
            Assert.AreEqual(expected, actual);
        }

        private static string InstallPackageTest(string packageId)
        {
            var installPath = PathCombine("Library", packageId);

            if (Directory.Exists(installPath))
                Directory.Delete(installPath, true);

            return Coffee.CSharpCompilerSettings.CustomCompiler.GetInstalledPath(packageId).GetValueOrDefault().Path;
        }

        private static string PathCombine(params string[] paths)
        {
            return paths.Aggregate(Path.Combine);
        }
    }
}
