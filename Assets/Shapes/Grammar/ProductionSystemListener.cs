using System.Collections.Generic;
using System.Linq;

public class ProductionSystemListener : SimplePAGBaseListener
{
	private IShapeProductionSystem system;
	private ShapeRule currentRule;

	public ProductionSystemListener(IShapeProductionSystem system)
	{
		this.system = system;
	}

	public override void EnterRule(SimplePAGParser.RuleContext context)
	{
		this.currentRule = new ShapeRule();
		this.currentRule.Symbol = context.ID().GetText();
		this.system.Rules[this.currentRule.Symbol] = this.currentRule;
	}

	public override void EnterSuccessor(SimplePAGParser.SuccessorContext context)
	{
		var symbolCtx = context.ID();
		var cmdCtx = context.cmdDefinition();

		if (symbolCtx != null)
		{
			var successor = new SymbolShapeSuccessor
			{
				Symbol = symbolCtx.GetText(),
				Probability = 1f
			};
			this.currentRule.Successors.Add(successor);
		}
		else if (cmdCtx != null)
		{
			string cmdName = null;
			var pushPopCtx = cmdCtx.pushPopScope();

			if (pushPopCtx != null)
			{
				switch (pushPopCtx.GetText())
				{
				case "[":
					cmdName = "Push";
					break;
				case "]":
					cmdName = "Pop";
					break;
				}
			}
			else
			{
				cmdName = cmdCtx.ID().GetText();
			}
				
			var argsDef = cmdCtx.argumentsDefinition();
			var args = argsDef == null ? new string[0] : argsDef.argumentDefinition().Select(x => x.GetText()).ToArray();
			
			var cmd = new ShapeCommand
			{
				Name = cmdName,
				Arguments = args
			};
			
			var cmdSuccessor = new CommandShapeSuccessor
			{
				Command = cmd
			};
			
			this.currentRule.Successors.Add(cmdSuccessor);
        }
	}
}