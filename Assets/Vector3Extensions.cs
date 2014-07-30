using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Vector3Extensions
{
	/// <summary>
	/// E.g. (1,2,3) => (2,3,1)
	/// </summary>
	public static Vector3 ToYZX(this Vector3 v)
	{
		return new Vector3(v.y, v.z, v.x);
	}

	/// <summary>
	/// E.g. (1,2,3) => (3,1,2)
	/// </summary>
	public static Vector3 ToZXY(this Vector3 v)
	{
		return new Vector3(v.z, v.x, v.y);
	}

	/// <summary>
	/// E.g. (1,2,3) => (1,3,2)
	/// </summary>
	public static Vector3 ToXZY(this Vector3 v)
	{
		return new Vector3(v.x, v.z, v.y);
	}

//	public static Vector3 SwapXZ(this Vector3 v)
//	{
//		return new Vector3(v.z, v.y, v.x);
//	}
//
//	public static Vector3 Swap(this Vector3 v)
}
