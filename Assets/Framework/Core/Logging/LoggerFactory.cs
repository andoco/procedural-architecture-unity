namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections.Generic;

    public interface ILoggerFactory
    {
        ILog CreateLogger(Type type);
    }

    public abstract class LoggerFactory : ILoggerFactory
    {
        private readonly object locker = new object();
        private Dictionary<string, ILog> logCache = new Dictionary<string, ILog>();

        public ILog CreateLogger(Type type)
        {
            var name = type.Name;
            ILog log;

            lock(this.locker)
            {
                if (!this.logCache.TryGetValue(name, out log))
                {
                    log = this.CreateLogger(name);
                    this.logCache[name] = log;
                }
            }

            return log;
        }

        protected abstract ILog CreateLogger(string name);
    }
}