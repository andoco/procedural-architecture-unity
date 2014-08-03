using System;
using System.Collections.Generic;

public interface IShapeProductionSystem
{
	IDictionary<string, ShapeRule> Rules { get; }

	IDictionary<string, string> DefaultArgs { get; }

	string Axiom { get; set; }

	void Run(IShapeConfiguration configuration, IList<string> rootArgs, IDictionary<string, string> globalArgs);
}
