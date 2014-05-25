using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class MyListener : SimplePAGBaseListener
{
	private readonly ScopeDrawContext drawCtx;

	public MyListener(ScopeDrawContext drawCtx)
	{
		this.drawCtx = drawCtx;
	}
	
	public override void EnterSuccessor (SimplePAGParser.SuccessorContext context)
	{
		base.EnterSuccessor (context);

		var id = context.ID();
		if (id == null)
		{
//			var cmd = context.GetChild(0).GetText();
//			if (cmd == "Set")
//			{
//				var shapeName = context.asset_uri().GetText().Trim('"');
//				Debug.Log(string.Format("Setting shape {0}", shapeName));
//				this.drawCtx.AddShape(shapeName);
//			}
		}

//		Debug.Log(string.Format("enter successor {0}", context.ToStringTree()));
//		Debug.Log(context.GetChild(0).GetText());
	}

	public override void EnterCmdDefinition (SimplePAGParser.CmdDefinitionContext context)
	{
		base.EnterCmdDefinition (context);

		var cmdName = context.ID().GetText();
		var args = context.argumentsDefinition();

		Debug.Log(string.Format("Command: {0} {1}", cmdName, args.ToStringTree()));

		switch (cmdName)
		{
		case "Set":
			var shapeName = args.argumentDefinition(0).GetText().Trim('"');
			this.drawCtx.AddShape(shapeName);
			break;
		case "Trans":
			var axes = args.argumentDefinition().Select(x => float.Parse(x.floating_point().GetText())).ToArray();
			var delta = new Vector3(axes[0], axes[1], axes[2]);
			this.drawCtx.AddScope(delta, Quaternion.identity, Vector3.one);
			break;
		case "Rot":
			var rotAxes = args.argumentDefinition().Select(x => float.Parse(x.floating_point().GetText())).ToArray();
			Debug.Log(string.Format("{0}", rotAxes[0]));
			this.drawCtx.AddScope(Vector3.zero, Quaternion.Euler(rotAxes[0], rotAxes[1], rotAxes[2]), Vector3.one);
			break;
		}
	}

//	public override void ExitSuccessor (SimplePAGParser.SuccessorContext context)
//	{
//		base.ExitSuccessor (context);
//
//		Debug.Log(string.Format("exit successor {0}", context.GetText()));
//	}
//
//	public override void ExitAsset_uri (SimplePAGParser.Asset_uriContext context)
//	{
//		base.ExitAsset_uri (context);
//
//		Debug.Log(string.Format("asset uri {0}", context.GetText()));
//	}
}

public class Demo : MonoBehaviour {

	public Mesh cubeMesh;
	public Mesh rootMesh;
	public Material material;

	// Use this for initialization
	void Start () {
		var scopeCtx = new ScopeDrawContext();
		scopeCtx.Shapes = new Dictionary<string, Mesh> { { "facade", cubeMesh }, { "roof", rootMesh } };

		scopeCtx.AddScope(Vector3.up*0.5f, Quaternion.identity, Vector3.one);
//		scopeCtx.AddShape("facade");
//		scopeCtx.AddScope(Vector3.up*2f, Quaternion.Euler(Vector3.right*90f), Vector3.one);
//		scopeCtx.AddShape("roof");

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
		Debug.Log(string.Format("Adding scope trans={0}, rot={1}, scale={2} to {3}", trans, rot, scale, this.CurrentScope));
		this.CurrentScope = this.CurrentScope.AddScope(this.NextId(), Matrix4x4.TRS(trans, rot, scale));
	}

	public void AddShape(string name)
	{
		Debug.Log(string.Format("Adding shape {0} to {1}", name, this.CurrentScope));
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

