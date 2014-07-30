namespace Andoco.Unity.Framework.Core
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	[System.Serializable]
	public class PrefabRegistryItem
	{
		public PrefabRegistryItem(string key, GameObject go)
		{
			this.Key = key;
			this.Value = go;
		}

		public string Key;
		public GameObject Value;
	}

	public class PrefabRegistry : MonoBehaviour
	{
		private static SingletonGameObjectComponent<PrefabRegistry> lazyInstance = new SingletonGameObjectComponent<PrefabRegistry>();
		
		public static PrefabRegistry Instance { get { return lazyInstance.Instance; } }
		
		public List<PrefabRegistryItem> Prefabs = new List<PrefabRegistryItem>();
		
		public void RegisterPrefab(GameObject prefab, string name = null)
		{
			name = name ?? prefab.name;
			var item = this.Prefabs.SingleOrDefault(x => x.Key == name);
			if (item == null)
				this.Prefabs.Add(new PrefabRegistryItem(name, prefab));
			else
				item.Value = prefab;
		}
		
		public GameObject FindByName(string name)
		{
			var item = this.Prefabs.SingleOrDefault(x => x.Key == name);

			return item == null ? null : item.Value;
		}
	}
}