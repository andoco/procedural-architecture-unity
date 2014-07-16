using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Demo : MonoBehaviour
{	
	private IShapeProductionSystem system;
	private IShapeConfiguration shapeConfiguration;
	private Dictionary<string, Mesh> shapeMeshes;
	private Vector3 anchor = Vector3.one * 0.5f;
//	private Vector3 anchor = new Vector3(0.5f, 0f, 0.5f);

	public Material material;

	// Use this for initialization
	void Start () {
		var meshBuilder = new MeshBuilder();

		meshBuilder.BuildQuad(Vector3.zero, 1f, 1f);
		var quad = meshBuilder.BuildMesh();

		meshBuilder.Clear();
		meshBuilder.BuildCube(1f, 1f, 1f, anchor);
		var cube = meshBuilder.BuildMesh();

		meshBuilder.Clear();
		meshBuilder.BuildFacade(1f, 1f, 1f, anchor);
		var facade = meshBuilder.BuildMesh();
        
		meshBuilder.Clear();
		meshBuilder.BuildRoof(1f, 1f, 1f, 0.2f, 0.2f, 0.02f, anchor);
		var roof = meshBuilder.BuildMesh();
        
		this.shapeMeshes = new Dictionary<string, Mesh> { 
			{ "quad", quad },
			{ "cube", cube },
			{ "facade", facade }, 
			{ "roof", roof },
		};

		BuildProductionSystem("volumetest");
		BuildProductionConfiguration();
		AddGeometry(this.shapeConfiguration.RootNode);
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Debug.Log("GIZMOS ====");
//			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
//				var shapeNode = (ShapeNode)node;
//				var matrix = shapeNode.Value.Matrix;
//				Gizmos.matrix = matrix;
//				var nodeType = node.IsLeaf ? "LEAF" : "PARENT";
//				Debug.Log(string.Format("pos={0}, rot={1}, scale={2} {3} {4}", matrix.GetPosition(), matrix.GetRotation(), matrix.GetScale(), nodeType, node));
//                
//                if (node.IsLeaf)
//				{
//					Gizmos.color = Color.white;
//                }
//				else
//				{
//					Gizmos.color = Color.grey;
//				}
//
//				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
//			});
//			
//			Gizmos.matrix = Matrix4x4.identity;

			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
				var shapeNode = (ShapeNode)node;
//				var matrix = shapeNode.Value.Matrix;
//				Gizmos.matrix = matrix;
				var nodeType = node.IsLeaf ? "LEAF" : "PARENT";
//				Debug.Log(string.Format("pos={0}, rot={1}, scale={2} {3} {4}", matrix.GetPosition(), matrix.GetRotation(), matrix.GetScale(), nodeType, node));
				
				if (node.IsLeaf)
				{
					Gizmos.color = Color.white;
				}
				else
                {
                    Gizmos.color = Color.grey;
                }
                
//                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
				if (shapeNode.Value.Volume != null)
				{
					Debug.Log("here");
					foreach (var edge in shapeNode.Value.Volume.Edges)
					{
						Gizmos.DrawLine(edge.CornerA.Position, edge.CornerB.Position);
                    }

					foreach (var corner in shapeNode.Value.Volume.Corners)
					{
						Gizmos.DrawSphere(corner.Position, 0.1f);
					}
                }
            });
        }
    }
    
    private void BuildProductionSystem(string sourceFile)
	{
		var houseProg = Resources.Load<TextAsset>(sourceFile).text;

		var builder = new IronyShapeProductionSystemBuilder();
		this.system = builder.Build(houseProg);
		this.system.Axiom = "root";
		
		foreach (var item in this.system.Rules)
		{
			Debug.Log(string.Format("RULE: {0} = {1}", item.Key, item.Value));
		}
	}

	private void BuildProductionConfiguration()
	{
		Debug.Log("======= Building System ========");
		
		this.shapeConfiguration = new ShapeConfiguration(this.system.Rules);
		this.system.Run(this.shapeConfiguration);
		
		Debug.Log("======= Finished Building System ========");
		
		Debug.Log(this.shapeConfiguration);
	}

	private void AddGeometry(ShapeNode tree)
	{
		Debug.Log(tree);
		foreach (var leaf in tree.LeafNodeDescendants().Cast<ShapeNode>())
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