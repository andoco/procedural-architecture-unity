using System.Text;
using Irony.Parsing;

public class IronyArchitectureGrammar : Grammar
{
	public const string RuleStatementName = "ruleStatement";
	public const string PredecessorName = "predecessor";
	public const string SuccessorListName = "successorList";
	public const string ProbabilityName = "probability";
	public const string AssignmentStatementName = "assignmentStatement";
	public const string SuccessorName = "successor";

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
		NUMBER.AddSuffix("r", System.TypeCode.Single);

		NonTerminal program = new NonTerminal("program"),
		assignmentSection = new NonTerminal("assignmentSection"),
		assignmentStatement = new NonTerminal(AssignmentStatementName),
		ruleSection = new NonTerminal("ruleSection"),
		ruleStatement = new NonTerminal(RuleStatementName),
		predecessor = new NonTerminal(PredecessorName),
		successorList = new NonTerminal(SuccessorListName),
		successor = new NonTerminal(SuccessorName),
		command = new NonTerminal("command"),
		argumentList = new NonTerminal("argumentList"),
		arg = new NonTerminal("arg"),
		namedArg = new NonTerminal("namedArg"),
		atom = new NonTerminal("atom"),
		commandBlock = new NonTerminal("commandBlock"),
		ruleList = new NonTerminal("ruleList"),
		ruleSymbol = new NonTerminal("ruleSymbol"),
		scopeCmd = new NonTerminal("scopeCmd"),
		simpleCmd = new NonTerminal("simpleCmd"),
		probability = new NonTerminal(ProbabilityName);

		program.Rule = (assignmentSection + ruleSection) | ruleSection;
		assignmentSection.Rule = MakePlusRule(assignmentSection, assignmentStatement);
		assignmentStatement.Rule = "let" + VARIABLE + equal + atom + ";";
		predecessor.Rule = ID + "(" + argumentList + ")" | ID;
		ruleSection.Rule = MakePlusRule(ruleSection, ruleStatement);
		ruleStatement.Rule = ((predecessor + ToTerm("::-")) | (ToTerm("::-"))) + successorList + probability + ";";
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
		probability.Rule = Empty | (colon + NUMBER);

		this.Root = program;

		MarkTransient(ruleList, commandBlock);

		MarkPunctuation ("::-", ",", "(", ")", "{", "}", ";", ":", "=", "let", ".");
	}
}