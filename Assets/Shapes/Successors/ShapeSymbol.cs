using System.Collections.Generic;
using System.Linq;

public class ShapeSymbol
{
	public ShapeSymbol(string name, IEnumerable<string> args)
	{
		this.Name = name;
		this.UnresolvedArgs = args.ToArray();
	}

	public string Name { get; private set; }

	public string[] UnresolvedArgs { get; private set; }

	public override string ToString ()
	{
		return string.Format ("[ShapeSymbol: Name={0}, UnresolvedArgs={1}]", Name, FormatArgs());
	}

	private string FormatArgs()
	{
		return string.Format("[{0}]", string.Join(",", this.UnresolvedArgs));
	}
}
