namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;

	public static class BoundsExtensions
	{
		public static bool ContainsVertically(this Bounds bounds, Vector3 point)
		{
			point.y = bounds.center.y;
			return bounds.Contains(point);
		}

		public static Vector3 RandomPoint(this Bounds bounds)
		{
			var x = Mathf.Lerp(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x, Random.value);
			var z = Mathf.Lerp(bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z, Random.value);

			return new Vector3(x, bounds.center.y, z);
		}
	}
}