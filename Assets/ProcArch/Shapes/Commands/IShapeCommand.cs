using System.Linq;

public interface IShapeCommand
{
	string Name { get; set; }

	Argument[] Arguments { get; set; }

	void Execute(IShapeConfiguration configuration);
}
