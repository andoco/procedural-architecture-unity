using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Demo : MonoBehaviour {

	public Mesh cubeMesh;

	// Use this for initialization
	void Start () {
		var root = new ScopeNode(null, Matrix4x4.identity);
		var c1 = root.AddScopeNode(root.Matrix);
		var c2 = root.AddScopeNode(root.Matrix * Matrix4x4.TRS(Vector3.forward*2f, Quaternion.Euler(Vector3.up*45f), Vector3.one*1.25f));

		c1.AddGeometryNode(this.cubeMesh);
		c2.AddGeometryNode(this.cubeMesh);

		foreach (var leaf in root.LeafNodes())
		{
			Debug.Log(leaf);

			if (leaf is GeometryNode)
			{
				var geoNode = (GeometryNode)leaf;
				AddGeometryToScene(geoNode);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void AddGeometryToScene(GeometryNode geoNode)
	{
		var go = new GameObject();
		go.transform.FromMatrix4x4(geoNode.Matrix);
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = geoNode.Geometry;
	}
}

public class Node : List<Node>
{
	public Node(Node parent)
	{
		this.Parent = parent;
	}

	public Node Parent { get; private set; }

	public bool IsLeaf { get { return !this.Any(); } }

	public IEnumerable<Node> LeafNodes()
	{
		if (this.IsLeaf)
			yield return this;

		foreach (var child in this)
		{
			Debug.Log(child);
			foreach (var leaf in child.LeafNodes())
				yield return leaf;
		}
	}

	public override string ToString ()
	{
		return string.Format ("[Node({0}): IsLeaf={2}]", GetType().Name, Parent, IsLeaf);
	}
}

public class ScopeNode : Node
{
	public ScopeNode(Node parent, Matrix4x4 matrix)
		: base(parent)
	{
		this.Matrix = matrix;
	}

	public Matrix4x4 Matrix { get; private set; }	
}

public class GeometryNode : ScopeNode
{
	public GeometryNode(ScopeNode parent, Mesh geometry)
		: base(parent, parent.Matrix)
	{
		this.Geometry = geometry;
	}

	public Mesh Geometry { get; private set; }
}

public static class NodeExtensions
{
	public static ScopeNode AddScopeNode(this ScopeNode root, Matrix4x4 matrix)
	{
		var node = new ScopeNode(root, matrix);
		root.Add(node);
		return node;
	}

	public static GeometryNode AddGeometryNode(this ScopeNode root, Mesh geometry)
	{
		var node = new GeometryNode(root, geometry);
		root.Add(node);
		return node;
	}
}