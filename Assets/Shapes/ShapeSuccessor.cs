public enum ShapeSuccessorKind
{
	Command,
	Symbol
}

public interface IShapeSuccessor
{
	ShapeSuccessorKind Kind { get; }
}

public sealed class SymbolShapeSuccessor : IShapeSuccessor
{
	public ShapeSuccessorKind Kind { get { return ShapeSuccessorKind.Symbol; } }

	public string Symbol { get; set; }

	public float Probability { get; set; }

	public override string ToString ()
	{
		return string.Format ("[SymbolShapeSuccessor: Symbol={0}, Probability={1}]", Symbol, Probability);
	}
}

public sealed class CommandShapeSuccessor : IShapeSuccessor
{
	public ShapeSuccessorKind Kind { get { return ShapeSuccessorKind.Command; } }
	
	public ShapeCommand Command { get; set; }

	public override string ToString ()
	{
		return string.Format ("[CommandShapeSuccessor: Command={0}]", Command.Name);
	}
}