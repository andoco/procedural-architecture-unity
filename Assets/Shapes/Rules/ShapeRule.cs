using System.Collections.Generic;
using System.Linq;

public sealed class ShapeRule
{
	public ShapeRule()
	{
		this.Successors = new List<IShapeSuccessor>();
	}

	public string Symbol { get; set; }

	public string GuardCondition { get; set; }

	public IList<IShapeSuccessor> Successors { get; private set; }

	public override string ToString ()
	{
		return string.Format ("[ShapeRule: Symbol={0}, GuardCondition={1}, Successors={2}]", Symbol, GuardCondition, FormatSuccessors());
	}
	
	private string FormatSuccessors()
	{
		return string.Format("[{0}]", string.Join(",", this.Successors.Select(x => x.ToString()).ToArray()));
	}
}