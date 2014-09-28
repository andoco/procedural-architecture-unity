using Andoco.Core;

namespace Andoco.Unity.Framework.Core
{
    using System;
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Misc;

	public class Diagnostics : MonoBehaviour {
        private const int bytesPerMB = 1048576;

		private static SingletonGameObjectComponent<Diagnostics> diagnostics = new SingletonGameObjectComponent<Diagnostics>();

	    private readonly IDictionary<string, string> entries = new Dictionary<string, string>();
        private LazyDictionary<string, TimelineChartInfo> charts;
        private Dictionary<int, TimelineChartInfo> timelineChartMap = new Dictionary<int, TimelineChartInfo>();
        private GarbageCollectionState garbageCollectionCountState = new GarbageCollectionState();

	    private int currentTimelineId = -1;
	    public bool timelinesVisible;

	    public bool loggingEnabled = false;
	    public bool entriesVisible;
	    	    
	    public delegate float TimelineSampleSourceDelegate(int timelineId);

	    private float accum   = 0f; // FPS accumulated over the interval
	    private int   frames  = 0; // Frames drawn over the interval
	    private string sFPS = ""; // The fps formatted into a string.
	    private float accumTotal = 0f;
	    private int framesTotal = 0;
	    private float avgFps = 0f;
	    private string sAvgFps = "";
        private int fpsTimelineId;

        public GarbageCollectionConfig garbageCollectionConfig;

		public  float frequency = 0.5F; // The update frequency of the fps
		public int nbDecimal = 1; // How many decimal do you want to display

	    public float CurrentFps { get; private set; }

        public float defaultSampleHistoryTime = 10f;
        public float defaultVerticalScale = 100f;

		public static Diagnostics Instance { get { return diagnostics.Instance; } }

        void Awake()
        {
            this.charts = new LazyDictionary<string, TimelineChartInfo>(name => new TimelineChartInfo { Name = name });
        }
	    	
		// Use this for initialization
		void Start () {
            this.fpsTimelineId = this.CreateTimeline("FPS", Color.red);
            this.garbageCollectionCountState.totalMemoryTimelineId = this.CreateTimeline("GC TotalMemory", Color.green);
            this.garbageCollectionCountState.collectionCountTimelineId = this.CreateTimeline("GC Collections (x10)", Color.blue);

			StartCoroutine(FPS());
            StartCoroutine(UpdateGarbageCollection());

			if (this.timelinesVisible)
				this.StartTimeline();
		}
        		
		// Update is called once per frame
		void Update () {
			accum += Time.timeScale/ Time.deltaTime;
			++frames;
		}
        		
		void OnGUI()
		{
	        GUILayout.BeginVertical();
	        GUILayout.BeginArea(new Rect(10, 10, 250, Screen.height));

	        // FPS box
	        GUILayout.Box(string.Format("{0} FPS (Avg {1})", this.sFPS, this.sAvgFps));

	        // Entries toggle
	        this.entriesVisible = GUILayout.Toggle(this.entriesVisible, "Messages");

	        // Timelines toggle
	        var newTimelinesVisible = GUILayout.Toggle(this.timelinesVisible, "Timelines");
	        if (newTimelinesVisible && !this.timelinesVisible)
	        {
				this.StartTimeline();
	        }
	        else if (!newTimelinesVisible && this.timelinesVisible)
	        {
	            CancelInvoke("UpdateTimelines");
	        }
	        this.timelinesVisible = newTimelinesVisible;

	        if (this.entriesVisible)
	            this.DrawEntries();

			GUILayout.EndVertical();
	        GUILayout.EndArea();

	        // Timelines window
	        if (this.timelinesVisible)
            {
                foreach (var chart in this.charts.Values)
                {
                    chart.WindowRect = GUI.Window(chart.WindowId, chart.WindowRect, DrawTimelineChartWindow, chart.Name);
                }
            }
		}

