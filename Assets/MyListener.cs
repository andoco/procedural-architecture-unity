using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class MyListener : SimplePAGBaseListener
{
	private readonly ScopeDrawContext drawCtx;

	public MyListener(ScopeDrawContext drawCtx)
	{
		this.drawCtx = drawCtx;
	}

	public override void EnterCmdDefinition (SimplePAGParser.CmdDefinitionContext context)
	{
		base.EnterCmdDefinition (context);

		var cmdName = context.ID().GetText();
		var args = context.argumentsDefinition();

		Debug.Log(string.Format("Command: {0} {1}", cmdName, args.ToStringTree()));

		switch (cmdName)
		{
		case "Set":
			var shapeName = args.argumentDefinition(0).GetText().Trim('"');
			this.drawCtx.AddShape(shapeName);
			break;
		case "Trans":
			var axes = args.argumentDefinition().Select(x => float.Parse(x.floating_point().GetText())).ToArray();
			var delta = new Vector3(axes[0], axes[1], axes[2]);
			this.drawCtx.AddScope(delta, Quaternion.identity, Vector3.one);
			break;
		case "Rot":
			var rotAxes = args.argumentDefinition().Select(x => float.Parse(x.floating_point().GetText())).ToArray();
			this.drawCtx.AddScope(Vector3.zero, Quaternion.Euler(rotAxes[0], rotAxes[1], rotAxes[2]), Vector3.one);
			break;
		case "Scale":
			var scaleAxes = args.argumentDefinition().Select(x => float.Parse(x.floating_point().GetText())).ToArray();
			this.drawCtx.AddScope(Vector3.zero, Quaternion.identity, new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]));
			break;
		}
	}
}
