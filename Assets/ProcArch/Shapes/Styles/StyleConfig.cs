using System;
using System.Collections.Generic;
using UnityEngine;

public class StyleConfig : IStyleConfig
{
	public const string DefaultStyle = "default";
	public const string DefaultTheme = "default";
	
	private IDictionary<string, ShapeStyle> styles = new Dictionary<string, ShapeStyle>();
	private IDictionary<string, ShapeTheme> themes = new Dictionary<string, ShapeTheme>();

	public StyleConfig()
	{
		this.AddStyle(new ShapeStyle(DefaultStyle, new Dictionary<string, string>()));
		this.AddTheme(new ShapeTheme(DefaultTheme, new Dictionary<string, object> { { "default", Color.grey } }));
	}

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

		ShapeStyle s;
		if (!this.styles.TryGetValue(style, out s))
		{
			s = this.styles[DefaultStyle];
		}

		ShapeTheme t;
		if (!this.themes.TryGetValue(theme, out t))
		{
			t = this.themes[DefaultTheme];
		}

		var themeKey = s.GetThemeKey(key);
		themeVal = t[themeKey];

		return themeVal;
	}
}