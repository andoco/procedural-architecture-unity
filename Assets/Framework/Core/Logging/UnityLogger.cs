namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;

    public class UnityLogger : Logger
    {
        public UnityLogger(string name, LogLevelConfig config)
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
    
}