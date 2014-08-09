using UnityEngine;
using System.Collections.Generic;

public static class StyleConfigExtensions
{
	public static Color GetColor(this IStyleConfig styleConfig, string section, string key)
	{
		return (Color)styleConfig.GetStyle(section, key);
	}

	public static Color GetColorOrDefault(this IStyleConfig styleConfig, string section, string key, Color defaultValue)
	{
		return (Color)(styleConfig.GetStyleOrDefault(section, key, null) ?? defaultValue);
	}
}