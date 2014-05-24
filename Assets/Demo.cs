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

			if (leaf.Value is GeometryScope)
			{
				var geoScope = (GeometryScope)leaf.Value;
				AddGeometryToScene(geoScope);
			}
		}
	}

	private void AddGeometryToScene(GeometryScope geoNode)
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
		this.RootScope = new TreeNode<IScope>(this.NextId(), null);
		this.RootScope.Value = new Scope(Matrix4x4.identity);
		this.CurrentScope = this.RootScope;
	}

	public TreeNode<IScope> RootScope { get; private set; }

	public TreeNode<IScope> CurrentScope { get; private set; }

	public IDictionary<string, Mesh> Shapes { get; set; }

	public void AddScope(Vector3 trans, Quaternion rot, Vector3 scale)
	{
		Debug.Log(string.Format("Adding scope to {0}", this.CurrentScope));
		this.CurrentScope = this.CurrentScope.AddScope(this.NextId(), Matrix4x4.TRS(trans, rot, scale));
	}

	public void AddShape(string name)
	{
		Debug.Log(string.Format("Adding shape to {0}", this.CurrentScope));
		var mesh = this.Shapes[name];
		this.CurrentScope.AddGeometry(this.NextId(), mesh);
	}

	private string NextId()
	{
		return (this.counter++).ToString();
	}
}

public interface IScope
{
	Matrix4x4 Matrix { get; }
}

public class Scope : IScope
{
	public Scope(Matrix4x4 matrix)
	{
		this.Matrix = matrix;
	}

	public Matrix4x4 Matrix { get; private set; }	
}

public class GeometryScope : Scope
{
	public GeometryScope(Matrix4x4 matrix, Mesh geometry)
		: base(matrix)
	{
		this.Geometry = geometry;
	}

	public Mesh Geometry { get; private set; }
}

public static class TreeNodeExtensions
{
	public static TreeNode<IScope> AddScope(this TreeNode<IScope> root, string id, Matrix4x4 matrix)
	{
		var node = new TreeNode<IScope>(id, root);
		node.Value = new Scope(matrix);
		root.Add(node);

		return node;
	}

	public static TreeNode<IScope> AddGeometry(this TreeNode<IScope> root, string id, Mesh geometry)
	{
		var node = new TreeNode<IScope>(id, root);
		node.Value = new GeometryScope(root.Value.Matrix, geometry);
		root.Add(node);

		return node;
	}
}