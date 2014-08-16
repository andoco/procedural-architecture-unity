namespace Andoco.Unity.ProcArch.Shapes.Styles
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class StyleConfig : IStyleConfig
    {
        public const string DefaultStyle = "default";
        
        private IDictionary<string, ShapeStyle> styles = new Dictionary<string, ShapeStyle> ();
        private IDictionary<string, ShapeTheme> themes = new Dictionary<string, ShapeTheme> ();
    
        public StyleConfig ()
        {
            this.DefaultTheme = "default";
            this.AddStyle (new ShapeStyle (DefaultStyle, new Dictionary<string, string> ()));
            this.AddTheme (new ShapeTheme (DefaultTheme, new Dictionary<string, object> { { "default", Color.grey } }));
        }
    
        public string DefaultTheme { get; set; }
    
        public void AddStyle(ShapeStyle style)
        {
            this.styles[style.Name] = style;
        }

		public void AddStyle(string name, params string[] styleArgs)
		{
			if (styleArgs.Length % 2 != 0)
				throw new System.ArgumentException("Styles must be supplied in pairs of two: style-name, theme-key");

			var styleDictionary = new Dictionary<string, string>();

			for (var i=0; i < styleArgs.Length; i+=2)
			{
				styleDictionary[styleArgs[i]] = styleArgs[i+1];
			}

			this.AddStyle(new ShapeStyle(name, styleDictionary));
		}
        
        public void AddTheme(ShapeTheme theme)
        {
            this.themes[theme.Name] = theme;
        }

		public void AddTheme(string name, params object[] themeArgs)
		{
			if (themeArgs.Length % 2 != 0)
				throw new System.ArgumentException("Themes must be supplied in pairs of two: theme-name, theme-value");
			
			var dictionary = new Dictionary<string, object>();
			
			for (var i=0; i < themeArgs.Length; i+=2)
			{
				dictionary[(string)themeArgs[i]] = themeArgs[i+1];
			}
			
			this.AddTheme(new ShapeTheme(name, dictionary));
		}
    
        public object GetStyle (string style, string theme, string key)
        {
            object themeVal = null;
    
            // Get the requested style or use a default one if not found.
            ShapeStyle s;
            if (string.IsNullOrEmpty (style) || !this.styles.TryGetValue (style, out s)) {
                s = this.styles [DefaultStyle];
            }
    
            // Get the requested theme or use a default one if not found.
            ShapeTheme t;
            if (string.IsNullOrEmpty (theme) || !this.themes.TryGetValue (theme, out t)) {
                t = this.themes [DefaultTheme];
            }
    
            // Resolve the value for the requested key.
            var themeKey = s.GetThemeKey(key);
            themeVal = t.GetValue(themeKey);
    
            return themeVal;
        }
    }
}