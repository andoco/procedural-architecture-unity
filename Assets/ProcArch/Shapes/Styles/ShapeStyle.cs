namespace Andoco.Unity.ProcArch.Shapes.Styles
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    
    /// <summary>
    /// Provides a mapping from shape part names to a logical theme names (e.g. "wall" => "stone").
    /// </summary>
    public class ShapeStyle
    {
        private Dictionary<string, string> data = new Dictionary<string, string> (StringComparer.InvariantCultureIgnoreCase);
    
        /// <summary>
        /// The default theme key returned when the style does not contain a mapping for a style key.
        /// </summary>
        public const string DefaultThemeKey = "default";
    
        public ShapeStyle (string name, IDictionary<string, string> items)
        {
            this.Name = name;
            this.AddRange (items);
        }
    
        public string this [string key] {
            get {
                string result;
                if (!this.data.TryGetValue (key, out result)) {
                    throw new ArgumentException (string.Format ("Could not find key [{0}] in style [{1}]", key, this.Name), "key");
                }
                return result;
            }
        }
    
        public string Name { get; private set; }
    
        public void AddRange (IDictionary<string, string> items)
        {
            foreach (var item in items) {
                this.data [item.Key] = item.Value;
            }
        }
    
        public string GetThemeKey (string styleKey)
        {
            string themeKey;
            if (!this.data.TryGetValue(styleKey, out themeKey))
            {
                themeKey = DefaultThemeKey;
            }
    
            return themeKey;
        }
    }
}
