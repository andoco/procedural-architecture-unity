namespace Andoco.Unity.Framework.Core.Logging
{
    using System;

    public abstract class Logger : ILog
    {
        private LogLevelConfig config;
        
        public Logger(string name, LogLevelConfig config)
        {
            this.Name = name;
            this.config = config;
        }

        public string Name { get; private set; }

        public bool IsTraceEnabled
        {
            get
            {
                return this.config.trace;
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return this.config.debug;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return this.config.info;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return this.config.warning;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return this.config.error;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return this.config.fatal;
            }
        }

        public void Trace(object message, params object[] args)
        {
            if (this.IsTraceEnabled)
            {
                this.InternalWrite(LogLevel.Trace, message, null, args);
            }
        }

        public void Debug(object message, params object[] args)
        {
            if (this.IsDebugEnabled)
            {
                this.InternalWrite(LogLevel.Debug, message, null, args);
            }
        }

        public void Info(object message, params object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.InternalWrite(LogLevel.Info, message, null, args);
            }
        }

        public void Warn(object message, params object[] args)
        {
            if (this.IsWarnEnabled)
            {
                this.InternalWrite(LogLevel.Warn, message, null, args);
            }
        }

        public void Error(object message, params object[] args)
        {
            if (this.IsInfoEnabled)
            {
                this.InternalWrite(LogLevel.Error, message, null, args);
            }
        }

        public void Fatal(object message, params object[] args)
        {
            if (this.IsFatalEnabled)
            {
                this.InternalWrite(LogLevel.Fatal, message, null, args);
            }
        }
        
        protected abstract void InternalWrite(LogLevel level, object message, System.Exception exception, object[] args);
    }
    
}