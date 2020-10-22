using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Coffee.CSharpCompilerSettings
{
    internal class ShellHelper
    {
        public static void ExecuteCommand(string exe, string args)
        {
            Core.LogInfo("Execute command: {0} {1}", exe, args);

            var p = Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
            });

            // Don't consume 100% of CPU while waiting for process to exit
            if (Application.platform == RuntimePlatform.OSXEditor)
                while (!p.HasExited)
                    Thread.Sleep(100);
            else
                p.WaitForExit();

            if (p.ExitCode != 0)
            {
                var ex = new Exception(p.StandardError.ReadToEnd());
                Core.LogException(ex);
                throw ex;
            }
        }
    }
}
