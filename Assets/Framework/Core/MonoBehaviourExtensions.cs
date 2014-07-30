namespace Andoco.Unity.Framework.Core
{
	using System.Reflection;
	using System.Collections.Generic;
	using UnityEngine;

	public static class MonoBehaviourExtensions
	{
	    #region Logging

		public static void Log(this MonoBehaviour monoBehaviour, string msg, bool enabled = true)
		{
			if (enabled)
				Debug.Log(string.Format("{0} {1} {2}", Time.time, monoBehaviour, msg), monoBehaviour);
		}

	    public static void Warn(this MonoBehaviour monoBehaviour, string msg, bool enabled)
	    {
	        Debug.LogWarning(string.Format("{0} {1} {2}", Time.time, monoBehaviour, msg), monoBehaviour);
	    }

	    #endregion

	    public static IDictionary<FieldInfo, object> TrackFieldDefaults<T>(this T behaviour)
	    {
	        var t = typeof(T);
	        var vals = new Dictionary<FieldInfo, object>();

	        foreach (var f in t.GetFields())
	        {
                if (f.IsLiteral)
                    continue;

	            var val = f.GetValue(behaviour);
	            
	            vals[f] = val;
	        }
	        
	        return vals;
	    }
	    
	    public static void ResetFieldValues<T>(this T behaviour, IDictionary<FieldInfo, object> vals)
	    {
	        foreach (var item in vals)
	        {
	            item.Key.SetValue(behaviour, item.Value);
	        }
	    }
	}
}