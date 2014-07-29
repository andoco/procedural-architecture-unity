using UnityEngine;
using System;
using System.Collections.Generic;

public class ScopeTest : MonoBehaviour {

	private class ScopeComponent
	{
		public ScopeComponent(string name, SimpleTransform tx, Func<Vector3, Vector3> axisMap)
		{
			this.Name = name;
			this.Transform = tx;
			this.AxisMap = axisMap;
		}

		public string Name { get; private set; }

		public SimpleTransform Transform { get; private set; }

		public Func<Vector3, Vector3> AxisMap { get; private set; }
	}

	private class Scope
	{
		public Scope(SimpleTransform tx)
		{
			this.Transform = tx;
			this.Components = new List<ScopeComponent>();
		}

		public SimpleTransform Transform { get; private set; }

		public IList<ScopeComponent> Components { get; private set; }
	}

	private IList<Scope> scopes = new List<Scope>();
//	private IList<ScopeComponent> scopeComponents = new List<ScopeComponent>();

	public GameObject scopePrefab;
	public GameObject scopeCmpPrefab;

	// Use this for initialization
	void Start () {
		var scope1 = new Scope(new SimpleTransform(Vector3.zero, Quaternion.identity, new Vector3(2f, 3f, 4f)));
		// right
		scope1.Components.Add(new ScopeComponent("right", new SimpleTransform(new Vector3(0.5f, 0f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.right), new Vector3(1f, 0.1f, 1f)), x => x.ToZXY()));
		// left
		scope1.Components.Add(new ScopeComponent("left", new SimpleTransform(new Vector3(-0.5f, 0f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.left), new Vector3(1f, 0.1f, 1f)), x => x.ToZXY()));
		// forward
		scope1.Components.Add(new ScopeComponent("front", new SimpleTransform(new Vector3(0f, 0f, 0.5f), Quaternion.LookRotation(Vector3.up, Vector3.forward), new Vector3(1f, 0.1f, 1f)), x => x.ToXZY()));
		// backward
		scope1.Components.Add(new ScopeComponent("back", new SimpleTransform(new Vector3(0f, 0f, -0.5f), Quaternion.LookRotation(Vector3.up, Vector3.back), new Vector3(1f, 0.1f, 1f)), x => x.ToXZY()));
		// down
		scope1.Components.Add(new ScopeComponent("bottom", new SimpleTransform(new Vector3(0f, -0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.down), new Vector3(1f, 0.1f, 1f)), x => x));
		// up
		scope1.Components.Add(new ScopeComponent("top", new SimpleTransform(new Vector3(0f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), new Vector3(1f, 0.1f, 1f)), x => x));

		this.scopes.Add(scope1);

		foreach (var scope in this.scopes)
		{
			var scopeTx = scope.Transform;

			var scopeGo = (GameObject)GameObject.Instantiate(this.scopePrefab);
			scopeGo.transform.position = scope.Transform.Position;
			scopeGo.transform.rotation = scope.Transform.Rotation;
			scopeGo.transform.localScale = scope.Transform.Scale;

			foreach (var cmp in scope.Components)
			{
				var cmpTx = cmp.Transform;

				var p = scopeTx.Position + (scopeTx.Rotation * Vector3.Scale(scopeTx.Scale, cmp.Transform.Position));
				
//				var s = scopeTx.Scale;
//				s = cmp.Transform.Rotation * s;
//				s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
//				var newScale = s;
//				var newScale = scopeTx.Scale.ToYZX();
//				var newScale = scopeTx.Scale.ToZXY(); // This works!
//				var newScale = new Vector3(scopeTx.Scale.z, scopeTx.Scale.x, scopeTx.Scale.y);
				var newScale = cmp.AxisMap(scopeTx.Scale);
				newScale = Vector3.Scale(newScale, cmpTx.Scale);

				var go = (GameObject)GameObject.Instantiate(this.scopeCmpPrefab);
				go.name = cmp.Name;
				go.transform.position = p;
				go.transform.rotation = cmp.Transform.Rotation;
				go.transform.localScale = newScale;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	void OnDrawGizmos()
//	{
//		foreach (var scope in this.scopes)
//		{
//			var tx = scope.Transform;
//			Gizmos.DrawWireCube(tx.Position, tx.Scale);
//
//			Gizmos.color = Color.red;
//			foreach (var cmp in scope.Components)
//			{
//				var p = tx.Position + (tx.Rotation * Vector3.Scale(tx.Scale, cmp.Transform.Position));
//
////				var s = tx.Scale;
////				s = cmp.Transform.Rotation * s;
////				s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
////				var newScale = s;
//				var newScale = tx.Scale.ToYZX();
//
//				Gizmos.DrawSphere(p, 0.1f);
//				Gizmos.DrawWireCube(p, newScale);
//			}
//		}
//	}
}
