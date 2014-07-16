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

	public Volume Volume { get; set; }

	public ShapeRule Rule { get; set; }

	public string ShapeName { get; set; }

	public override string ToString ()
	{
		var matrixInfo = string.Format("Matrix=(pos={0}, rot={1}, scale={2})", Matrix.GetPosition(), Matrix.GetRotation(), Matrix.GetScale());

		if (Rule != null)
			return string.Format ("[ShapeNodeValue: Status={0}, Rule={1}, {2}]", Status, Rule, matrixInfo);
		else
			return string.Format ("[ShapeNodeValue: Status={0}, ShapeName={1}, {2}]", Status, ShapeName, matrixInfo);
	}
}