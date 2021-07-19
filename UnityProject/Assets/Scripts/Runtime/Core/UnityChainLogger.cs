using UnityEngine;

namespace UnityChain
{
    public static class UnityChainLogger
    {
        public static void Log(string format, params object[] args)
        {
            string message = Format(format, args);
            Debug.Log(message);
        }

        public static void LogError(string format, params object[] args)
        {
            string message = Format(format, args);
            Debug.LogError(message);
        }

        private static string Format(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return string.Format("{0}: {1}", s_logHeader, message);
        }

        private static string s_logHeader = "[UnityChain]";
    }
}