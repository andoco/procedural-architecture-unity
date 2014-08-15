namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Diagnostics;

    public static class LogManager
    {
        private static readonly object locker = new object();

        static LogManager()
        {
            LogFactory = new DefaultLoggerFactory();
        }

        public static ILoggerFactory LogFactory { get; set; }

        public static ILog GetCurrentClassLogger()
        {
            lock(locker)
            {
                var frame = new StackFrame(1, true);
                var type = frame.GetMethod().DeclaringType;
                
                return LogFactory.CreateLogger(type);
            }
        }
    }
}