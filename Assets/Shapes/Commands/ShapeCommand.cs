using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeCommand : IShapeCommand
{
	/// <summary>
	/// Gets or sets the name of the command.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the arguments to be supplied to the command.
	/// </summary>
	public string[] Arguments { get; set; }

	/// <summary>
	/// Gets or sets the shape symbols that are present in the command body.
	/// </summary>
	public ShapeSymbol[] Shapes { get; set; }

	public void Execute(IShapeConfiguration configuration)
	{
		switch (Name)
		{
		case "Set":
		case "Vol":
			var volArgs = configuration.ResolveArgs(this.Arguments);
			var volName = volArgs[0];
			var volStyle = volArgs.Length > 1 ? TrimArg(volArgs[1]) : null;
			configuration.AddVolume(TrimArg(volName), volStyle);
			break;
		case "Trans":
			var axes = configuration.ResolveArgs(this.Arguments).Select(x => float.Parse(x)).ToArray();
			configuration.TransformScope(new Vector3(axes[0], axes[1], axes[2]));
			break;
		case "Rot":
			var rotAxes = configuration.ResolveArgs(this.Arguments).Select(x => float.Parse(x)).ToArray();
			configuration.RotateScope(new Vector3(rotAxes[0], rotAxes[1], rotAxes[2]));
			break;
		case "Scale":
			var scaleAxes = configuration.ResolveArgs(this.Arguments).Select(x => float.Parse(x)).ToArray();
			configuration.ScaleScope(new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]));
			break;
		case "Push":
			configuration.PushScope();
			break;
		case "Pop":
			configuration.PopScope();
			break;
		case "Subdiv":
			var subdivArgs = configuration.ResolveArgs(this.Arguments);
			var sizes = subdivArgs.Skip(1).Select(arg => Size.Parse(arg)).ToArray();
			configuration.SplitDivideScope(TrimArg(subdivArgs[0]), sizes, Shapes);
			break;
		case "Comp":
			var compArgs = configuration.ResolveArgs(this.Arguments);
			configuration.SplitComponent(TrimArg(compArgs[0]), Shapes[0]);
			break;
		default:
			throw new System.ArgumentException(string.Format("Unknown command: {0}", this.Name), "Name");
		}
	}

	private static string TrimArg(string arg)
	{
		return arg.Trim('"');
	}
}