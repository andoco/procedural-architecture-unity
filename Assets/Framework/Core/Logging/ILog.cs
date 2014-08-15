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

        void Trace(object message);

        void Debug(object message);

        void Info(object message);

        void Warn(object message);

        void Error(object message);

        void Fatal(object message);
    }
    
}