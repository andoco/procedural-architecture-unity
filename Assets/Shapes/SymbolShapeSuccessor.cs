
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
