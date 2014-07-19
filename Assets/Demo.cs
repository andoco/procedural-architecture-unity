using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;


public class Demo : MonoBehaviour
{	
	private IShapeProductionSystem system;
	private IShapeConfiguration shapeConfiguration;
	private Dictionary<string, Mesh> shapeMeshes;
	private Vector3 anchor = Vector3.one * 0.5f;
//	private Vector3 anchor = new Vector3(0.5f, 0f, 0.5f);

	private const int numColors = 50;
	private Color[] faceColors = new Color[numColors];

	public Material material;

	// Use this for initialization
	void Start () {
		for (int i=0; i < numColors; i++)
		{
			faceColors[i] = new Color(Random.value, Random.value, Random.value);
		}

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

		var sb = new StringBuilder("======= Tree =======\n\n");
		this.shapeConfiguration.RootNode.TraverseDepthFirst((n, depth) => {
			var shapeNode = (ShapeNode)n;
			sb.AppendFormat("{0} {1}\n", "".PadLeft(depth, '-'), shapeNode.Value);
		});
		Debug.Log(sb);

//		this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
//			var shapeNode = (ShapeNode)node;
//
//			var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			go.transform.localPosition = new Vector3(0f, 0.5f, 0f);
//			go.transform.position = shapeNode.Value.Volume.Transform.Position; // this.CurrentScope.Matrix.GetPosition();
//			go.transform.rotation = shapeNode.Value.Volume.Transform.Rotation; // this.CurrentScope.Matrix.GetRotation();
//			go.transform.localScale = shapeNode.Value.Volume.Transform.Scale;
//
//		});
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
				var shapeNode = (ShapeNode)node;
				var nodeType = node.IsLeaf ? "LEAF" : "PARENT";
				
				if (node.IsLeaf)
				{
					Gizmos.color = Color.white;
				}
				else
                {
                    Gizmos.color = Color.grey;
                }
                
				if (shapeNode.Value.Volume != null)
				{
					var trans = shapeNode.Value.Transform;
					var vol = shapeNode.Value.Volume;

//					Gizmos.DrawLine(trans.Position, trans.Position + (trans.Rotation * Vector3.forward * 2f));

					for (int i=0; i < shapeNode.Value.Volume.Edges.Count; i++)
					{
						var edge = shapeNode.Value.Volume.Edges[i];
						Gizmos.color = faceColors[i];
						Gizmos.DrawLine(edge.CornerA.Position, edge.CornerB.Position);
//						var p1 = trans.Position + trans.Rotation * edge.CornerA.Position;
//						var p2 = trans.Position + trans.Rotation * edge.CornerB.Position;
//						Gizmos.DrawLine(p1, p2);
                    }

					vol.DrawGizmos();
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
	}

	private void AddGeometry(ShapeNode tree)
	{
		tree.TraverseBreadthFirst(node => {
			var shapeNode = (ShapeNode)node;
			var vol = shapeNode.Value.Volume;
			if (vol != null)
			{
				var mesh = vol.BuildMesh();
				AddGeometryMesh(shapeNode.Value.ShapeName, mesh, shapeNode.Value.Transform);
			}
		});

//		foreach (var leaf in tree.LeafNodeDescendants().Cast<ShapeNode>())
//		{
////			var name = leaf.Value.ShapeName;
////			if (!string.IsNullOrEmpty(name))
////			{
////				var mesh = this.shapeMeshes[name];
////				var trans = leaf.Value.Transform;
////				AddGeometryMesh(name, mesh, trans);
////			}
//
////			var mesh = leaf.Value.Volume.CreateMesh();
//			var mesh = leaf.Value.Volume.BuildMesh();
//			AddGeometryMesh(leaf.Value.ShapeName, mesh, leaf.Value.Transform);
//		}
	}

	private void AddGeometryMesh(string name, Mesh mesh, SimpleTransform trans)
	{
		var go = new GameObject(name);

//		go.transform.position = trans.Position;
//		go.transform.rotation = trans.Rotation;
//		go.transform.localScale = trans.Scale;

		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = this.material;
	}
}