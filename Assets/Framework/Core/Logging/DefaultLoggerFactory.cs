namespace Andoco.Unity.Framework.Core.Logging
{
    using System;

    public sealed class DefaultLoggerFactory : LoggerFactory
    {
        protected override ILog CreateLogger(string name)
        {
            return new NullLogger(name);
        }
    }
}