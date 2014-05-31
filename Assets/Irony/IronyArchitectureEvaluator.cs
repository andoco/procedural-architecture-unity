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
//        Debug.Log(node);
		
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
		this.currentRule = new ShapeRule();
		this.currentRule.Symbol = ruleNode.FirstChild.Token.Text;
		this.system.Rules[this.currentRule.Symbol] = this.currentRule;
	}
	
	private void EnterSuccessor(ParseTreeNode successorNode)
	{
		foreach (var child in successorNode.ChildNodes)
		{
			if (child.Term.Name == "ID")
			{
				var successor = new SymbolShapeSuccessor
				{
					Symbol = child.Token.Text,
					Probability = 1f
				};
				this.currentRule.Successors.Add(successor);
			}
			else if (child.Term.Name == "command")
			{
				var cmdName = child.FirstChild.Token.Text;
				Debug.Log(cmdName);

				var args = new List<string>();

				if (cmdName == "[")
				{
					cmdName = "Push";
				}
				else if (cmdName == "]")
				{
					cmdName = "Pop";
				}
				else
				{
					var argsNode = child.ChildNodes[1];
					
					foreach (var argAtomNode in argsNode.ChildNodes)
					{
						Debug.Log(string.Format("ARG: {0}", argAtomNode.FirstChild.Token.Text));
						args.Add(argAtomNode.FirstChild.Token.Text);
					}
				}

				var cmd = new ShapeCommand
				{
					Name = cmdName,
					Arguments = args.ToArray()
				};
				
				var cmdSuccessor = new CommandShapeSuccessor
				{
					Command = cmd
				};
				
				this.currentRule.Successors.Add(cmdSuccessor);
			}
		}
	}
}