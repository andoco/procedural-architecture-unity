using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using Irony.Parsing;

public class Demo : MonoBehaviour
{	
	private IShapeConfiguration shapeConfiguration;
	private Dictionary<string, Mesh> shapeMeshes;
	private Vector3 anchor = Vector3.one * 0.5f;
//	private Vector3 anchor = new Vector3(0.5f, 0f, 0.5f);

	public Material material;

	// Use this for initialization
	void Start () {
		var meshBuilder = new MeshBuilder();
		meshBuilder.BuildCube(1f, 1f, 1f, anchor);
		var cube = meshBuilder.BuildMesh();

		meshBuilder.Clear();
		meshBuilder.BuildFacade(1f, 1f, 1f, anchor);
		var facade = meshBuilder.BuildMesh();
        
		meshBuilder.Clear();
		meshBuilder.BuildRoof(1f, 1f, 1f, 0.2f, 0.2f, 0.02f, anchor);
		var roof = meshBuilder.BuildMesh();
        
		this.shapeMeshes = new Dictionary<string, Mesh> { 
			{ "cube", cube },
			{ "facade", facade }, 
			{ "roof", roof },
		};

//		ProcessPAG("simple");
		ProcessIronyGrammar("house");
		AddGeometry(this.shapeConfiguration.RootNode);
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Debug.Log("GIZMOS ====");
			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
				var matrix = node.Value.Matrix;
				Gizmos.matrix = matrix;
				var nodeType = node.IsLeaf ? "LEAF" : "PARENT";
				Debug.Log(string.Format("pos={0}, rot={1}, scale={2} {3} {4}", matrix.GetPosition(), matrix.GetRotation(), matrix.GetScale(), nodeType, node));
                
                if (node.IsLeaf)
				{
					Gizmos.color = Color.white;
                }
				else
				{
					Gizmos.color = Color.grey;
				}

				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			});
			
			Gizmos.matrix = Matrix4x4.identity;
		}
	}

	private void ProcessPAG(string sourceFile)
	{
		var houseProg = Resources.Load<TextAsset>(sourceFile).text;
		
		var input = new AntlrInputStream(houseProg);
		var lexer = new SimplePAGLexer(input);
		var tokens = new CommonTokenStream(lexer);
		var parser = new SimplePAGParser(tokens);

		this.shapeConfiguration = new ShapeConfiguration();

		var system = new ShapeProductionSystem(this.shapeConfiguration);
		var listener = new ProductionSystemListener(system);
		ParseTreeWalker.Default.Walk(listener, parser.pag());
		system.Axiom = "root";

		foreach (var item in system.Rules)
		{
			Debug.Log(string.Format("RULE: {0} = {1}", item.Key, item.Value));
		}

		Debug.Log("======= Building System ========");

		system.Run();

		Debug.Log("======= Finished Building System ========");
	}

	private void ProcessIronyGrammar(string sourceFile)
	{
		var houseProg = Resources.Load<TextAsset>(sourceFile).text;

		this.shapeConfiguration = new ShapeConfiguration();
		
		var system = new ShapeProductionSystem(this.shapeConfiguration);
		system.Axiom = "root";

		var builder = new IronyArchitectureBuilder();

		builder.Build(houseProg, system);
		
		foreach (var item in system.Rules)
		{
			Debug.Log(string.Format("RULE: {0} = {1}", item.Key, item.Value));
		}

		Debug.Log("======= Building System ========");

		system.Run();

		Debug.Log("======= Finished Building System ========");

		Debug.Log(this.shapeConfiguration);
	}

	private void AddGeometry(TreeNode<ShapeNodeValue> tree)
	{
		Debug.Log(tree);
		foreach (var leaf in tree.LeafNodeDescendants())
		{
			var name = leaf.Value.ShapeName;
			if (!string.IsNullOrEmpty(name))
			{
				var mesh = this.shapeMeshes[name];
				var matrix = leaf.Value.Matrix;
				AddGeometryMesh(name, mesh, matrix);
			}
		}
	}

	private void AddGeometryMesh(string name, Mesh mesh, Matrix4x4 matrix)
	{
		var go = new GameObject(name);

		matrix = Matrix4x4.identity * matrix;

		go.transform.FromMatrix4x4(matrix);

//		go.transform.position = matrix.GetPosition();
//		go.transform.rotation = matrix.GetRotation();
//		go.transform.localScale = matrix.GetScale();

		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = this.material;
	}
}