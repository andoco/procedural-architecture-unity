using System.Linq;
using UnityEngine;

public class ShapeCommand : IShapeCommand
{
	public string Name { get; set; }
	
	public string[] Arguments { get; set; }

	public void Execute(IShapeConfiguration configuration)
	{
		switch (Name)
		{
		case "Set":
			var shapeName = Arguments[0].Trim('"');
			configuration.AddShape(shapeName);
			break;
		case "Trans":
			var axes = Arguments.Select(x => float.Parse(x)).ToArray();
			configuration.TransformScope(new Vector3(axes[0], axes[1], axes[2]));
			break;
		case "Rot":
			var rotAxes = Arguments.Select(x => float.Parse(x)).ToArray();
			configuration.RotateScope(new Vector3(rotAxes[0], rotAxes[1], rotAxes[2]));
			break;
		case "Scale":
			var scaleAxes = Arguments.Select(x => float.Parse(x)).ToArray();
			configuration.ScaleScope(new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]));
			break;
		}
	}
}