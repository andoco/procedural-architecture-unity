namespace Andoco.Unity.Framework.Core.Logging
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Core.Graph.Tree;
    using Common.Logging.Configuration;
    using Common.Logging.Factory;
    using Common.Logging;

    public class UnityLogger : AbstractLogger
    {
        private LogLevelConfig config;

        public UnityLogger(LogLevelConfig config)
        {
            this.config = config;
        }

        #region implemented abstract members of AbstractLogger
        protected override void WriteInternal(Common.Logging.LogLevel level, object message, System.Exception exception)
        {
            UnityEngine.Debug.Log(string.Format("{0} {1} {2}", Time.time, level.ToString().ToUpper(), message));
        }
        public override bool IsTraceEnabled {
            get {
                return this.config.trace;
            }
        }
        public override bool IsDebugEnabled {
            get {
                return this.config.debug;
            }
        }
        public override bool IsInfoEnabled {
            get {
                return this.config.info;
            }
        }
        public override bool IsWarnEnabled {
            get {
                return this.config.warning;
            }
        }
        public override bool IsErrorEnabled {
            get {
                return this.config.error;
            }
        }
        public override bool IsFatalEnabled {
            get {
                return this.config.fatal;
            }
        }
        #endregion
    }
}