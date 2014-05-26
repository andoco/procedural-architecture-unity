using UnityEngine;

public enum ShapeStatus
{
	Active,
	Inactive
}

public class ShapeNodeValue
{
	public ShapeStatus Status { get; set; }

	public Matrix4x4 Matrix { get; set; }

	public ShapeRule Rule { get; set; }

	public Mesh Geometry { get; set; }
}