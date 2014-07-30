namespace Andoco.Unity.Framework.Core
{
	using System;
    using System.Collections.Generic;
    using System.Linq;
	using UnityEngine;

	public static class Vector3Extensions
	{
//		public static Vector3 CalculateProjectileTargetPos(this Vector3 origin, Vector3 projectileVelocity, Vector3 targetPos, Vector3 targetVelocity)
//		{
//			Vector3 totarget = targetPos - origin;
//			
//			//float a = Vector3.Dot(targetVelocity, targetVelocity) - (projectileVelocity * projectileVelocity);
//			float a = Vector3.Dot(targetVelocity, targetVelocity) - Vector3.Scale(projectileVelocity, projectileVelocity);
//			float b = 2 * Vector3.Dot(targetVelocity, totarget);
//			float c = Vector3.Dot(totarget, totarget);
//			
//			float p = -b / (2 * a);
//			float q = (float)Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);
//			
//			float t1 = p - q;
//			float t2 = p + q;
//			float t;
//			
//			if (t1 > t2 && t2 > 0)
//			{
//			    t = t2;
//			}
//			else
//			{
//			    t = t1;
//			}
//			
//			Vector3 aimSpot = target.position + target.velocity * t;
//			
//			return aimSpot;
//		}

		/// <summary>
		/// Restricts the translation applied to a vector relative to an origin point.
		/// </summary>
		/// <returns>The delta by origin.</returns>
		/// <param name="v">The vector being translated.</param>
		/// <param name="origin">The origin point.</param>
		/// <param name="delta">The translation vector being applied to <paramref name="v"/>.</param>
		/// <param name="minDistanceFromOrigin">Minimum allowed distance from origin.</param>
		/// <param name="maxDistanceFromOrigin">Max allowed distance from origin.</param>
		public static Vector3 RestrictDeltaByOrigin(this Vector3 v, Vector3 origin, Vector3 delta, float minDistanceFromOrigin, float maxDistanceFromOrigin)
		{
			var newPos = v + delta;
			
			// perform distance check using x and y axis only
			var p1 = new Vector3(newPos.x, 0, newPos.z);
			var p2 = new Vector3(origin.x, 0, origin.z);
			var d = Vector3.Distance(p1, p2);
			if (d < minDistanceFromOrigin || d > maxDistanceFromOrigin)
				delta = Vector3.zero;

			return delta;
		}

        /// <summary>
        /// Checks if a target vector is within a cone shaped area.
        /// </summary>
        /// <returns><c>true</c> if target is in the cone, <c>false</c> otherwise.</returns>
        /// <param name="origin">The origin vector of the cone.</param>
        /// <param name="direction">The direction in which the cone is facing.</param>
        /// <param name="distance">The distance that the cone beam extends to.</param>
        /// <param name="sweepDegrees">The angle in degrees across the full sweep of the cone.</param>
        /// <param name="target">The target position vector to be checked.</param>
        public static bool CheckInCone(this Vector3 origin, Vector3 direction, float distance, float sweepDegrees, Vector3 target)
        {
            var delta = target - origin;
            
            // Check if within distance of the cone.
            if (delta.magnitude > distance)
                return false;
            
            // Get the heading vector from origin to target.
            var heading = delta.normalized;
            
            // Get the dot product relative to a direction vector from the origin. The result is equal to the cosine of the angle between them.
            // it will be one when the directions are the same, zero when they are at 90º and 0.5 when they are at 60º.
            var dot = Vector3.Dot(direction, heading);
            
            var maxAngleCos = Mathf.Cos(Mathf.Deg2Rad * sweepDegrees / 2f);
            
            if (dot < maxAngleCos)
                return false;
            
            return true;
        }

        /// <summary>
        /// Checks if a target vector is in the direct line of sight of this vector.
        /// </summary>
        /// <returns><c>true</c> if the target is visible, <c>false</c> otherwise.</returns>
        /// <param name="origin">The origin vector.</param>
        /// <param name="target">The target position to be checked.</param>
        /// <param name="layerMask">Layer mask for layers that are considered obstructions to the line of sight.</param>
        public static bool CheckCanSee(this Vector3 origin, Vector3 target, int layerMask)
        {
            var delta = target - origin;
            var ray = new Ray(origin, delta);
            if (Physics.Raycast(ray, delta.magnitude, layerMask))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Calculates the center of the vertices in <paramref name="vertices"/>.
        /// </summary>
        public static Vector2 CalculateCenter(this IEnumerable<Vector2> vertices)
        {
            var count = 0;
            var sum = Vector2.zero;
            foreach (var p in vertices)
            {
                sum += p;
                count++;
            }
            return sum / count;
        }

        /// <summary>
        /// Recalculate the vertices in the polygon so they are relative to <paramref name="origin"/>.
        /// </summary>
        /// <param name="poly">The polygon whose vertices will be recalculated.</param>
        /// <param name="origin">The origin for the recalculated vertices.</param>
        public static IEnumerable<Vector2> Reposition(this IEnumerable<Vector2> poly, Vector2 origin)
        {
            // Calculate the current center of the polygon.
            var center = poly.CalculateCenter();

            // Calculate the offset from the new origin.
            var offset = origin - center;

            // Apply the offset to the vertices.
            foreach (var p in poly)
            {
                yield return new Vector2(p.x + offset.x, p.y + offset.y);
            }
        }

        public static IEnumerable<Vector2> Scale(this IEnumerable<Vector2> poly, float scale)
        {
            // Calculate the current center of the polygon.
            var center = poly.CalculateCenter();
            var repositioned = poly.Reposition(Vector2.zero);
            var scaled = repositioned.Select(v => v * scale);

            return scaled.Select(v => v + center);
        }

		#region Axis mapping

		/// <summary>
		/// Maps Y to X, Z to Y, and X to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (2,3,1).
		/// </example>
		public static Vector3 ToYZX(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.x);
		}
		
		/// <summary>
		/// Maps Z to X, X to Y, and Y to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (3,1,2).
		/// </example>
		public static Vector3 ToZXY(this Vector3 v)
		{
			return new Vector3(v.z, v.x, v.y);
		}
		
		/// <summary>
		/// Maps X to X, Z to Y, and T to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (1,3,2).
		/// </example>
		public static Vector3 ToXZY(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.y);
		}

		#endregion
	}
}

