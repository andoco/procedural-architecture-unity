
public sealed class CommandShapeSuccessor : IShapeSuccessor
{
	public ShapeSuccessorKind Kind { get { return ShapeSuccessorKind.Command; } }
	
	public ShapeCommand Command { get; set; }

	public override string ToString ()
	{
		return string.Format ("[CommandShapeSuccessor: Command={0}]", Command.Name);
	}
}