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

            public bool IsPooled { get; set; }
			
			public Transform PoolRoot { get; set; }

            public Transform SpawnedRoot { get; set; }
			
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

        public bool organiseSpawned = false;

        public Transform spawnedRoot;

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
            if (!this.PoolEnabled)
                throw new System.InvalidOperationException(string.Format("Cannot create a pool for {0} because the pool is not enabled", prefab));

            Debug.Log(string.Format("Creating pool for {0} with initial size {1}", prefab, initialSize));

            if (this.prefabInfo.ContainsKey(prefab))
                throw new System.InvalidOperationException(string.Format("Cannot create a pool for {0} because the pool already exists", prefab));
            
            var info = this.GetOrCreatePrefabInfo(prefab);
            info.IsPooled = true;
                
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
            if (prefab == null)
                throw new System.ArgumentNullException("prefab", "Prefab must be supplied to spawn an instance");

			if (this.PoolEnabled)
			{
                if (this.isFirstUpdate && !this.isPrePooling)
                    Debug.LogWarning("Spawning an object before prepooling frame is complete");

                PrefabInfo info;

                // Get the prefab info.
                if (!this.prefabInfo.TryGetValue(prefab, out info))
                {
                    // Create prefab info for any non-pooled prefabs. This allows us to organise and track the instances
                    // even though they won't be returned to the pool.

                    info = new PrefabInfo
                    {
                        Prefab = prefab,
                        IsPooled = false
                    };
                    
                    this.prefabInfo.Add(prefab, info);
                }

				Transform obj = null;
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
                        SetupParent(obj.transform, info);

                        // We now have an instance from the pool we can setup for spawning.
						obj.transform.localPosition = position;
						obj.transform.localRotation = rotation;
						obj.gameObject.SetActive(true);
						this.prefabLookup.Add(obj, prefab);
						
						obj.SendMessage("Spawned", SendMessageOptions.DontRequireReceiver);
						
						// raise event
						if (this.ObjectSpawned != null)
							this.ObjectSpawned(this, new ObjectSpawnedEventArgs(obj));
						
						return obj;
					}
				}

                // No pooled instance was available so make a new one.
				var suffix = this.GetNextObjectSuffix(prefab);
				obj = ((GameObject)Object.Instantiate(prefab, position, rotation)).transform;
				obj.gameObject.name += suffix;
				this.prefabLookup.Add(obj, prefab);
                SetupParent(obj.transform, info);
				return obj;
			}
			else
            {
                // The pool is not enabled so we just instantiate normally.
                var obj = ((GameObject)Object.Instantiate(prefab, position, rotation)).transform;
                this.prefabLookup[obj] = prefab;
                return obj;
            }
		}

        private void SetupParent(Transform tx, PrefabInfo info)
        {
            if (this.organiseSpawned)
            {
                if (info.SpawnedRoot == null)
                {
                    info.SpawnedRoot = (new GameObject(info.Prefab.name)).transform;
                    info.SpawnedRoot.transform.parent = this.spawnedRoot;
                }

                tx.parent = info.SpawnedRoot;
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

		public bool TryRecycle(Transform obj)
		{
			if (
				this.PoolEnabled && 
				this.prefabLookup.ContainsKey(obj))
			{
				this.Recycle(obj);
				return true;
			}

			return false;
		}

		public void RecycleOrDestroy(Transform obj)
		{
			if (!this.TryRecycle(obj))
			{
				GameObject.Destroy(obj.gameObject);
			}
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
            PrefabInfo info;

			if (obj == null)
				throw new System.ArgumentNullException(string.Format("Cannot recycle object because it is null or destroyed. {0}", obj));

            // Check if a pool exists for the prefab
            if (
                this.PoolEnabled && 
                this.prefabLookup.TryGetValue(obj, out prefab) && 
                this.prefabInfo.TryGetValue(prefab, out info))
            {
                // Stop tracking the recycled object.
                this.prefabLookup.Remove(obj);

                if (info.IsPooled)
                {
                    info.Instances.Add(obj);

                    // notify and set to inactive
                    obj.SendMessage("Recycled", SendMessageOptions.DontRequireReceiver);
                    obj.parent = info.PoolRoot;
                    obj.gameObject.SetActive(false);

                    // raise event
                    if (this.ObjectRecycled != null)
                        this.ObjectRecycled(this, new ObjectRecycledEventArgs(obj));
                }
                else
                {
                    // No pool is available for the object so destroy normally. We need to set the parent to null
                    // first as it isn't immediately removed from its parent otherwise.
                    obj.parent = null;
                    GameObject.Destroy(obj.gameObject);
                }

                // Destroy any empty prefab root gameobject.
                if (info.SpawnedRoot != null && info.SpawnedRoot.childCount == 0)
                {
                    GameObject.Destroy(info.SpawnedRoot.gameObject);
                    info.SpawnedRoot = null;
                }
            }
            else
            {
                throw new System.InvalidOperationException(string.Format("Cannot recycle an instance that was not spawned from this pool. {0}", obj));
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

		public static bool TryRecycle(this Transform obj)
		{
			return ObjectPool.instance.TryRecycle(obj);
		}

		public static void RecycleOrDestroy(this Transform obj)
		{
			ObjectPool.instance.RecycleOrDestroy(obj);
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