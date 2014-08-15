namespace Andoco.Unity.Framework.Core.Logging
{
    using System;

    public sealed class NullLogger : Logger
    {
        public NullLogger(string name)
            : base(name, new LogLevelConfig())
        {
        }

        protected override void InternalWrite(LogLevel level, object message, Exception exception)
        {
            throw new NotImplementedException("NullLogger should never reach this point");
        }
    }
}