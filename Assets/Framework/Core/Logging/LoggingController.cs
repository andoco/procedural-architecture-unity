namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class LoggingController : MonoBehaviour {

        public bool trace;
        public bool debug;
        public bool info = true;
        public bool warning = true;
        public bool error = true;
        public bool fatal = true;

        static LoggingController()
        {
            LogManager.LogFactory = new UnityLoggerFactory();
        }

    	void Awake()
        {
            StartCoroutine(UpdateLogConfig());
    	}
    	
        private IEnumerator UpdateLogConfig()
        {
            while (true)
            {
                if (LogManager.LogFactory is UnityLoggerFactory)
                {
                    var c = UnityLoggerFactory.CurrentConfig;
                    
                    c.trace = this.trace;
                    c.debug = this.debug;
                    c.info = this.info;
                    c.warning = this.warning;
                    c.error = this.error;
                    c.fatal = this.fatal;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}