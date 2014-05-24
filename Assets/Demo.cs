using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Demo : MonoBehaviour {

	public Mesh cubeMesh;
	public Mesh rootMesh;
	public Material material;

	// Use this for initialization
	void Start () {
		var scopeCtx = new ScopeDrawContext();
		scopeCtx.Shapes = new Dictionary<string, Mesh> { { "facade", cubeMesh }, { "roof", rootMesh } };

		scopeCtx.AddScope(Vector3.up*0.5f, Quaternion.identity, Vector3.one);
		scopeCtx.AddShape("facade");
		scopeCtx.AddScope(Vector3.up*2f, Quaternion.Euler(Vector3.right*90f), Vector3.one);
		scopeCtx.AddShape("roof");

		var root = scopeCtx.RootScope;

		foreach (var leaf in root.LeafNodeDescendants())
		{
			Debug.Log(leaf);

			if (leaf is GeometryNode)
			{
				var geoNode = (GeometryNode)leaf;
				AddGeometryToScene(geoNode);
			}
		}
	}

	private void AddGeometryToScene(GeometryNode geoNode)
	{
		var go = new GameObject();
		go.transform.FromMatrix4x4(geoNode.Matrix);
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = geoNode.Geometry;
		meshRenderer.material = this.material;
	}
}

public class ScopeDrawContext
{
	private int counter = 0;

	public ScopeDrawContext()
	{
		this.RootScope = new ScopeNode(this.NextId(), null, Matrix4x4.identity);
		this.CurrentScope = this.RootScope;
	}

	public ScopeNode RootScope { get; private set; }

	public ScopeNode CurrentScope { get; private set; }

	public IDictionary<string, Mesh> Shapes { get; set; }

	public void AddScope(Vector3 trans, Quaternion rot, Vector3 scale)
	{
		Debug.Log(string.Format("Adding scope to {0}", this.CurrentScope));
		this.CurrentScope = this.CurrentScope.AddScopeNode(this.NextId(), Matrix4x4.TRS(trans, rot, scale));
	}

	public void AddShape(string name)
	{
		Debug.Log(string.Format("Adding shape to {0}", this.CurrentScope));
		var mesh = this.Shapes[name];
		this.CurrentScope.AddGeometryNode(this.NextId(), mesh);
	}

	private string NextId()
	{
		return (this.counter++).ToString();
	}
}

public class ScopeNode : TreeNode
{
	public ScopeNode(string id, TreeNode parent, Matrix4x4 matrix)
		: base(id, parent)
	{
		this.Matrix = matrix;
	}

	public Matrix4x4 Matrix { get; private set; }	
}

public class GeometryNode : ScopeNode
{
	public GeometryNode(string id, ScopeNode parent, Mesh geometry)
		: base(id, parent, parent.Matrix)
	{
		this.Geometry = geometry;
	}

	public Mesh Geometry { get; private set; }
}

public static class NodeExtensions
{
	public static ScopeNode AddScopeNode(this ScopeNode root, string id, Matrix4x4 matrix)
	{
		var node = new ScopeNode(id, root, matrix);
		root.Add(node);
		return node;
	}

	public static GeometryNode AddGeometryNode(this ScopeNode root, string id, Mesh geometry)
	{
		var node = new GeometryNode(id, root, geometry);
		root.Add(node);
		return node;
	}
}