using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Size
{
	public Size(float value, bool isRelative)
		: this()
	{
		this.Value = value;
		this.IsRelative = isRelative;
	}

	public float Value { get; set; }

	public bool IsRelative { get; set; }

	public static Size Parse(string value)
	{
		if (value.EndsWith("r"))
		{
			return new Size(float.Parse(value), true);
		}
		else
		{
			return new Size(float.Parse(value), false);
		}
	}
}
