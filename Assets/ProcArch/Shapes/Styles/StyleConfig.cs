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
	
	public T GetStyle<T>(string section, string key)
	{
		return (T)this.styles[section][key];
	}

	public T GetStyleOrDefault<T>(string section, string key, T defaultValue)
	{
		var result = defaultValue;

		IDictionary<string, object> sectionStyles;
		if (this.styles.TryGetValue(section, out sectionStyles))
		{
			object val;
			if (sectionStyles.TryGetValue(key, out val))
			{
				result = (T)val;
			}
		}

		return result;
	}
}