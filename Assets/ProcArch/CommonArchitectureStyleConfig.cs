namespace Andoco.Unity.ProcArch
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using System.Text;
    using System;
    using Andoco.Unity.ProcArch.Shapes.Styles;
    
    public class CommonArchitectureStyleConfig : StyleConfig
    {
        public CommonArchitectureStyleConfig ()
            : base()
        {
            // Styles
        
			this.AddStyle("facade", "face-color", "stone");
			this.AddStyle("roof", "face-color", "tile");
			this.AddStyle("chimney", "face-color", "stone");
			this.AddStyle("sided-roof", "top-color", "tile", "side-color", "stone");   
			this.AddStyle("door", "face-color", "stone", "Inner-color", "dark-wood");
			this.AddStyle("window", "face-color", "glass");
			this.AddStyle("windowSill", "face-color", "stone");
        
            // Themes

			this.AddTheme(
				"pale-stone-wood",
				"default", Color.red,
				"stone", new Color(255f/255f, 212f/255f, 140/255f),
				"dark-wood", new Color(59f/255f, 49f/255f, 41f/255f),
				"tile", new Color(143f/255f, 64f/255f, 16f/255f),
				"glass", new Color(0f, 0f, 0f));
            
            this.AddTheme(
				"villageRed",
                "default", Color.grey,
                "stone", new Color(194f/255f, 184f/255f, 139f/255f),
                "dark-wood", new Color(59f/255f, 49f/255f, 41f/255f),
                "tile", new Color(196f/255f, 82f/255f, 48f/255f));
    
            this.AddTheme(
				"villageBlue",
                "default", Color.grey,
                "stone", new Color(194f/255f, 184f/255f, 139f/255f),
                "dark-wood", new Color(59f/255f, 49f/255f, 41f/255f),
                "tile", new Color(46f/255f, 128f/255f, 194f/255f));
                
			this.DefaultTheme = "pale-stone-wood";
        }
    
        //  public static IDictionary<string, IDictionary<string, object>> GetCommonStyles()
        //  {
        //      var darkWood = new Color(59f/255f, 49f/255f, 41f/255f);
        //      var lightWood = new Color(124f/255f, 109f/255f, 94f/255f);
        //      var glass = new Color(206f/255f, 217f/255f, 203f/255f);
        //
        //      var windowLit = new Color(255f/255f, 255f/255f, 200f/255f);
        //
        //      var beige = new Color(208f/255f, 197f/255f, 133f/255f);
        //      var grey = new Color(110f/255f, 110f/255f, 110f/255f);
        //      var lightRed = new Color(255f/255f, 195f/255f, 0);
        //
        //      var godusFacade = new Color(194f/255f, 184f/255f, 139f/255f);
        //      var godusRedRoof = new Color(196f/255f, 82f/255f, 48f/255f);
        //      var godusBlueRoof = new Color(46f/255f, 128f/255f, 194f/255f);
        //
        //      var roofTop = godusRedRoof;
        //      var facade = godusFacade;
        //      var window = windowLit;
        //
        //      var styles = new Dictionary<string, IDictionary<string, object>> {
        //          { "default", new Dictionary<string, object> { 
        //                  { "face-color", grey }
        //              }
        //          },
        //          { "facade", new Dictionary<string, object> { 
        //                  { "face-color", facade }
        //              }
        //          },
        //          { "sided-roof", new Dictionary<string, object> { 
        //                  { "top-color", roofTop },
        //                  { "side-color", facade }
        //              }
        //          },
        //          { "roof", new Dictionary<string, object> { 
        //                  { "face-color", roofTop },
        //              }
        //          },
        //          { "vert", new Dictionary<string, object> { 
        //                  { "face-color", grey }
    //              } 
    //          },
    //          { "horiz", new Dictionary<string, object> { 
    //                  { "face-color", beige }
    //              } 
    //          },
    //          { "window", new Dictionary<string, object> { 
    //                  { "face-color", window }
    //              } 
    //          },
    //          { "door", new Dictionary<string, object> {
    //                  { "face-color", new Color(0.5f, 0.4f, 0f) },
    //                  { "Inner-color", darkWood }
    //              }
    //          }
    //      };
    //
    //      return styles;
    //  }
    }
}
