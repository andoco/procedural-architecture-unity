using UnityEngine;

public enum ShapeStatus
{
	Active,
	Inactive
}

public class ShapeNodeValue
{
	public ShapeNodeValue()
	{
		this.Status = ShapeStatus.Active;
	}

	public ShapeStatus Status { get; set; }

	public SimpleTransform Transform { get; set; }

	public Volume Volume { get; set; }

	public ShapeRule Rule { get; set; }

	public string ShapeName { get; set; }

	public override string ToString ()
	{
		if (Rule != null)
			return string.Format ("[ShapeNodeValue: Status={0}, Rule={1}, {2}]", Status, Rule, Transform);
		else
			return string.Format ("[ShapeNodeValue: Status={0}, ShapeName={1}, {2}]", Status, ShapeName, Transform);
	}
}