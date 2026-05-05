namespace SCH.Shared.Logger
{
    using Microsoft.Extensions.Logging;

    public class Logger<T>: ILogger<T>
    {
        protected readonly Microsoft.Extensions.Logging.ILogger<T> logger;

        public Type LoggerType => typeof(T);

        public Logger(Microsoft.Extensions.Logging.ILogger<T> logger)
        {
            this.logger = logger;
        }

        void ILogger<T>.Critical(string? message, params object?[] args)
        {
            logger.LogCritical(message, args);
        }

        void ILogger<T>.Critical(
            Exception? exception, string? message, params object?[] args)
        {
            logger.LogCritical(exception, message, args);
        }

        void ILogger<T>.Error(string? message, params object?[] args)
        {
            logger.LogError(message, args);
        }

        void ILogger<T>.Error(
            Exception? exception, string? message, params object?[] args)
        {
            logger.LogError(exception, message, args);
        }

 
        void ILogger<T>.Warn(string? message, params object?[] args)
        {
            logger.LogWarning(message, args);
        }


        void ILogger<T>.Info(string? message, params object?[] args)
        {
            logger.LogInformation(message, args);
        }



    }
}