		IEnumerator FPS()
		{
			// Infinite loop executed every "frenquency" secondes.
			while( true )
			{
				// Update the FPS
				float fps = accum/frames;
				sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
	            this.CurrentFps = fps;

	            this.accumTotal += accum;
	            this.framesTotal += frames;
	            this.avgFps = this.accumTotal / this.framesTotal;
	            this.sAvgFps = this.avgFps.ToString("f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
							
				accum = 0.0F;
				frames = 0;

                this.LogTimelineSample(this.fpsTimelineId, avgFps);
				
				yield return new WaitForSeconds( frequency );
			}
		}

	    #region Public methods

	    public void SetEntry(string name, string text)
	    {
	        this.entries[name] = text;
	    }

        public int CreateTimeline(
            string timelineName, 
            Color color, 
            TimelineSampleSourceDelegate source = null)
        {
            return this.CreateTimeline(timelineName, "Default", color, source);
        }
            
        public int CreateTimeline(
            string timelineName, 
            string chartName, 
            Color color, 
            TimelineSampleSourceDelegate source = null, 
            float? verticalScale = null,
            float? sampleHistory = null)
	    {
            var chart = this.charts.GetOrInit(chartName);

            // Vertical scale.
            var verticalScaleToUse = verticalScale ?? this.defaultVerticalScale;
            chart.Config.VerticalScale = verticalScaleToUse;
            chart.Config.VerticalMarkers = chart.Config.VerticalScale / 2f;

            // Sample history time.
            var sampleHistoryTime = sampleHistory ?? this.defaultSampleHistoryTime;
            chart.SampleHistoryTime = sampleHistoryTime;
            chart.Config.HorizontalScale = chart.SampleHistoryTime;
            chart.Config.HorizontalMarkers = chart.Config.HorizontalScale / 4f;
            
            var newId = this.GetNextTimelineId();
            var timeline = new TimelineInfo(newId, timelineName)
	        {
	            SampleSource = source,
	            Color = color,
                Chart = chart
	        };

            chart.Timelines[newId] = timeline;
            this.timelineChartMap[newId] = chart;

	        this.Log(string.Format("Created timeline {0}", timeline), this.loggingEnabled);
	        
	        // Initialise the samples with zeros to make chart drawing simpler.
//            var numSamples = chart.GetSampleHistorySize();
//	        for (var i=0; i < numSamples; i++)
//	            timeline.Samples.Enqueue(0f);
	        
	        return newId;
	    }
        	    
	    public void LogTimelineSample(int timelineId, float sampleValue)
	    {
            if (this.timelinesVisible)
            {
                this.LogTimelineSample(this.GetTimeline(timelineId), sampleValue);
            }
	    }

	    public void RegisterSampleSource(int timelineId, TimelineSampleSourceDelegate source)
	    {
            var timeline = this.GetTimeline(timelineId);
	        timeline.SampleSource = source;
	    }
	    
	    #endregion
	    
	    #region Private methods

        private TimelineInfo GetTimeline(int timelineId)
        {
            var chart = this.timelineChartMap[timelineId];
            return chart.Timelines[timelineId];
        }

	    private void DrawEntries()
	    {
	        for (int i=0; i < this.entries.Count; i++)
	        {
	            var entry = this.entries.ElementAt(i);
	            GUILayout.Box(string.Format("{0} : {1}", entry.Key, entry.Value));
	        }
	    }

	    private int GetNextTimelineId()
	    {
	        this.currentTimelineId++;
	        return this.currentTimelineId;
	    }

	    private void LogTimelineSample(TimelineInfo timeline, float sampleValue)
	    {
            if (timeline.Samples.Any())
            {
                var next = timeline.Samples.Peek();
                while (next != null && next.Timestamp < Time.time - timeline.Chart.SampleHistoryTime)
                {
                    timeline.Samples.Dequeue();
                    next = timeline.Samples.Peek();
                }
            }
	        
            var sample = new TimelineSample(Time.time, sampleValue);
	        timeline.Samples.Enqueue(sample);
	        
	        // Flag that the timeline has received a sample for this period.
	        timeline.IsLatestSampleReceived = true;
	    }
	    
		private void StartTimeline()
		{
//			InvokeRepeating("UpdateTimelines", this.sampleFrequency, this.sampleFrequency);
		}

	    private void DrawTimelineChartWindow(int windowId)
	    {
            var chart = this.charts.Values.Single(c => c.WindowId == windowId);
	        this.DrawTimelineChart(chart);
	        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	    }
	    
	    private void DrawTimelineChart(TimelineChartInfo chart)
	    {
	        Drawing.DrawChartGrid(chart.Config);

	        var style = new GUIStyle();

	        var lblYPos = 0f;
            foreach (var timeline in chart.Timelines.Values)
	        {
	            lblYPos += 20f;
	            style.normal.textColor = timeline.Color;
	            GUI.Label(new Rect(40, lblYPos, 100, 20), timeline.Name, style);

	            DrawChart(timeline.Samples.ToArray(), chart.Config, timeline.Color);
	        }
	    }

        private void DrawChart(TimelineSample[] samples, Drawing.ChartConfig config, Color? color = null)
        {
            if (samples.Length < 2)
                return;

            color = color ?? Color.yellow;
            var baseX = config.Rect.x;
            var baseY = config.Rect.y;
            var w = config.Rect.width;
            var h = config.Rect.height;
            
            const float lineWidth = 1f;
            var previousPos = new Vector2(baseX, baseY + h);

            if (w <= config.HorizontalScale)
                throw new UnityException("scale is smaller than width");

            var startTime = samples[0].Timestamp;
//            var endTime = samples[samples.Length - 1].Timestamp;
            var pointsPerSec = w / config.HorizontalScale;

//            // Test using manual values
//            for (var i=0f; i <= config.HorizontalScale; i+=1f)
//            {
//                var x = baseX + stepSize * i;
//                var y = baseY + h / 2f + (Random.value * h * 0.25f);
//                var pos = new Vector2(x, y);
//                Drawing.DrawLine(previousPos, pos, color.Value, lineWidth);
//                previousPos = pos;
//            }

//            return;
            // end test

            foreach (var sample in samples)
            {
                var t = sample.Timestamp - startTime;
//                t = (float)(System.Math.Truncate((double)t*10.0f) / 10.0f);
                var x = baseX + t * pointsPerSec;
                x = (float)(System.Math.Truncate((double)x*1.0f) / 1.0f);
                var y = (baseY + h) - sample.Value * (h / config.VerticalScale);
                y = (float)(System.Math.Truncate((double)y*1.0f) / 1.0f);
                var pos = new Vector2(x, y);
                
                Drawing.DrawLine(previousPos, pos, color.Value, lineWidth);
//                GUI.Label(new Rect(x, y, 50f, 50f), sample.Timestamp.ToString("#.##"));
                
                previousPos = pos;
            }
        }
	    
//	    private void UpdateTimelines()
//	    {
//            foreach (var chart in this.charts.Values)
//            {
//                if (chart.LastSampleTime + (1f / chart.SampleFrequency) <= Time.time)
//                {
//                    UpdateTimelines(chart);
//                    chart.LastSampleTime = Time.time;
//                }
//            }
//	    }

//        private void UpdateTimelines(TimelineChartInfo chart)
//        {
//            // Get sample values from registered sample sources.
//            foreach (var timeline in chart.Timelines.Values)
//            {
//                if (timeline.SampleSource != null)
//                {
//                    this.LogTimelineSample(timeline, timeline.SampleSource(timeline.Id));
//                }
//            }            
//        }

        private IEnumerator UpdateGarbageCollection()
        {
            yield return new WaitForSeconds(this.garbageCollectionConfig.initialDelay);

            this.garbageCollectionCountState.lastCollectionCount = System.GC.CollectionCount(0);
            this.garbageCollectionCountState.lastCollectionTime = Time.time;

            while (true)
            {
                // Collections.
                var current = System.GC.CollectionCount(0);
                this.SetEntry("GC Collections", current.ToString());

                var t = Time.time;
                var collectionCountDelta = current - this.garbageCollectionCountState.lastCollectionCount;
                var collectionTimeDelta = t - this.garbageCollectionCountState.lastCollectionTime;
                var currentAvg = collectionCountDelta / collectionTimeDelta;
                this.SetEntry("GC Last Collections/sec", currentAvg.ToString("0.00"));

                this.garbageCollectionCountState.totalCollectionCount += collectionCountDelta;
                this.garbageCollectionCountState.totalCollectionTime += collectionTimeDelta;
                var totalAvg = this.garbageCollectionCountState.totalCollectionCount / this.garbageCollectionCountState.totalCollectionTime;
                this.SetEntry("GC Total Collections/sec", totalAvg.ToString("0.00"));
                this.LogTimelineSample(this.garbageCollectionCountState.collectionCountTimelineId, totalAvg * 10f);

                this.garbageCollectionCountState.lastCollectionCount = current;
                this.garbageCollectionCountState.lastCollectionTime = t;

                // Total memory.
                var totalMb = System.GC.GetTotalMemory(false) / bytesPerMB;
                this.SetEntry("GC TotalMemory", totalMb.ToString());
                this.LogTimelineSample(this.garbageCollectionCountState.totalMemoryTimelineId, totalMb);

                yield return new WaitForSeconds(this.garbageCollectionConfig.frequency);
            }
        }
        	    
	    #endregion

        [Serializable]
        public class GarbageCollectionConfig
        {
            public float initialDelay = 5f;
            public float frequency = 1f;
        }
        
        public class GarbageCollectionState
        {
            public int collectionCountTimelineId;
            public int totalMemoryTimelineId;

            public int lastCollectionCount;
            public float lastCollectionTime;
            
            public int totalCollectionCount;
            public float totalCollectionTime;
        }

	    #region Private classes

        private sealed class TimelineChartInfo
        {
            private static int currentWindowId;

            public TimelineChartInfo()
            {
                this.WindowId = currentWindowId;
                currentWindowId++;

                // IMPORTANT: The game window must run maximised otherwise we'll see line glitches.
                this.WindowRect = new Rect(0f, 0, Screen.width / 2f, Screen.height / 4f);

                this.Timelines = new Dictionary<int, TimelineInfo>();
                this.SampleHistoryTime = 10f;

                this.Config = new Drawing.ChartConfig { 
                    // TODO: Rendering bug when using margins.
//                    Rect = new Rect(20f, 20f, this.WindowRect.width - 40f, this.WindowRect.height - 25f),
                    Rect = this.WindowRect,
                    HorizontalMarkers = 1f,
                    VerticalScale = 100f, 
                    VerticalMarkers = 5f
                };
            }

            /// <summary>
            /// Gets or sets amount of time in seconds for which sample history will be maintained.
            /// </summary>
            public float SampleHistoryTime { get; set; }

            public float LastSampleTime { get; set; }

            public int WindowId { get; set; }

            public Rect WindowRect { get; set; }

            public Drawing.ChartConfig Config { get; set; }

            public string Name { get; set; }

            public IDictionary<int, TimelineInfo> Timelines { get; private set; }
        }
	    
	    private sealed class TimelineInfo
	    {
	        public TimelineInfo(int id, string name)
	        {
	            this.Id = id;
	            this.Name = name;
	            this.Samples = new Queue<TimelineSample>();
	            this.Color = Color.black;
	        }

            public TimelineChartInfo Chart { get; set; }
	        
	        public int Id { get; private set; }
	        
	        public string Name { get; private set; }
	        
	        public Queue<TimelineSample> Samples { get; private set; }
	        
	        public bool IsLatestSampleReceived { get; set; }
	        
	        public TimelineSampleSourceDelegate SampleSource { get; set; }

	        public Color Color { get; set; }

	        public override string ToString ()
	        {
	            return string.Format ("[TimelineInfo: Id={0}, Name={1}]", Id, Name);
	        }
	    }

        private sealed class TimelineSample
        {
            public TimelineSample(float timestamp, float value)
            {
                this.Timestamp = timestamp;
                this.Value = value;
            }

            public float Timestamp { get; private set; }

            public float Value { get; private set; }
        }
	    
	    #endregion
	}
}