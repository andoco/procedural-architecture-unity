namespace Andoco.Unity.Framework.Core.Logging
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public interface ILog
    {
        string Name { get; }

        bool IsTraceEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }
        
        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        void Trace(object message, params object[] args);

        void Debug(object message, params object[] args);

        void Info(object message, params object[] args);

        void Warn(object message, params object[] args);

        void Error(object message, params object[] args);

        void Fatal(object message, params object[] args);
    }
}