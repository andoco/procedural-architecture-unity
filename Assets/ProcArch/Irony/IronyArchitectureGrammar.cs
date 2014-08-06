using System.Text;
using Irony.Parsing;

public class IronyArchitectureGrammar : Grammar
{
	public const string IDName = "ID";
	public const string StringName = "STRING";
	public const string NumberName = "NUMBER";
	public const string VariableName = "VARIABLE";

	public const string ProgramName = "program";
	public const string AssignmentSectionName = "assignmentSection";
	public const string RuleSectionName = "ruleSection";
	public const string RuleStatementName = "ruleStatement";
	public const string PredecessorStatementName = "predecessorStatement";
	public const string PredecessorName = "predecessor";
	public const string SuccessorListName = "successorList";
	public const string ProbabilityName = "probability";
	public const string AssignmentStatementName = "assignmentStatement";
	public const string SuccessorName = "successor";
	public const string RuleSymbolName = "ruleSymbol";
	public const string RuleListName = "ruleList";
	public const string CommandName = "command";
	public const string CommandBlockName = "commandBlock";
	public const string SimpleCommandName = "simpleCmd";
	public const string ScopeCommandName = "scopeCmd";
	public const string AtomName = "atom";
	public const string ArgumentListName = "argumentList";
	public const string ArgName = "arg";
	public const string NamedArgName = "namedArg";

	public IronyArchitectureGrammar()
		: base(false)
	{
		var dot = ToTerm(".");
		var colon = ToTerm(":");
		var equal = ToTerm("=");
		var push = ToTerm("[");
		var pop = ToTerm("]");

		var ID = TerminalFactory.CreateCSharpIdentifier(IDName); // IdentifierTerminal?
		var STRING = new StringLiteral(StringName, "\"", StringOptions.AllowsAllEscapes);
		var NUMBER = new NumberLiteral(NumberName, NumberOptions.AllowSign);
		var VARIABLE = TerminalFactory.CreateCSharpIdentifier(VariableName);
		NUMBER.AddSuffix("r", System.TypeCode.Single);

		NonTerminal program = new NonTerminal(ProgramName),
		assignmentSection = new NonTerminal(AssignmentSectionName),
		assignmentStatement = new NonTerminal(AssignmentStatementName),
		ruleSection = new NonTerminal(RuleSectionName),
		ruleStatement = new NonTerminal(RuleStatementName),
		predecessorStatement = new NonTerminal(PredecessorStatementName),
		predecessor = new NonTerminal(PredecessorName),
		successorList = new NonTerminal(SuccessorListName),
		successor = new NonTerminal(SuccessorName),
		command = new NonTerminal(CommandName),
		argumentList = new NonTerminal(ArgumentListName),
		arg = new NonTerminal(ArgName),
		namedArg = new NonTerminal(NamedArgName),
		atom = new NonTerminal(AtomName),
		commandBlock = new NonTerminal(CommandBlockName),
		ruleList = new NonTerminal(RuleListName),
		ruleSymbol = new NonTerminal(RuleSymbolName),
		scopeCmd = new NonTerminal(ScopeCommandName),
		simpleCmd = new NonTerminal(SimpleCommandName),
		probability = new NonTerminal(ProbabilityName);

		program.Rule = (assignmentSection + ruleSection) | ruleSection;
		assignmentSection.Rule = MakePlusRule(assignmentSection, assignmentStatement);
		assignmentStatement.Rule = "let" + VARIABLE + equal + atom + ";";
		predecessorStatement.Rule = predecessor | Empty;
		predecessor.Rule = ID + "(" + argumentList + ")" | ID;
		ruleSection.Rule = MakePlusRule(ruleSection, ruleStatement);
		ruleStatement.Rule = predecessorStatement + ToTerm("::-") + successorList + probability + ";";
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