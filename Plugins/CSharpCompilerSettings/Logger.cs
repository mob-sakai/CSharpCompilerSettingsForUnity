using System;

namespace Coffee.CSharpCompilerSettings
{
    internal static class Logger
    {
        private static string s_LogHeader = "<b><color=#cc4444>[CscSettings]</color></b> ";
        private static Func<bool> s_EnableLogFunc = () => true;

        public static void Setup(string header, Func<bool> logLevelFunc)
        {
            s_LogHeader = header;
            s_EnableLogFunc = logLevelFunc;
        }

        public static void LogInfo(string format, params object[] args)
        {
            UnityEngine.Debug.Log(args.Length == 0
                ? s_LogHeader + format
                : string.Format(s_LogHeader + format, args));
        }

        public static void LogDebug(string format, params object[] args)
        {
            if (!s_EnableLogFunc()) return;

            LogInfo(format, args);
        }

        public static void LogWarning(string format, params object[] args)
        {
            if (!s_EnableLogFunc()) return;

            UnityEngine.Debug.LogWarning(args.Length == 0
                ? s_LogHeader + format
                : string.Format(s_LogHeader + format, args));
        }

        public static void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(new Exception(s_LogHeader + e.Message, e.InnerException));
        }

        public static void LogException(string format, params object[] args)
        {
            LogException(args.Length == 0
                ? new Exception(format)
                : new Exception(string.Format(format, args)));
        }
    }
}
