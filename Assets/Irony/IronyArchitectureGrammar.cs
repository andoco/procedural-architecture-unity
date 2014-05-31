using System.Text;
using UnityEngine;
using Irony.Parsing;

public class PAGrammar : Grammar
{
	public PAGrammar()
		: base(false)
	{
		var ID = TerminalFactory.CreateCSharpIdentifier("ID"); // IdentifierTerminal?
		var STRING = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
		var NUMBER = new NumberLiteral("number", NumberOptions.AllowSign);

		NonTerminal program = new NonTerminal("program"),
		ruleStatement = new NonTerminal("ruleStatement"),
		successorList = new NonTerminal("successorList"),
		successor = new NonTerminal("successor"),
		command = new NonTerminal("command"),
		argumentList = new NonTerminal("argumentList"),
		atom = new NonTerminal("atom");
		
		program.Rule = MakePlusRule(program, ruleStatement);
		
		ruleStatement.Rule = ID + ToTerm("::-") + successorList + ";";
		successorList.Rule = MakePlusRule(successorList, successor);
		successor.Rule = command | ID;
		command.Rule = ToTerm("[") | ToTerm("]") | ID + "(" + argumentList + ")";
		argumentList.Rule = MakeStarRule(argumentList, ToTerm(","), atom);
		atom.Rule = NUMBER | STRING;
		
		this.Root = program;
		
		MarkPunctuation ("::-", ",", "(", ")", ";");
	}
}