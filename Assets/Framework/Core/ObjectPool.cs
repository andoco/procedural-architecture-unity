namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	
	public class ObjectPool : MonoBehaviour
	{
        [System.Serializable]
        public class AutoPoolInfo
        {
            public GameObject prefab;

            public int quantity;
        }

		private class PrefabInfo
		{
			public PrefabInfo()
			{
				this.Instances = new List<Transform>();
			}
			
			public GameObject Prefab { get; set; }
			
			public Transform PoolRoot { get; set; }
			
			public int InstanceCounter { get; set; }
			
			public IList<Transform> Instances { get; private set; }
		}
		
		private static ObjectPool _instance;

        private bool isPrePooling;
        private bool isFirstUpdate = true;
		
		private IDictionary<GameObject, PrefabInfo> prefabInfo = new Dictionary<GameObject, PrefabInfo>();
		
		private HashSet<Transform> scheduledForRecycle = new HashSet<Transform>();
		
		public const string GameObjectName = "_ObjectPool";
		
		public event System.EventHandler<ObjectSpawnedEventArgs> ObjectSpawned;
		
		public event System.EventHandler<ObjectRecycledEventArgs> ObjectRecycled;
		
		public Dictionary<Transform, GameObject> prefabLookup = new Dictionary<Transform, GameObject>();
		
		public bool PoolEnabled = true;

        public AutoPoolInfo[] autoPoolPrefabs;

        #region Static properties
        
        public static ObjectPool instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                // try and find existing gameobject
                var obj = GameObject.Find(GameObjectName);
                if (obj != null)
                {
                    _instance = obj.GetComponent<ObjectPool>();
                    if (_instance == null)
                        throw new System.InvalidOperationException(string.Format("Found GameObject with name {0} but it does not have the ObjectPool component", GameObjectName));
                    
                    Debug.Log("Using existing ObjectPool gameobject");
                    
                    return _instance;
                }
                
                // create new gameobject
                Debug.Log("Creating ObjectPool gameobject");
                obj = new GameObject(GameObjectName);
                obj.transform.localPosition = Vector3.zero;
                _instance = obj.AddComponent<ObjectPool>();
                return _instance;
            }
        }
        
        #endregion

        void Awake()
        {
            this.AutoPool();
        }
		
		void LateUpdate()
		{
			// Need to copy to array so that nested calls to Recycle() are possible from an object's Recycled() callback.
			var toRecycle = this.scheduledForRecycle.ToArray();
			this.scheduledForRecycle.Clear();
			
			foreach (var obj in toRecycle)
			{
				RecycleImmediate(obj);
			}

            if (this.autoPoolPrefabs != null && this.isFirstUpdate)
                this.isFirstUpdate = false;
		}
		
		public string GetNextObjectSuffix(GameObject prefab)
		{
			var info = this.GetOrCreatePrefabInfo(prefab);
			
			if (info.InstanceCounter > 9999)
				throw new System.InvalidOperationException(string.Format("InstanceCounter for prefab {0} exceeded 9999", prefab));
			
			var suffix = "-" + info.InstanceCounter.ToString("D4");
			info.InstanceCounter++;
			
			return suffix;
		}
		
		public void Clear()
		{
			this.prefabInfo.Clear();
		}

		public void CreatePool(GameObject prefab)
		{
			this.CreatePool(prefab, 0);
		}

		public void CreatePool(GameObject prefab, int initialSize)
		{
			Debug.Log(string.Format("Creating pool for {0} with initial size {1}", prefab, initialSize));
			
			if (!this.prefabInfo.ContainsKey(prefab))
			{
				this.GetOrCreatePrefabInfo(prefab);
				
				if (this.PoolEnabled)
				{
					var spawnedObjects = new List<Transform>();
					
					// IMPORTANT: Must spawn all initial objects before any get recycled, otherwise
					// we'll end up reusing the same one every time.
					for (int i=0; i < initialSize; i++)
					{
						var transform = Spawn(prefab);
						spawnedObjects.Add(transform);
					}
					
					// Recycle all initial objects in the pool.
					foreach (var transform in spawnedObjects)
					{
						// Using RecycleImmediate will cause the object to be recycled before its Start
						// method has been called. This will result in both Start and Spawned being called
						// at the same time when the object is next spawned, which could be problematic.
						//RecycleImmediate(transform);
						
						// Using Recycle instead of RecycleImmediate allow the Start methods of the object's 
						// components to be called before it is recycled. The next time it is spawned, only
						// the Spawned methods will be called.
						Recycle(transform);
					}
				}
			}
		}
		
        /// <summary>
        /// Spawn the specified prefab with a position and rotation.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: Objects should not be spawned during the first frame because any pre-pooled
        /// instances will not have been recycled yet.
        /// </remarks>
        /// <param name="prefab">Prefab.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
		public Transform Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			if (this.PoolEnabled && this.prefabInfo.ContainsKey(prefab))
			{
                if (this.isFirstUpdate && !this.isPrePooling)
                    Debug.LogWarning("Spawning an object before prepooling frame is complete");

				Transform obj = null;
				var info = this.prefabInfo[prefab];
				var list = info.Instances;
				if (list.Count > 0)
				{
					// Remove instance from pool until we have one that hasn't been destroyed.
					while (obj == null && list.Count > 0)
					{
						obj = list[0];
						list.RemoveAt(0);
					}
					
					if (obj != null)
					{
                        // We now have an instance from the pool we can setup for spawning.
						obj.transform.parent = null;
						obj.transform.localPosition = position;
						obj.transform.localRotation = rotation;
						obj.gameObject.SetActive(true);
						instance.prefabLookup.Add(obj, prefab);
						
						obj.SendMessage("Spawned", SendMessageOptions.DontRequireReceiver);
						
						// raise event
						if (instance.ObjectSpawned != null)
							instance.ObjectSpawned(instance, new ObjectSpawnedEventArgs(obj));
						
						return obj;
					}
				}

                // No pooled instance was available so make a new one.
				var suffix = instance.GetNextObjectSuffix(prefab);
				obj = ((GameObject)Object.Instantiate(prefab, position, rotation)).transform;
				obj.gameObject.name += suffix;
				instance.prefabLookup.Add(obj, prefab);
				return obj;
			}
			else
            {
                // No pool is available for the prefab for instantiate normally.
                var obj = ((GameObject)Object.Instantiate(prefab, position, rotation)).transform;
                this.prefabLookup[obj] = prefab;
                return obj;
            }
		}
		
		public Transform Spawn(GameObject prefab, Vector3 position)
		{
			return Spawn(prefab, position, Quaternion.identity);
		}
		
		public Transform Spawn(GameObject prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity);
		}
		
		/// <summary>
		/// Schedule the object to be recycled during the LateUpdate stage of the current frame.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void Recycle(Transform obj)
		{
			this.scheduledForRecycle.Add(obj);
		}
		
		public int Count(GameObject prefab)
		{
			if (this.prefabInfo.ContainsKey(prefab))
				return this.prefabInfo[prefab].Instances.Count;
			else
				return 0;
		}

        /// <summary>
        /// Returns objects that have been spawned from instance, even if they do no belong to a pool.
        /// </summary>
        /// <returns>The spawned instances.</returns>
        /// <param name="prefab">The prefab to return spawned instance for.</param>
        public IEnumerable<Transform> FindSpawnedInstances(GameObject prefab)
        {
            return this.prefabLookup.Where(item => item.Value == prefab).Select(item => item.Key);
        }

        #region Private methods
        
        private void AutoPool()
        {
            if (this.autoPoolPrefabs != null)
            {
                this.isPrePooling = true;
                foreach (var info in this.autoPoolPrefabs)
                {
                    this.CreatePool(info.prefab, info.quantity);
                }
                this.isPrePooling = false;
            }
        }
        
        private PrefabInfo GetOrCreatePrefabInfo(GameObject prefab)
        {
            PrefabInfo info;
            if (this.prefabInfo.ContainsKey(prefab))
                info = this.prefabInfo[prefab];
            else
            {
                info = new PrefabInfo
                {
                    Prefab = prefab,
                    PoolRoot = new GameObject(prefab.name).transform
                };
                info.PoolRoot.transform.parent = this.transform;
                this.prefabInfo[prefab] = info;
            }
            
            return info;
        }

        /// <summary>
        /// Immediately recycle the object by disabling it and returning it to the pool.
        /// </summary>
        /// <remarks>
        /// If an object is recycled using this method immediately after spawning, the object's
        /// Start() methods will not be called.
        /// </remarks>
        /// <param name="obj">Object.</param>
        private void RecycleImmediate(Transform obj)
        {
            GameObject prefab;
//            var prefab = instance.prefabLookup[obj];
            PrefabInfo info;

            // Check if a pool exists for the prefab
            if (this.PoolEnabled && this.prefabLookup.TryGetValue(obj, out prefab) && this.prefabInfo.TryGetValue(prefab, out info))

//            if (this.PoolEnabled && instance.prefabLookup.ContainsKey(obj))
//            if (this.PoolEnabled && info != null)
            {
                // add the recycled object back to the pool
//                var info = this.prefabInfo[];
                info.Instances.Add(obj);
                
                // stop tracking the recycled object
                this.prefabLookup.Remove(obj);
                
                // notify and set to inactive
                obj.SendMessage("Recycled", SendMessageOptions.DontRequireReceiver);
                obj.parent = info.PoolRoot;
                obj.gameObject.SetActive(false);
                
                // raise event
                if (this.ObjectRecycled != null)
                    this.ObjectRecycled(instance, new ObjectRecycledEventArgs(obj));
            }
            else
            {
                // No pool is available for the object so destroy normally.
                this.prefabLookup.Remove(obj);
                Object.Destroy(obj.gameObject);
            }
        }
        
        #endregion
	}
	
	public static class ObjectPoolExtensions
	{
		public static void CreatePool(this GameObject prefab)
		{
			prefab.CreatePool(0);
		}

		public static void CreatePool(this GameObject prefab, int initialSize)
		{
			ObjectPool.instance.CreatePool(prefab, initialSize);
		}
		
		public static Transform Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return ObjectPool.instance.Spawn(prefab, position, rotation);
		}
		public static Transform Spawn(this GameObject prefab, Vector3 position)
		{
			return ObjectPool.instance.Spawn(prefab, position, Quaternion.identity);
		}
		public static Transform Spawn(this GameObject prefab)
		{
			return ObjectPool.instance.Spawn(prefab, Vector3.zero, Quaternion.identity);
		}
		
		public static void Recycle(this Transform obj)
		{
			ObjectPool.instance.Recycle(obj);
		}
		
		public static int Count(GameObject prefab)
		{
			return ObjectPool.instance.Count(prefab);
		}
	}
	
	public class ObjectSpawnedEventArgs : System.EventArgs
	{
		public ObjectSpawnedEventArgs(Transform obj)
		{
			this.Object = obj;
		}
		
		public Transform Object { get; private set; }
	}
	
	public class ObjectRecycledEventArgs : System.EventArgs
	{
		public ObjectRecycledEventArgs(Transform obj)
		{
			this.Object = obj;
		}
		
		public Transform Object { get; private set; }
	}
}