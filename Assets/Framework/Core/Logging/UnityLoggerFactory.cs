namespace Andoco.Unity.Framework.Core.Logging
{
    public class UnityLoggerFactory : LoggerFactory
    {
        public static LogLevelConfig CurrentConfig { get; set; }

        public UnityLoggerFactory()
        {
            CurrentConfig = new LogLevelConfig();
        }

        protected override ILog CreateLogger(string name)
        {
            return new UnityLogger(name, CurrentConfig);
        }
    }
}
