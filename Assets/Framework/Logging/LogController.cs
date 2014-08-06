namespace Andoco.Unity.Framework.Core.Logging
{
    using UnityEngine;
    using System.Collections;
    using Common.Logging.Configuration;

    public class LogController : MonoBehaviour {

        public bool trace;
        public bool debug;
        public bool info;
        public bool warning;
        public bool error;
        public bool fatal;

        static LogController()
        {
            ConfigureCommonLogging();
        }

    	void Awake()
        {
            StartCoroutine(UpdateLogConfig());
    	}
    	
        private static void ConfigureCommonLogging()
        {
            Debug.Log("Configuring Common.Logging");

            // create properties
            var properties = new NameValueCollection();
            properties["showDateTime"] = "true";        
            Common.Logging.LogManager.Adapter = new UnityLoggerFactoryAdapter(properties);        
        }

        private IEnumerator UpdateLogConfig()
        {
            while (true)
            {
                var c = UnityLoggerFactoryAdapter.currentConfig;
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