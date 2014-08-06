namespace Andoco.Unity.Framework.Core.Logging
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Core.Graph.Tree;
    using Common.Logging.Configuration;
    using Common.Logging.Factory;
    using Common.Logging;

    public class UnityLoggerFactoryAdapter : AbstractCachingLoggerFactoryAdapter
    {
        public static LogLevelConfig currentConfig;

        public UnityLoggerFactoryAdapter(NameValueCollection properties)
            : base()
        {
            currentConfig = new LogLevelConfig();
        }

        public UnityLoggerFactoryAdapter(NameValueCollection properties, LogLevelConfig config)
            : base()
        {
            currentConfig = config;
        }

        #region implemented abstract members of AbstractCachingLoggerFactoryAdapter
        protected override Common.Logging.ILog CreateLogger(string name)
        {
            return new UnityLogger(currentConfig);
        }
        #endregion
    }
}