using UnityEngine;
using System.Collections.Generic;

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

	public IList<Argument> Args { get; set; }

	public override string ToString ()
	{
		return string.Format ("[ShapeNodeValue: Status={0}, Rule={1}, Transform={2}]", Status, Rule, Transform);
	}
}