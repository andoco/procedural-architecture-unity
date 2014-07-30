namespace Andoco.Unity.Framework.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using UnityEngine;

	public static class ListExtensions
	{
	    /// <summary>
	    /// Calculates the entire distance along a path of points.
	    /// </summary>
	    /// <returns>The total distance.</returns>
	    /// <param name="points">Path of points.</param>
	    public static float CalculateDistance(this IList<Vector3> points)
	    {
	        var dist = 0f;

	        for (int i=0; i < points.Count - 1; i++)
	            dist += Vector3.Distance(points[i], points[i+1]);
	        
	        return dist;
	    }
	}
}