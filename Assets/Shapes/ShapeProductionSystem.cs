using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeProductionSystem
{
	private int counter;

	private readonly IShapeConfiguration configuration;

	public ShapeProductionSystem(IShapeConfiguration configuration)
	{
		this.configuration = configuration;
		this.Rules = new Dictionary<string, ShapeRule>();
	}
	
	public IDictionary<string, ShapeRule> Rules { get; private set; }

	public string Axiom { get; set; }

	public void Run()
	{
		if (string.IsNullOrEmpty(this.Axiom))
			throw new InvalidOperationException("The axiom symbol has not been set");

		this.configuration.AddRule(this.Rules[this.Axiom]);
		var currentNode = this.configuration.CurrentNode;

		while (currentNode != null)
		{
			Debug.Log(string.Format("EVALUTE: {0}", currentNode.Value));
			var currentRule = currentNode.Value.Rule;

			if (currentRule != null)
			{
				foreach (var successor in currentRule.Successors)
				{
					if (successor is CommandShapeSuccessor)
					{
						var cmdSuccessor = (CommandShapeSuccessor)successor;
						cmdSuccessor.Command.Execute(this.configuration);
					}
					else if (successor is SymbolShapeSuccessor)
					{
						var symbolSuccessor = (SymbolShapeSuccessor)successor;
						this.configuration.AddRule(this.Rules[symbolSuccessor.Symbol]);
					}
					else
					{
						throw new InvalidOperationException(string.Format("Unknown successor type ", successor));
					}
				}
			}

			// Mark current node as Inactive and pick next one.
			currentNode.Value.Status = ShapeStatus.Inactive;
			currentNode = PickNextNode(this.configuration.RootNode);
		}
	}

	private TreeNode<ShapeNodeValue> PickNextNode(TreeNode<ShapeNodeValue> root)
	{
		return root.BreadthFirstSearch(x => x.Status == ShapeStatus.Active);
	}
}