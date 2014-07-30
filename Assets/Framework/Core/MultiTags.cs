namespace Andoco.Unity.Framework.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public class MultiTags : MonoBehaviour
	{
	    private IDictionary<System.Reflection.FieldInfo, object> fieldDefaults;
		private static IDictionary<string, HashSet<GameObject>> tagMap = new Dictionary<string, HashSet<GameObject>>(StringComparer.InvariantCultureIgnoreCase);

		public string[] Tags = new string[0];

		#region Static methods

		public static void AddToIndex(GameObject go, string tag)
		{
			GetGameObjectSet(tag).Add(go);
		}

		public static void RemoveFromIndex(GameObject go, string tag)
		{
			GetGameObjectSet(tag).Remove(go);
		}

		public static GameObject[] FindGameObjectsWithTag(string tag)
		{
			return GetGameObjectSet(tag).ToArray();
		}

	    /// <summary>
	    /// Finds the game objects in <paramref name="source"/> that match the supplied tag filters.
	    /// </summary>
	    /// <returns>The game objects with tags.</returns>
	    /// <param name="source">Source.</param>
	    /// <param name="require">Require.</param>
	    /// <param name="include">Include.</param>
	    /// <param name="exclude">Exclude.</param>
	    public static IList<GameObject> FindGameObjectsWithTags(IEnumerable<GameObject> source, IEnumerable<string> require, IEnumerable<string> include, IEnumerable<string> exclude)
	    {
	        var filtered = new List<GameObject>();
	        
	        foreach (var go in source)
	        {
	            var multitags = go.GetComponent<MultiTags>();
	            
	            if (multitags == null)
	            {
	                //Debug.LogWarning(string.Format("GameObject {0} did not have a MultiTags component", go));
	                continue;
	            }
	            
	            // If gameobject has any of the "exclude" tags, discard it.
	            if (exclude.Any() && MultiTags.HasAny(multitags, exclude.ToArray()))
	            {
	                continue;
	            }
	            
	            // If gameobject does not have all of the "require" tags, discard it.
	            if (require.Any() && !MultiTags.HasAll(multitags, require.ToArray()))
	            {
	                continue;
	            }
	            
	            // If gameobject does not have any of the "include" tags, discard it.
	            if (include.Any() && !MultiTags.HasAny(multitags, include.ToArray()))
	            {
	                continue;
	            }
	            
	            filtered.Add(go);
	        }

	        return filtered;
	    }

	    /// <summary>
	    /// Finds the game objects that match the supplied tag filters.
	    /// </summary>
	    /// <returns>The game objects with tags.</returns>
	    /// <param name="require">Require.</param>
	    /// <param name="include">Include.</param>
	    /// <param name="exclude">Exclude.</param>
	    public static IList<GameObject> FindGameObjectsWithTags(IEnumerable<string> require, IEnumerable<string> include, IEnumerable<string> exclude)
		{
	        // Get source collection containing objects with any "required" or "included" tags
	        var sourceTags = new List<string>();
	        sourceTags.AddRange(require);
	        sourceTags.AddRange(include);
	        var source = new HashSet<GameObject>();

			foreach (var tag in sourceTags)
			{
				foreach (var go in MultiTags.FindGameObjectsWithTag(tag))
				{
					source.Add(go);
				}
			}

	        return FindGameObjectsWithTags(source, require, include, exclude);
		}
		
		public static IEnumerable<string> Parse(string serialized)
		{
			if (string.IsNullOrEmpty(serialized))
				return new string[0];
			
			return serialized.Split(',').Select(tag => tag.Trim());
		}

	    public static string SerializeTags(IEnumerable<string> tags)
	    {
	        return string.Join(",", tags.ToArray());
	    }

	    /// <summary>
	    /// Checks if two tags are considered equal.
	    /// </summary>
	    /// <returns><c>true</c>, if equal was tagsed, <c>false</c> otherwise.</returns>
	    /// <param name="tag1">Tag1.</param>
	    /// <param name="tag2">Tag2.</param>
	    public static bool TagsEqual(string tag1, string tag2)
	    {
	        return string.Equals(tag1, tag2, StringComparison.InvariantCultureIgnoreCase);
	    }

	    /// <summary>
	    /// Determines if has any of the specified multitags tags.
	    /// </summary>
	    /// <returns><c>true</c> if has any the specified multitags tags; otherwise, <c>false</c>.</returns>
	    /// <param name="multitags">Multitags.</param>
	    /// <param name="tags">Tags.</param>
	    public static bool HasAny(MultiTags multitags, string[] tags)
	    {
	        foreach (var tag in tags)
	        {
	            if (multitags.Tags.Any(t => MultiTags.TagsEqual(tag, t)))
	                return true;
	        }
	        return false;
	    }

	    /// <summary>
	    /// Determines if has all the specified multitags tags.
	    /// </summary>
	    /// <returns><c>true</c> if has all the specified multitags tags; otherwise, <c>false</c>.</returns>
	    /// <param name="multitags">Multitags.</param>
	    /// <param name="tags">Tags.</param>
	    public static bool HasAll(MultiTags multitags, string[] tags)
	    {
	        foreach (var tag in tags)
	        {
	            if (!multitags.Tags.Any(t => MultiTags.TagsEqual(tag, t)))
	                return false;
	        }
	        return true;
	    }

		#endregion

		void Awake()
		{
	        this.fieldDefaults = this.TrackFieldDefaults();
	        //UnityEngine.Debug.Log(string.Format("DEFAULT TAGS: {0}", this.fieldDefaults.Count));
	        //UnityEngine.Debug.Log(string.Format("AWAKE: {0} {1}", gameObject, MultiTags.SerializeTags(this.Tags)));
			foreach (var tag in this.Tags)
				MultiTags.AddToIndex(gameObject, tag);
		}

	    void Start()
	    {
	        //UnityEngine.Debug.Log(string.Format("START: {0} {1}", gameObject, MultiTags.SerializeTags(this.Tags)));
	    }

	    void Spawned()
	    {
	        //UnityEngine.Debug.Log(string.Format("SPAWNED: {0} {1}", gameObject, MultiTags.SerializeTags(this.Tags)));
	    }

		void Recycled()
		{
	        //UnityEngine.Debug.Log(string.Format("RECYCLED1: {0} {1}", gameObject, MultiTags.SerializeTags(this.Tags)));

	        // First clear existing tags so they are removed from the index.
	        this.ClearTags();

	        // Restore the original default tags.
	        this.ResetFieldValues(fieldDefaults);

	        // Add the default tags back to the index.
	        foreach (var tag in this.Tags)
	            MultiTags.AddToIndex(gameObject, tag);

	        //UnityEngine.Debug.Log(string.Format("RECYCLED2: {0} {1}", gameObject, MultiTags.SerializeTags(this.Tags)));

			//ResetState();
		}

		void OnDestroy()
		{
			ResetState();
		}

		public void AddTag(string tag)
		{
	        if (HasTag(tag))
	            return;

			var newTagArray = new string[Tags.Length + 1];
			Tags.CopyTo(newTagArray, 0);
			newTagArray[newTagArray.Length - 1] = tag;
			Tags = newTagArray;

			MultiTags.AddToIndex(gameObject, tag);
		}

		public void AddTags(params string[] tags)
		{
			foreach (var t in tags)
				AddTag(t);
		}

		public void RemoveTag(string tag)
		{
			Tags = Tags.Where(t => !MultiTags.TagsEqual(t, tag)).ToArray();
			MultiTags.RemoveFromIndex(gameObject, tag);
		}

		public void RemoveTags(params string[] tags)
		{
			foreach (var t in tags)
				RemoveTag(t);
		}

		public void ClearTags()
		{
			foreach (var t in Tags)
			{
				MultiTags.RemoveFromIndex(gameObject, t);
			}
			Tags = new string[0];
		}
		
		public bool HasTag(string tag)
		{
			return Tags.Any(t => MultiTags.TagsEqual(t, tag));
		}

	    public bool HasAny(string[] tags)
	    {
	        return MultiTags.HasAny(this, tags);
	    }

	    public bool HasAll(string[] tags)
	    {
	        return MultiTags.HasAll(this, tags);
	    }

		private static HashSet<GameObject> GetGameObjectSet(string tag)
		{
			var items = tagMap.ContainsKey(tag) ? tagMap[tag] : null;

			if (items == null)
			{
				items = new HashSet<GameObject>();
				tagMap[tag] = items;
			}

			return items;
		}

		private void ResetState()
		{
			this.ClearTags();
		}
	}
}