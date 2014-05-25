using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class Demo : MonoBehaviour {

	public Mesh cubeMesh;
	public Mesh rootMesh;
	public Material material;

	// Use this for initialization
	void Start () {
		var scopeCtx = new ScopeDrawContext();
		scopeCtx.Shapes = new Dictionary<string, Mesh> { { "facade", cubeMesh }, { "roof", rootMesh } };

		ProcessPAG("house", scopeCtx);
		ProcessDrawScope(scopeCtx);
	}

	private void ProcessPAG(string sourceFile, ScopeDrawContext scopeCtx)
	{
		var houseProg = Resources.Load<TextAsset>(sourceFile).text;
		
		var input = new AntlrInputStream(houseProg);
		var lexer = new SimplePAGLexer(input);
		var tokens = new CommonTokenStream(lexer);
		var parser = new SimplePAGParser(tokens);
		
		var listener = new MyListener(scopeCtx);
		ParseTreeWalker.Default.Walk(listener, parser.pag());
	}

	private void ProcessDrawScope(ScopeDrawContext scopeCtx)
	{
		var root = scopeCtx.RootScope;
		
		foreach (var leaf in root.LeafNodeDescendants())
		{
			Debug.Log(leaf);
			
			if (leaf.Value is Shape)
			{
				var geoScope = (Shape)leaf.Value;
				AddGeometryToScene(geoScope);
			}
		}
	}

	private void AddGeometryToScene(Shape geoNode)
	{
		var go = new GameObject();
		go.transform.FromMatrix4x4(geoNode.Matrix);
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = geoNode.Geometry;
		meshRenderer.material = this.material;
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

public class Shape : Scope
{
	public Shape(Matrix4x4 matrix, Mesh geometry)
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
		node.Value = new Shape(root.Value.Matrix, geometry);
		root.Add(node);

		return node;
	}
}

