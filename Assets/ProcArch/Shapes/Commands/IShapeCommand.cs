using System.Linq;

public interface IShapeCommand
{
	string Name { get; set; }

	string[] Arguments { get; set; }

	void Execute(IShapeConfiguration configuration);
}
