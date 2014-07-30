using UnityEngine;
using System.Collections.Generic;

public class StyleConfig : IStyleConfig
{
	private readonly IDictionary<string, IDictionary<string, object>> styles;

	public const string DefaultStyle = "default";

	public StyleConfig(IDictionary<string, IDictionary<string, object>> styles)
	{
		this.styles = styles;
	}
	
	public object GetStyle(string section, string key)
	{
		return this.styles[section][key];
	}

	public object GetStyleOrDefault(string section, string key, object defaultValue)
	{
		var result = defaultValue;

		IDictionary<string, object> sectionStyles;
		if (this.styles.TryGetValue(section, out sectionStyles))
		{
			object val;
			if (sectionStyles.TryGetValue(key, out val))
			{
				result = val;
			}
		}

		return result;
	}
}