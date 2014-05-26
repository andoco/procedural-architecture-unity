public interface IShapeCommand
{
	string Name { get; set; }

	string[] Arguments { get; set; }

	void Execute();
}

public class ShapeCommand : IShapeCommand
{
	public string Name { get; set; }
	
	public string[] Arguments { get; set; }

	public void Execute()
	{
	}
}