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
		this.Matrix = Matrix4x4.identity;
	}

	public ShapeStatus Status { get; set; }

	public Matrix4x4 Matrix { get; set; }

	public ShapeRule Rule { get; set; }

	public string ShapeName { get; set; }

	public override string ToString ()
	{
		if (Rule != null)
			return string.Format ("[ShapeNodeValue: Status={0}, Rule={1}]", Status, Rule);
		else
			return string.Format ("[ShapeNodeValue: Status={0}, ShapeName={1}]", Status, ShapeName);
	}
}