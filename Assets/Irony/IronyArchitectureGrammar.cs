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
		var NUMBER = new NumberLiteral("number", NumberOptions.AllowSign | NumberOptions.AllowLetterAfter);
		var NUMBER_RELATIVE = ToTerm("r");

		NonTerminal program = new NonTerminal("program"),
		ruleStatement = new NonTerminal("ruleStatement"),
		successorList = new NonTerminal("successorList"),
		successor = new NonTerminal("successor"),
		command = new NonTerminal("command"),
		argumentList = new NonTerminal("argumentList"),
		atom = new NonTerminal("atom"),
		commandBlock = new NonTerminal("commandBlock"),
		ruleList = new NonTerminal("ruleList");

		program.Rule = MakePlusRule(program, ruleStatement);
		
		ruleStatement.Rule = ID + ToTerm("::-") + successorList + ";";
		successorList.Rule = MakePlusRule(successorList, successor);
		successor.Rule = command | ID;
		command.Rule = ToTerm("[") | ToTerm("]") | ID + "(" + argumentList + ")" + commandBlock;
		argumentList.Rule = MakeStarRule(argumentList, ToTerm(","), atom);
		atom.Rule = NUMBER | NUMBER + NUMBER_RELATIVE | STRING;
		commandBlock.Rule = ToTerm("{") + ruleList + ToTerm("}") | Empty;
		ruleList.Rule = MakeStarRule(ruleList, ToTerm("|"), ID);

		this.Root = program;

		MarkTransient(ruleList, commandBlock);
		
		MarkPunctuation ("::-", ",", "(", ")", "{", "}", ";");
	}
}