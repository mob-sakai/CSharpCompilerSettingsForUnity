using System.Collections;
using NUnit.Framework;
using Coffee.CSharpCompilerSettings;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = Coffee.CSharpCompilerSettings.Logger;

public class RequestCompilationTests : IPostBuildCleanup, IPrebuildSetup
{
    public void Setup()
    {
        Logger.Setup("[TEST] ", () => true);
        Utils.DisableCompilation = true;
    }

    [Test]
    public void RequestCompilation_Test()
    {
#if UNITY_2019_1_OR_NEWER
        Utils.DisableCompilation = false;
        Utils.RequestCompilation("CSharpCompilerSettingsTests");
#endif
    }

    [Test]
    public void RequestCompilation()
    {
        LogAssert.Expect(LogType.Warning, "[TEST] Skipped: Disable compilation");
        Utils.DisableCompilation = true;
        Utils.RequestCompilation();
    }

    public void Cleanup()
    {
        Utils.DisableCompilation = false;
    }
}
