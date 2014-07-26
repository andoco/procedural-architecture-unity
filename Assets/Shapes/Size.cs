using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Structure for a size value that can be an absolute or relative value.
/// </summary>
public struct Size
{
	public Size(float value, bool isRelative)
		: this()
	{
		this.Value = value;
		this.IsRelative = isRelative;
	}

	public const string RelativeSuffix = "r";

	public float Value { get; set; }

	public bool IsRelative { get; set; }

	public static Size Parse(string value)
	{
		if (value.EndsWith(RelativeSuffix))
		{
			return new Size(float.Parse(value.TrimEnd(RelativeSuffix.ToArray())), true);
		}
		else
		{
			return new Size(float.Parse(value), false);
		}
	}
}
