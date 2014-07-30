namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

	public class SingletonGameObjectComponent<T> where T : MonoBehaviour
	{
		private readonly string gameObjectName;
		
		private T instance = null;
		
		public SingletonGameObjectComponent()
			: this(typeof(T).Name)
		{
			
		}
		
		public SingletonGameObjectComponent(string gameObjectName)
		{
			if (string.IsNullOrEmpty(gameObjectName))
				throw new ArgumentException("The game object name cannot be null or empty", "gameObjectName");
			
			this.gameObjectName = gameObjectName;
		}
		
		public T Instance
		{
			get
			{
				if (this.instance == null)
				{
					var go = GameObject.Find(this.gameObjectName);
					if (go == null)
					{
						go = new GameObject(this.gameObjectName);
						go.transform.localPosition = Vector3.zero;
					}
					
					this.instance = go.GetComponent<T>();
					if (this.instance == null)
					{
						this.instance = go.AddComponent<T>();
					}
				}
				
				return this.instance;
			}
		}
	}
}