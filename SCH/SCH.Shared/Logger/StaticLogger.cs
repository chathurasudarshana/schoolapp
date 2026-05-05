namespace SCH.Shared.Logger
{
    using NLog;

    public static class StaticLogger
    {
        private static readonly NLog.Logger logger = LogManager.GetLogger("AppStaticLogger");

        public static void Error(string message)
        {
            logger.Error(message);
        }

        public static void Error<T>(T value)
        {
            logger.Error(value);
        }

        public static void Error(Exception exception)
        {
            logger.Error(exception);
        }

        public static void Error(Exception exception, string message)
        {
            logger.Error(exception, message);
        }


        public static void Error<T>(Exception exception, string message, T contextualData)
        {
            logger.Error(exception, message, contextualData);
        }

        public static void Warn(string message)
        {
            logger.Warn(message);
        }

        public static void Warn<T>(string message, T contextualData)
        {
            logger.Warn(message, contextualData);
        }

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Info<T>(string message, T contextualData)
        {
            logger.Info(message, contextualData);
        }

    }
}
