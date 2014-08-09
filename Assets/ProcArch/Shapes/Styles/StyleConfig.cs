using System;
using System.Collections.Generic;
using UnityEngine;

public class StyleConfig : IStyleConfig
{
	public const string DefaultStyle = "default";
	
	private IDictionary<string, ShapeStyle> styles = new Dictionary<string, ShapeStyle>();
	private IDictionary<string, ShapeTheme> themes = new Dictionary<string, ShapeTheme>();

	public StyleConfig()
	{
		this.DefaultTheme = "default";
		this.AddStyle(new ShapeStyle(DefaultStyle, new Dictionary<string, string>()));
		this.AddTheme(new ShapeTheme(DefaultTheme, new Dictionary<string, object> { { "default", Color.grey } }));
	}

	public string DefaultTheme { get; set; }

	public void AddStyle(ShapeStyle style)
	{
		this.styles[style.Name] = style;
	}
	
	public void AddTheme(ShapeTheme theme)
	{
		this.themes[theme.Name] = theme;
	}

	public object GetStyle(string style, string theme, string key)
	{
		object themeVal = null;

		// Get the requested style or use a default one if not found.
		ShapeStyle s;
		if (string.IsNullOrEmpty(style) || !this.styles.TryGetValue(style, out s))
		{
			s = this.styles[DefaultStyle];
		}

		// Get the requested theme or use a default one if not found.
		ShapeTheme t;
		if (string.IsNullOrEmpty(theme) || !this.themes.TryGetValue(theme, out t))
		{
			t = this.themes[DefaultTheme];
		}

		// Resolve the value for the requested key.
		var themeKey = s.GetThemeKey(key);
		themeVal = t.GetValue(themeKey);

		return themeVal;
	}
}