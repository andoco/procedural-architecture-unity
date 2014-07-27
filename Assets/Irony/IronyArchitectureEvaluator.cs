using UnityEngine;
using Irony.Parsing;
using System.Collections.Generic;
using System.Linq;

public class IronyArchitectureEvaluator
{
	private IShapeProductionSystem system;
	private ShapeRule currentRule;
	
	public IronyArchitectureEvaluator(IShapeProductionSystem system)
	{
		this.system = system;
	}

	public void Evaluate(ParseTreeNode root)
	{
		EvaluatePATree(root);
	}
	
	private void EvaluatePATree(ParseTreeNode node)
	{
		switch (node.Term.Name)
		{
		case "ruleStatement":
			Debug.Log("evaluating rule");
			EnterRule(node);
			break;
		case "successor":
			Debug.Log("evaluating successor");
			EnterSuccessor(node);
			break;
		}
		
		foreach (var child in node.ChildNodes)
			EvaluatePATree(child);
	}

	private void EnterRule(ParseTreeNode ruleNode)
	{
		var predecessor = ruleNode.FirstChild;
		var ruleSymbol = predecessor.FirstChild.Token.Text;

		var argNames = predecessor.ChildNodes.Count > 1
			? this.GetArgs(predecessor.ChildNodes[1])
			: new List<string>();

		this.currentRule = new ShapeRule();
		this.currentRule.Symbol = ruleSymbol;
		this.currentRule.ArgNames = argNames;
		this.system.Rules[this.currentRule.Symbol] = this.currentRule;
	}
		 
	private void EnterSuccessor(ParseTreeNode successorNode)
	{
		foreach (var child in successorNode.ChildNodes)
		{
			if (child.Term.Name == "ruleSymbol")
			{
				var symbolName = child.FirstChild.Token.Text;
				var symbolArgs = child.ChildNodes.Count > 1
					? this.GetArgs(child.ChildNodes[1])
					: new List<string>();

				var symbol = new ShapeSymbol(symbolName, symbolArgs);

				var successor = new SymbolShapeSuccessor
				{
					Symbol = symbol,
					Probability = 1f
				};

				this.currentRule.Successors.Add(successor);
			}
			else if (child.Term.Name == "command")
			{
				string cmdName;

				if (child.FirstChild.Term.Name == "simpleCmd")
				{
					cmdName = child.FirstChild.FirstChild.Token.Text;
				}
				else if (child.FirstChild.Term.Name == "scopeCmd")
				{
					cmdName = child.FirstChild.ChildNodes[2].Token.Text;
				}
				else
				{
					throw new System.ArgumentException(string.Format("Cannot evalute grammar command node: {0}", child.FirstChild));
				}

				var cmd = new ShapeCommand
				{
					Name = cmdName,
				};

				if (cmdName == "[")
				{
					cmd.Name = "Push";
				}
				else if (cmdName == "]")
				{
					cmd.Name = "Pop";
				}
				else
				{
					var cmdArgs = child.ChildNodes.Count > 1
						? this.GetArgs(child.ChildNodes[1])
						: new List<string>();
					cmd.Arguments = cmdArgs.ToArray();
	
					var shapes = new List<ShapeSymbol>();

					// Check if command block exists
					if (child.ChildNodes.Count > 2)
					{
						var ruleListNode = child.ChildNodes[2];

						foreach (var shapeNode in ruleListNode.ChildNodes)
						{
							var symbolName = shapeNode.FirstChild.Token.Text;
							var symbolArgs = shapeNode.ChildNodes.Count > 1
								? this.GetArgs(shapeNode.ChildNodes[1])
								: new List<string>();

							shapes.Add(new ShapeSymbol(symbolName, symbolArgs));
						}
					}

					cmd.Shapes = shapes.ToArray();
				}

				var cmdSuccessor = new CommandShapeSuccessor
				{
					Command = cmd
				};
				
				this.currentRule.Successors.Add(cmdSuccessor);
			}
		}
	}

	private IList<string> GetArgs(ParseTreeNode argsNode)
	{
		var args = new List<string>();
		
		foreach (var argAtomNode in argsNode.ChildNodes)
		{
			var argVal = argAtomNode.FirstChild.Token.Text;
			Debug.Log(string.Format("ARG: {0}", argVal));
			args.Add(argVal);
		}
		
		return args;
	}
}