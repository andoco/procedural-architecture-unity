using System.Text;
using UnityEngine;
using Irony.Parsing;

public class IronyArchitectureGrammar : Grammar
{
	public IronyArchitectureGrammar()
		: base(false)
	{
		var dot = ToTerm(".");
		var colon = ToTerm(":");
		var equal = ToTerm("=");
		var push = ToTerm("[");
		var pop = ToTerm("]");

		var ID = TerminalFactory.CreateCSharpIdentifier("ID"); // IdentifierTerminal?
		var STRING = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
		var NUMBER = new NumberLiteral("number", NumberOptions.AllowSign);
		var VARIABLE = TerminalFactory.CreateCSharpIdentifier("Variable");
		NUMBER.AddSuffix(Size.RelativeSuffix, System.TypeCode.Single);

		NonTerminal program = new NonTerminal("program"),
		assignmentSection = new NonTerminal("assignmentSection"),
		assignmentStatement = new NonTerminal("assignmentStatement"),
		ruleSection = new NonTerminal("ruleSection"),
		ruleStatement = new NonTerminal("ruleStatement"),
		predecessor = new NonTerminal("predecessor"),
		successorList = new NonTerminal("successorList"),
		successor = new NonTerminal("successor"),
		command = new NonTerminal("command"),
		argumentList = new NonTerminal("argumentList"),
		arg = new NonTerminal("arg"),
		namedArg = new NonTerminal("namedArg"),
		atom = new NonTerminal("atom"),
		commandBlock = new NonTerminal("commandBlock"),
		ruleList = new NonTerminal("ruleList"),
		ruleSymbol = new NonTerminal("ruleSymbol"),
		scopeCmd = new NonTerminal("scopeCmd"),
		simpleCmd = new NonTerminal("simpleCmd");

		program.Rule = assignmentSection + ruleSection;
		assignmentSection.Rule = MakePlusRule(assignmentSection, assignmentStatement);
		assignmentStatement.Rule = "let" + VARIABLE + equal + atom + ";";
		predecessor.Rule = ID + "(" + argumentList + ")" | ID;
		ruleSection.Rule = MakePlusRule(ruleSection, ruleStatement);
		ruleStatement.Rule = predecessor + ToTerm("::-") + successorList + ";";
		successorList.Rule = MakePlusRule(successorList, successor);
		successor.Rule = command | ruleSymbol;
		scopeCmd.Rule = "Scope" + dot + ID;
		simpleCmd.Rule = push | pop;
		command.Rule = simpleCmd | scopeCmd + "(" + argumentList + ")" + commandBlock;
		argumentList.Rule = MakeStarRule(argumentList, ToTerm(","), arg);
		arg.Rule = atom | namedArg;
		namedArg.Rule = VARIABLE + colon + atom;
		atom.Rule = NUMBER | STRING | VARIABLE;
		commandBlock.Rule = ToTerm("{") + ruleList + ToTerm("}") | Empty;
		ruleSymbol.Rule = ID + "(" + argumentList + ")" | ID;
		ruleList.Rule = MakeStarRule(ruleList, ToTerm("|"), ruleSymbol);

		this.Root = program;

		MarkTransient(ruleList, commandBlock);

		MarkPunctuation ("::-", ",", "(", ")", "{", "}", ";", "=", "let");
	}
}