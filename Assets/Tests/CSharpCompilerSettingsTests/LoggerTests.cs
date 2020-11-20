using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = Coffee.CSharpCompilerSettings.Logger;

public class LoggerSetupTests
{
    [Test]
    public void Setup()
    {
        Logger.Setup("[TEST] ", () => true);
        LogAssert.Expect(LogType.Log, "[TEST] Info");
        Logger.LogInfo("Info");
    }
}

public class LoggerTests : IPrebuildSetup
{
    public void Setup()
    {
        Logger.Setup("[TEST] ", () => true);
    }

    [Test]
    public void LogInfo()
    {
        LogAssert.Expect(LogType.Log, "[TEST] Info");
        Logger.LogInfo("Info");
    }

    [Test]
    public void LogDebug()
    {
        LogAssert.Expect(LogType.Log, "[TEST] Debug");
        Logger.LogDebug("Debug");
    }

    [Test]
    public void LogWarning()
    {
        LogAssert.Expect(LogType.Warning, "[TEST] Warning");
        Logger.LogWarning("Warning");
    }

    [Test]
    public void LogExceptionFormat()
    {
        LogAssert.ignoreFailingMessages = true;
        LogAssert.Expect(LogType.Exception, "Exception: [TEST] Fmt: System.Exception: Exception");
        Logger.LogException("Fmt: {0}", new Exception("Exception"));
    }

    [Test]
    public void LogException()
    {
        LogAssert.ignoreFailingMessages = true;
        LogAssert.Expect(LogType.Exception, "Exception: [TEST] System.Exception: Exception");
        Logger.LogException("{0}", new Exception("Exception"));
    }
}
