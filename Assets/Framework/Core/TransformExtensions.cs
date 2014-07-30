namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

	public static class TransformExtensions
	{
		public static Vector3 PredictFuturePosition(this Transform transform, float speed, float time)
		{
			return transform.position + (transform.forward * speed * time);
		}
		
		/// <summary>
		/// Calculates the time in seconds required to reach <paramref name="targetPos"/> at <paramref name="speed"/>.
		/// </summary>
		/// <remarks>
		/// The calculation assumes that the <paramref name="targetPos"/> remains stationary.
		/// </remarks>
		/// <returns>
		/// The time in seconds to reach <paramref name="targetPos"/>.
		/// </returns>
		/// <param name='transform'>
		/// Transform.
		/// </param>
		/// <param name='targetPos'>
		/// Target position.
		/// </param>
		/// <param name='speed'>
		/// Speed.
		/// </param>
		public static float TimeToTarget(this Transform transform, Vector3 targetPos, float speed)
		{
			var distance = Vector3.Distance(targetPos, transform.position);
			
			return distance / speed;
		}

		public static bool MoveTowards(this Transform transform, Vector3 targetPos, float speed, float arrivalDist)
		{
			transform.position += (targetPos - transform.position).normalized * speed * Time.deltaTime;

			return Vector3.Distance(transform.position, targetPos) <= arrivalDist;
		}

	    public static bool IsHit(this Transform target, Vector3 screenPos)
	    {
	        Ray ray;
	        RaycastHit hit;
	        
	        ray = Camera.main.ScreenPointToRay(screenPos);
	        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
	        {
	            if (hit.collider.transform == target)
	            {
	                return true;
	            }
	        }
	        
	        return false;
	    }

	    public static bool IsNearHit(this Transform target, Vector3 screenPos, float radius)
	    {
	        Ray ray;
	        RaycastHit hit;
	        
	        ray = Camera.main.ScreenPointToRay(screenPos);
	        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
	        {
	            // We got a direct hit on the target.
	            if (hit.collider.transform == target)
	                return true;

	            // Check all colliders within a sphere.
	            var colliders = Physics.OverlapSphere(hit.point, radius);
	            foreach (var c in colliders)
	            {
	                if (c.transform == target)
	                {
	                    return true;
	                }
	            }
	        }
	        
	        return false;
	    }

	    public static T GetComponentInSelfOrParent<T>(this Transform t) where T : MonoBehaviour
	    {
	        return t.GetComponentInSelfOrAncestors<T>(1);
	    }

	    /// <summary>
	    /// Gets the component in self or ancestors.
	    /// </summary>
	    /// <returns>The component in self or ancestors.</returns>
	    /// <param name="t">The transform to search.</param>
	    /// <param name="depth">Depth to search for in ancestors. 0 will only search self, 1 will search self and parent and so on.</param>
	    /// <typeparam name="T">The type of the component to search for.</typeparam>
	    public static T GetComponentInSelfOrAncestors<T>(this Transform t, int depth) where T : MonoBehaviour
	    {
	        T component = null;
	        while (component == null && t != null && depth >= 0)
	        {
	            component = t.gameObject.GetComponent<T>();
	            t = t.parent;
	            depth--;
	        }
	        return component;
	    }
	}
}