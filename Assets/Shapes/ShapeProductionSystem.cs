using System.Collections.Generic;

public class ShapeProductionSystem
{
	public ShapeProductionSystem()
	{
		this.Rules = new Dictionary<string, ShapeRule>();
	}

	public IDictionary<string, ShapeRule> Rules { get; private set; }

	public void Run()
	{
	}
}