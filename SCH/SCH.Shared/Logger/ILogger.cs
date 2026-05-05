using System;

namespace SCH.Shared.Logger
{
    public interface ILogger<T>
    {
        Type LoggerType => typeof(T);

        void Critical(string? message = null, params object?[] args);

        void Critical(Exception? exception = null, string? message = null, params object?[] args);

        void Error(string? message = null, params object?[] args);

        void Error(Exception? exception = null, string? message = null, params object?[] args);

        void Warn(string? message = null, params object?[] args);

        void Info(string? message = null, params object?[] args);

    }
}
