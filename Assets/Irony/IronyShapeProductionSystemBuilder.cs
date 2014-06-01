using System;
using System.Text;
using UnityEngine;
using Irony.Parsing;

public class IronyShapeProductionSystemBuilder : IShapeProductionSystemBuilder
{
	public IShapeProductionSystem Build(string source)
	{
		var system = new ShapeProductionSystem();
		var evaluator = new IronyArchitectureEvaluator(system);
		var grammar = new PAGrammar();
		var root = GetRoot(source, grammar);

		var sb = new StringBuilder();
		DispTree(root, 0, sb);
		Debug.Log(sb.ToString());

		evaluator.Evaluate(root);

		return system;
	}

	private static ParseTreeNode GetRoot(string sourceCode, Grammar grammar)
	{
		LanguageData language = new LanguageData (grammar);
		Parser parser = new Parser (language);
		ParseTree parseTree = parser.Parse (sourceCode);
		ParseTreeNode root = parseTree.Root;

		if (language.Errors.Count > 0)
		{
			foreach (var error in language.Errors)
				Debug.Log(string.Format("GRAMMAR: {0}", error.Message));
		}

		if (parseTree.HasErrors())
		{
			foreach (var msg in parseTree.ParserMessages)
			{
				Debug.Log(string.Format("PARSER: {0}", msg.Message));
			}
		}
		
		if (root == null)
			throw new InvalidOperationException("Failed to parse tree for source");
		
		return root;
	}
	
	private static void DispTree (ParseTreeNode node, int level, StringBuilder sb)
	{
		for (int i = 0; i < level; i++)
			sb.Append ("  ");
		sb.AppendLine (node.ToString ());
		
		foreach (ParseTreeNode child in node.ChildNodes)
			DispTree (child, level + 1, sb);
		
	}
}