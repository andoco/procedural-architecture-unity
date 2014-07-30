using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;

public class CommonArchitectureStyleConfig : StyleConfig
{
	public CommonArchitectureStyleConfig()
		: base(GetCommonStyles())
	{
	}

	public static IDictionary<string, IDictionary<string, object>> GetCommonStyles()
	{
		var beige = new Color(208f/255f, 197f/255f, 133f/255f);
		var grey = new Color(110f/255f, 110f/255f, 110f/255f);
		var roofTop = new Color(255f/255f, 195f/255f, 0);
		
		var styles = new Dictionary<string, IDictionary<string, object>> {
			{ "default", new Dictionary<string, object> { 
					{ "face-color", grey }
				}
			},
			{ "facade", new Dictionary<string, object> { 
					{ "face-color", grey }
				} 
			},
			{ "roof", new Dictionary<string, object> { 
					{ "top-color", roofTop },
					{ "side-color", grey }
				}
			},
			{ "hipped-roof", new Dictionary<string, object> { 
					{ "top-color", roofTop },
					{ "side-color", roofTop }
				}
			},
			{ "vert", new Dictionary<string, object> { 
					{ "face-color", grey }
				} 
			},
			{ "horiz", new Dictionary<string, object> { 
					{ "face-color", beige }
				} 
			},
			{ "window", new Dictionary<string, object> { 
					{ "face-color", new Color(0.95f, 0.95f, 1f) }
				} 
			},
		};

		return styles;
	}
}
