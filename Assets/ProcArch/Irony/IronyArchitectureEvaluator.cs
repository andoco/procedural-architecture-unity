using UnityEngine;
using Irony.Parsing;
using System;
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
		case IronyArchitectureGrammar.AssignmentStatementName:
			EnterAssignment(node);
			break;
		case IronyArchitectureGrammar.PredecessorName:
			EnterPredecessor(node);
			break;
		case IronyArchitectureGrammar.SuccessorListName:
			EnterSuccessorList(node);
			break;
		case IronyArchitectureGrammar.SuccessorName:
			EnterSuccessor(node);
			break;
		case IronyArchitectureGrammar.ProbabilityName:
			EnterProbability(node);
			break;
		}
		
		foreach (var child in node.ChildNodes)
			EvaluatePATree(child);
	}

	private void EnterAssignment(ParseTreeNode assignmentNode)
	{
		var argName = assignmentNode.ChildNodes[0].Token.Text;
		var argVal = assignmentNode.ChildNodes[1].FirstChild.Token.Text;

		this.system.DefaultArgs[argName] = argVal;
	}

	private void EnterPredecessor(ParseTreeNode node)
	{
		var idNode = node.FirstChild;
		
		var ruleSymbol = idNode.Token.Text;
		
		var argNames = node.ChildNodes.Count > 1
			? this.GetArgs(node.ChildNodes[1]).Select(x => x.Value).ToList()
			: new List<string>();
		
		this.currentRule = new ShapeRule();
		this.currentRule.Symbol = ruleSymbol;
		this.currentRule.ArgNames = argNames;
		this.system.Rules[this.currentRule.Symbol] = this.currentRule;
	}

	private void EnterSuccessorList(ParseTreeNode node)
	{
		this.currentRule.Successors.Add(new SuccessorList());
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
					: new List<Argument>();

				var symbol = new ShapeSymbol(symbolName, symbolArgs);

				var successor = new SymbolShapeSuccessor
				{
					Symbol = symbol,
					Probability = 1f
				};

				this.currentRule.Successors.Last().Successors.Add(successor);
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
						: new List<Argument>();
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
								: new List<Argument>();

							shapes.Add(new ShapeSymbol(symbolName, symbolArgs));
						}
					}

					cmd.Shapes = shapes.ToArray();
				}

				var cmdSuccessor = new CommandShapeSuccessor
				{
					Command = cmd
				};
				
				this.currentRule.Successors.Last().Successors.Add(cmdSuccessor);
			}
		}
	}

	private void EnterProbability(ParseTreeNode node)
	{
		if (node.ChildNodes.Any())
		{
			var probability = float.Parse(node.FirstChild.Token.Text);
			this.currentRule.Successors.Last().Probability = probability;
		}
		else
		{
			this.currentRule.Successors.Last().Probability = 1f;
		}
	}

	private IList<Argument> GetArgs(ParseTreeNode argsNode)
	{
		var args = new List<Argument>();
		
		foreach (var argNode in argsNode.ChildNodes)
		{
			Argument arg;

			switch (argNode.FirstChild.Term.Name)
			{
			case "atom":
				arg = new Argument(argNode.FirstChild.FirstChild.Token.Text);
				break;
			case "namedArg":
				arg = new Argument(argNode.FirstChild.ChildNodes[0].Token.Text, argNode.FirstChild.ChildNodes[1].ChildNodes[0].Token.Text);
				break;
			default:
				throw new System.InvalidOperationException(string.Format("Unknown arg type: {0}", argNode.Term.Name));
			}
//			var argVal = argAtomNode.FirstChild.Token.Text;
//			Debug.Log(string.Format("ARG: {0}", argVal));
//			args.Add(argVal);

			Debug.Log(string.Format("ARG: {0}", arg));

			args.Add(arg);
		}
		
		return args;
	}
}