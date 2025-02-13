using NLog;

namespace CommonLibrary
{
    public static class Logging
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public static void LogError(string message)
        {
            Logger.Error(message);
        }
    }
}
