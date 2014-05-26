using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeProductionSystem
{
	private int counter;

	public ShapeProductionSystem()
	{
		this.Rules = new Dictionary<string, ShapeRule>();
	}

	public IDictionary<string, ShapeRule> Rules { get; private set; }

	public TreeNode<ShapeNodeValue> Run(string axiomSymbol)
	{
		var tree = new TreeNode<ShapeNodeValue>(this.NextId(), null);
		tree.Value = new ShapeNodeValue
		{
			Status = ShapeStatus.Active,
			Rule = this.Rules[axiomSymbol],
			Matrix = Matrix4x4.identity
		};

		var currentNode = tree;

		while (currentNode != null)
		{
			var currentRule = currentNode.Value.Rule;
			Debug.Log(string.Format("EVALUTE: {0}", currentRule));

			if (currentRule != null)
			{
				foreach (var successor in currentRule.Successors)
				{
					if (successor is CommandShapeSuccessor)
					{
						// Run the command.
						// Scope commands will operate on the current scope.
						// Commands that add geometry will add them to new leaf nodes.
						var cmdSuccessor = (CommandShapeSuccessor)successor;
						cmdSuccessor.Command.Execute(currentNode);
					}
					else if (successor is SymbolShapeSuccessor)
					{
						var symbolSuccessor = (SymbolShapeSuccessor)successor;
						
						// Create new node for the successor rule with a copy of the current scope.
						var nodeValue = new ShapeNodeValue
						{
							Status = ShapeStatus.Active,
							Rule = this.Rules[symbolSuccessor.Symbol],
							Matrix = currentNode.Value.Matrix
						};
						
						var node = new TreeNode<ShapeNodeValue>(NextId(), currentNode) { Value = nodeValue };
						
						currentNode.Add(node);
					}
					else
					{
						throw new InvalidOperationException(string.Format("Unknown successor type ", successor));
					}
				}
			}

			// Mark current node as Inactive and pick next one.
			currentNode.Value.Status = ShapeStatus.Inactive;
			currentNode = PickNextNode(tree, currentNode);
		}

//		tree.TraverseBreadthFirst(node => Debug.Log(node.Value));

		return tree;
	}

	private TreeNode<ShapeNodeValue> PickNextNode(TreeNode<ShapeNodeValue> root, TreeNode<ShapeNodeValue> currentNode)
	{
		return root.BreadthFirstSearch(x => x.Status == ShapeStatus.Active);
	}
	
	private string NextId()
	{
		return (this.counter++).ToString();
	}
}