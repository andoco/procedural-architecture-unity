namespace Andoco.Unity.Framework.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using UnityEngine;

	public static class EnumerableExtensions
	{
        /// <summary>
        /// Finds the point <paramref name="points"/> that is closest to <paramref name="pos"/>.
        /// </summary>
        /// <returns>The closest point.</returns>
        /// <param name="points">Points.</param>
        /// <param name="pos">Position.</param>
		public static Vector3 FindClosestPoint(this IEnumerable<Vector3> points, Vector3 pos)
		{
			Vector3? closest = null;
			var closestDist = float.MaxValue;
			foreach (var p in points)
			{
				var d = Vector3.Distance(pos, p);
				if (d < closestDist)
				{
					closest = p;
					closestDist = d;
				}
			}

			return closest.Value;
		}

        /// <summary>
        /// Returns the incremented index of <paramref name="currentIndex"/> or 0 if the end is reached.
        /// </summary>
        /// <returns>The incremented index.</returns>
        /// <param name="collection">Collection.</param>
        /// <param name="currentIndex">Current index.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static int WrapIncrementedIndex<T>(this IEnumerable<T> collection, int currentIndex)
		{
			return currentIndex < collection.Count() - 1 ? currentIndex + 1 : 0;
		}

        /// <summary>
        /// Returns the decremented index of <paramref name="currentIndex"/> or the end index if the start is reached.
        /// </summary>
        /// <returns>The decremented index.</returns>
        /// <param name="collection">Collection.</param>
        /// <param name="currentIndex">Current index.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static int WrapDecrementedIndex<T>(this IEnumerable<T> collection, int currentIndex)
		{
			return currentIndex == 0 ? collection.Count() - 1 : currentIndex - 1;
		}
	}
}