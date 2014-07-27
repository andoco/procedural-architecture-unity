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
		var resolvedArgs = configuration.ResolveArgs(this.Arguments); //this.ResolveArgs(configuration);

		switch (Name)
		{
		case "Set":
		case "Vol":
			configuration.AddVolume(TrimArg(Arguments[0]));
			break;
		case "Trans":
			var axes = resolvedArgs.Select(x => float.Parse(x)).ToArray();
			configuration.TransformScope(new Vector3(axes[0], axes[1], axes[2]));
			break;
		case "Rot":
			var rotAxes = resolvedArgs.Select(x => float.Parse(x)).ToArray();
			configuration.RotateScope(new Vector3(rotAxes[0], rotAxes[1], rotAxes[2]));
			break;
		case "Scale":
			var scaleAxes = resolvedArgs.Select(x => float.Parse(x)).ToArray();
			configuration.ScaleScope(new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]));
			break;
		case "Push":
			configuration.PushScope();
			break;
		case "Pop":
			configuration.PopScope();
			break;
		case "Subdiv":
			var sizes = Arguments.Skip(1).Select(arg => Size.Parse(arg)).ToArray();
			configuration.SplitDivideScope(TrimArg(Arguments[0]), sizes, Shapes);
			break;
		case "Comp":
			configuration.SplitComponent(TrimArg(Arguments[0]), Shapes[0]);
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