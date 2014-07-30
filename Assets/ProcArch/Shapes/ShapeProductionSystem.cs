using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeProductionSystem : IShapeProductionSystem
{
	private int counter;

	public ShapeProductionSystem()
	{
		this.Rules = new Dictionary<string, ShapeRule>();
	}
	
	public IDictionary<string, ShapeRule> Rules { get; private set; }

	public string Axiom { get; set; }

	public void Run(IShapeConfiguration configuration, IList<string> args)
	{
		if (string.IsNullOrEmpty(this.Axiom))
			throw new InvalidOperationException("The axiom symbol has not been set");

		configuration.AddRule(this.Rules[this.Axiom], args);
		var currentNode = configuration.RootNode;

		while (currentNode != null)
		{
			Debug.Log(string.Format("EVALUTE: {0}", currentNode.Value));

			// IMPORTANT: The scope stack must be reset when processing a new node so that
			// push/pop commands only apply to rule of the current node.
			//this.configuration.SetScope(new Scope(currentNode.Value.Matrix));
			configuration.CurrentNode = currentNode;

			var currentRule = currentNode.Value.Rule;

			if (currentRule != null)
			{
				foreach (var successor in currentRule.Successors)
				{
					if (successor is CommandShapeSuccessor)
					{
						var cmdSuccessor = (CommandShapeSuccessor)successor;
						cmdSuccessor.Command.Execute(configuration);
					}
					else if (successor is SymbolShapeSuccessor)
					{
						var symbolSuccessor = (SymbolShapeSuccessor)successor;
						var resolvedArgs = configuration.ResolveArgs(symbolSuccessor.Symbol.UnresolvedArgs);
						var rule = this.Rules[symbolSuccessor.Symbol.Name];
						configuration.AddRule(rule, resolvedArgs);
					}
					else
					{
						throw new InvalidOperationException(string.Format("Unknown successor type ", successor));
					}
				}
			}

			// Mark current node as Inactive and pick next one.
			currentNode.Value.Status = ShapeStatus.Inactive;
			currentNode = PickNextNode(configuration.RootNode);
		}
	}

	private ShapeNode PickNextNode(ShapeNode root)
	{
		// TODO: Could auto-set geometry leaf nodes to Inactive.
		Predicate<TreeNode> pred = (TreeNode node) => {
			return ((ShapeNode)node).Value.Status == ShapeStatus.Active;
		};

		return root.BreadthFirstSearch(pred) as ShapeNode;
	}
}