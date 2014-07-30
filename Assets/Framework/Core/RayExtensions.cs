namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

	public static class RayExtensions
	{
		public static Vector3? GetHitPoint(this Ray ray)
		{
			return ray.GetHitPoint(LayerMaskConstants.LayerMaskAll);
		}

		public static Vector3? GetHitPoint(this Ray ray, int layerMask)
		{
			RaycastHit hit;
			Vector3? p = null;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				p = hit.point;
			}
			
			return p;
		}
	}
}