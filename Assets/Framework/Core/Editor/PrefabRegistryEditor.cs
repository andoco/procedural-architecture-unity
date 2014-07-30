namespace Andoco.Unity.Framework.Core.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using UnityEditor;
	using UnityEngine;

	using Andoco.Unity.Framework.Core;

	[CustomEditor(typeof(PrefabRegistry))]
	public class PrefabRegistryEditor : Editor
	{
		private IDictionary<int, string> renames = new Dictionary<int, string>();
		private IDictionary<int, GameObject> prefabChanges = new Dictionary<int, GameObject>();
		private IList<int> deletions = new List<int>();
		
		[MenuItem("Andoco/Add Prefab Registry")]
		public static void AddPrefabRegistry() {
			// will lazily add the GameObject
			Debug.Log(string.Format("Added new instance of {0}", PrefabRegistry.Instance));
		}
		
		public override void OnInspectorGUI()
		{
			var myTarget = (PrefabRegistry)target;

			EditorGUILayout.BeginVertical();

			//foreach (var item in myTarget.Prefabs)
			for (int i=0; i < myTarget.Prefabs.Count; i++)
			{
				var item = myTarget.Prefabs[i];

				EditorGUILayout.BeginHorizontal();
				
				// name field
				var newName = EditorGUILayout.TextField(item.Key);
				if (newName != item.Key)
					this.renames.Add(i, newName);
				
				// prefab field
				var newPrefab = EditorGUILayout.ObjectField(item.Value, typeof(GameObject), false) as GameObject;
				if (newPrefab != item.Value)
					this.prefabChanges.Add(i, newPrefab);
				
				// delete field
				if (GUILayout.Button("x"))
				{
					this.deletions.Add(i);
				}
				
				EditorGUILayout.EndHorizontal();
			}
			
			// add new prefab field
			if (GUILayout.Button("Add New Prefab"))
			{
				var name = string.Format("Prefab{0:D4}", myTarget.Prefabs.Count);
				myTarget.Prefabs.Add(new PrefabRegistryItem(name, null));
			}

			EditorGUILayout.EndVertical();
			
			// perform renames
			foreach (var renameItem in this.renames)
			{
				var item = myTarget.Prefabs[renameItem.Key];
				item.Key = renameItem.Value;
			}
			
			// perform prefab changes
			foreach (var prefabItem in this.prefabChanges)
			{
				var item = myTarget.Prefabs[prefabItem.Key];
				item.Value = prefabItem.Value;
			}
			
			// perform deletions
			foreach (var index in this.deletions)
			{
				myTarget.Prefabs.RemoveAt(index);
			}
			
			this.renames.Clear();
			this.prefabChanges.Clear();
			this.deletions.Clear();

			if (GUI.changed)
			{
				//Debug.Log("GUI changed. Setting dirty", this);
				EditorUtility.SetDirty(myTarget);
			}
		}
	}
}