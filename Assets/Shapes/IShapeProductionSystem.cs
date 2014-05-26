using System;
using System.Collections.Generic;

public interface IShapeProductionSystem
{
	IDictionary<string, ShapeRule> Rules { get; }

	string Axiom { get; set; }

	void Run();
}
