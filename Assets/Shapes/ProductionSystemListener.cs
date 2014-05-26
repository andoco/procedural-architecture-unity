using System.Collections.Generic;
using System.Linq;

public class ProductionSystemListener : SimplePAGBaseListener
{
	private ShapeProductionSystem system;
	private ShapeRule currentRule;

	public ProductionSystemListener(ShapeProductionSystem system)
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

		if (symbolCtx != null)
		{
			var successor = new SymbolShapeSuccessor
			{
				Symbol = symbolCtx.GetText(),
				Probability = 1f
			};
			this.currentRule.Successors.Add(successor);
		}
		else
		{
			var cmdCtx = context.cmdDefinition();
			var cmdName = cmdCtx.ID().GetText();
			var args = cmdCtx.argumentsDefinition();

			var cmd = new ShapeCommand
			{
				Name = cmdName,
				Arguments = args.argumentDefinition().Select(x => x.GetText()).ToArray()
			};
                
			var cmdSuccessor = new CommandShapeSuccessor
			{
				Command = cmd
			};

			this.currentRule.Successors.Add(cmdSuccessor);
        }
	}
}