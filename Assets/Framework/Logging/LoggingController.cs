using System.Diagnostics;

namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;
    using System.Collections;

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

    public abstract class Log : ILog
    {
        private LogLevelConfig config;
        
        public Log(string name, LogLevelConfig config)
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

        public void Trace(object message)
        {
            if (this.IsTraceEnabled)
            {
                this.InternalWrite(LogLevel.Trace, message, null);
            }
        }

        public void Debug(object message)
        {
            if (this.IsDebugEnabled)
            {
                this.InternalWrite(LogLevel.Debug, message, null);
            }
        }

        public void Info(object message)
        {
            if (this.IsInfoEnabled)
            {
                this.InternalWrite(LogLevel.Info, message, null);
            }
        }

        public void Warn(object message)
        {
            if (this.IsWarnEnabled)
            {
                this.InternalWrite(LogLevel.Warn, message, null);
            }
        }

        public void Error(object message)
        {
            if (this.IsInfoEnabled)
            {
                this.InternalWrite(LogLevel.Error, message, null);
            }
        }

        public void Fatal(object message)
        {
            if (this.IsFatalEnabled)
            {
                this.InternalWrite(LogLevel.Fatal, message, null);
            }
        }
        
        protected abstract void InternalWrite(LogLevel level, object message, System.Exception exception);
    }

    public class UnityLog : Log
    {
        public UnityLog(string name, LogLevelConfig config)
            : base(name, config)
        {
        }

        protected override void InternalWrite(LogLevel level, object message, System.Exception exception)
        {
            var msg = string.Format("{0} {1} {2} {3}", this.Name, Time.time, level.ToString().ToUpper(), message);
            
            switch (level)
            {
            case LogLevel.Debug:
            case LogLevel.Trace:
            case LogLevel.Info:
                UnityEngine.Debug.Log(msg);
                break;
                
            case LogLevel.Warn:
                UnityEngine.Debug.LogWarning(msg);
                break;
                
            case LogLevel.Error:
            case LogLevel.Fatal:
                UnityEngine.Debug.LogError(msg);
                break;
            }
        }
    }

    public interface ILogFactory
    {
        ILog CreateLogger(Type type);
    }

    public abstract class LogFactory : ILogFactory
    {
        public ILog CreateLogger(Type type)
        {
            return this.CreateLogger(type.Name);
        }

        protected abstract ILog CreateLogger(string name);
    }

    public class UnityLogFactory : LogFactory
    {
        public static LogLevelConfig CurrentConfig { get; set; }

        public UnityLogFactory()
        {
            CurrentConfig = new LogLevelConfig();
        }

        protected override ILog CreateLogger(string name)
        {
            // TODO: caching
            return new UnityLog(name, CurrentConfig);
        }
    }

    public static class LogManager
    {
        public static ILogFactory LogFactory { get; set; }

        public static ILog GetCurrentClassLogger()
        {
            // TODO: thread safety
            var frame = new StackFrame(1, true);
            var type = frame.GetMethod().DeclaringType;

            return LogFactory.CreateLogger(type);
        }
    }

    public class LoggingController : MonoBehaviour {

        public bool trace;
        public bool debug;
        public bool info;
        public bool warning;
        public bool error;
        public bool fatal;

        static LoggingController()
        {
            LogManager.LogFactory = new UnityLogFactory();
        }

    	void Awake()
        {
            StartCoroutine(UpdateLogConfig());
    	}
    	
        private IEnumerator UpdateLogConfig()
        {
            while (true)
            {
                var c = UnityLogFactory.CurrentConfig;
                c.trace = this.trace;
                c.debug = this.debug;
                c.info = this.info;
                c.warning = this.warning;
                c.error = this.error;
                c.fatal = this.fatal;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}