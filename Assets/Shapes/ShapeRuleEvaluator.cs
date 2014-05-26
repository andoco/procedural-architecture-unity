public class ShapeRuleEvaluator
{
	private readonly IShapeRuleGuardEvaluator guardEvaluator;
	private readonly ShapeProductionSystem system;

	public ShapeRuleEvaluator(IShapeRuleGuardEvaluator guardEvaluator, ShapeProductionSystem system)
	{
		this.system = system;
				this.guardEvaluator = guardEvaluator;
	}

	public void Evaluate(ShapeRule rule)
	{
		if (this.guardEvaluator.Evaluate(rule.GuardCondition))
		{
			foreach (var successor in rule.Successors)
			{

			}
		}
	}
}

