using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Provides real values for logical theme names (e.g. "stone", "wood", "metal").
/// </summary>
public class ShapeTheme
{
	private Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
	
	public ShapeTheme(string name, IDictionary<string, object> items)
	{
		this.Name = name;
		this.AddRange(items);
	}

	public object this[string key]
	{
		get
		{
			object result;
			if (!this.data.TryGetValue(key, out result))
			{
				throw new ArgumentException(string.Format("Could not find key [{0}] in theme [{1}]", key, this.Name), "key");
			}
			return result;
		}
	}
	
	public string Name { get; private set; }

	public void AddRange(IDictionary<string, object> items)
	{
		foreach (var item in items)
		{
			this.data[item.Key] = item.Value;
		}
	}
}
