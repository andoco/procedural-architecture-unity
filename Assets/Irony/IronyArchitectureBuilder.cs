using System.Text;
using UnityEngine;
using Irony.Parsing;

public class IronyArchitectureBuilder
{
	public void Build(string source, IShapeProductionSystem system)
	{
		var evaluator = new IronyArchitectureEvaluator(system);
		var g = new PAGrammar();
		var root = GetRoot(source, g);

		Debug.Log(IsValid(source, new PAGrammar()));

		var sb = new StringBuilder();
		DispTree(root, 0, sb);
		Debug.Log(sb.ToString());

		evaluator.Evaluate(root);
	}

	public static bool IsValid (string sourceCode, Grammar grammar)
	{
		Debug.Log(string.Format("Validating {0} for {1}", sourceCode, grammar));
		LanguageData language = new LanguageData (grammar);
		
		Parser parser = new Parser (language);
		
		ParseTree parseTree = parser.Parse (sourceCode);
		
		ParseTreeNode root = parseTree.Root;
		
		return root != null;
	}

	public static void DispTree (ParseTreeNode node, int level, StringBuilder sb)
	{
		for (int i = 0; i < level; i++)
			sb.Append ("  ");
		sb.AppendLine (node.ToString ());
		
		foreach (ParseTreeNode child in node.ChildNodes)
			DispTree (child, level + 1, sb);
		
	}

	public static ParseTreeNode GetRoot(string sourceCode, Grammar grammar)
	{
		
		LanguageData language = new LanguageData (grammar);
		
		Parser parser = new Parser (language);
		
		ParseTree parseTree = parser.Parse (sourceCode);
		
		ParseTreeNode root = parseTree.Root;
		
		return root;
		
	}

}