using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;


public class Demo : MonoBehaviour
{	
	private IShapeProductionSystem system;
	private IShapeConfiguration shapeConfiguration;
//	private Dictionary<string, Mesh> shapeMeshes;
	private Vector3 anchor = Vector3.one * 0.5f;
//	private Vector3 anchor = new Vector3(0.5f, 0f, 0.5f);

	private const int numColors = 50;
	private Color[] faceColors = new Color[numColors];
	private TextAsset[] sourceFiles;
	private int currentSourceFileIndex;
	private GameObject rootGo;
	
	public Material material;
	public GUIText guiText;

	// Use this for initialization
	void Start () {
		for (int i=0; i < numColors; i++)
		{
			faceColors[i] = new Color(Random.value, Random.value, Random.value);
		}

		this.sourceFiles = Resources.LoadAll<TextAsset>("");

		this.ShowSystem();

//		var meshBuilder = new MeshBuilder();
//
//		meshBuilder.BuildQuad(Vector3.zero, 1f, 1f);
//		var quad = meshBuilder.BuildMesh();
//
//		meshBuilder.Clear();
//		meshBuilder.BuildCube(1f, 1f, 1f, anchor);
//		var cube = meshBuilder.BuildMesh();
//
//		meshBuilder.Clear();
//		meshBuilder.BuildFacade(1f, 1f, 1f, anchor);
//		var facade = meshBuilder.BuildMesh();
//        
//		meshBuilder.Clear();
//		meshBuilder.BuildRoof(1f, 1f, 1f, 0.2f, 0.2f, 0.02f, anchor);
//		var roof = meshBuilder.BuildMesh();
//        
//		this.shapeMeshes = new Dictionary<string, Mesh> { 
//			{ "quad", quad },
//			{ "cube", cube },
//			{ "facade", facade }, 
//			{ "roof", roof },
//		};
	}

	void Update()
	{
		Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 45f * Time.deltaTime);

		var showSystem = false;

		if (Input.GetKeyUp(KeyCode.RightArrow) & this.currentSourceFileIndex < this.sourceFiles.Length - 1)
		{
			this.currentSourceFileIndex += 1;
			showSystem = true;
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow) && this.currentSourceFileIndex > 0)
		{
			this.currentSourceFileIndex -= 1;
			showSystem = true;
		}

		if (showSystem)
		{
			ShowSystem();
		}
	}

	private void ShowSystem()
	{
		var asset = this.sourceFiles[this.currentSourceFileIndex];
		var source = asset.text;

		if (this.guiText != null)
			this.guiText.text = asset.name;

		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

		try
		{
			BuildProductionSystem(source);
			BuildProductionConfiguration();
			AddGeometry(this.shapeConfiguration.RootNode);
			
			var sb = new StringBuilder("======= Tree =======\n\n");
			this.shapeConfiguration.RootNode.TraverseDepthFirst((n, depth) => {
				var shapeNode = (ShapeNode)n;
				sb.AppendFormat("{0} {1}\n", "".PadLeft(depth, '-'), shapeNode.Value);
			});
			Debug.Log(sb);
		}
		catch (System.Exception e)
		{
			Debug.Log(e);
		}
	}

	void OnDrawGizmos()
	{
		if (Application.isPlaying && this.shapeConfiguration != null)
		{
			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
				var shapeNode = (ShapeNode)node;
				
				if (node.IsLeaf)
				{
					Gizmos.color = Color.white;
				}
				else
                {
                    Gizmos.color = Color.grey;
                }
                
				var vol = shapeNode.Value.Volume;

				if (vol != null)
				{
					for (int i=0; i < vol.Edges.Count; i++)
					{
						var edge = vol.Edges[i];
						Gizmos.color = faceColors[i];
						Gizmos.DrawLine(edge.CornerA.Position, edge.CornerB.Position);
                    }

					vol.DrawGizmos();
                }
            });
        }
    }
	    
    private void BuildProductionSystem(string sourceFile)
	{
		var builder = new IronyShapeProductionSystemBuilder();
		this.system = builder.Build(sourceFile);
		this.system.Axiom = "root";
		
		foreach (var item in this.system.Rules)
		{
			Debug.Log(string.Format("RULE: {0} = {1}", item.Key, item.Value));
		}
	}

	private void BuildProductionConfiguration()
	{
		Debug.Log("======= Building System ========");

		var beige = new Color(208f/255f, 197f/255f, 133f/255f);
		var grey = new Color(110f/255f, 110f/255f, 110f/255f);

		var styles = new Dictionary<string, IDictionary<string, object>> {
			{ "facade", new Dictionary<string, object> { 
					{ "face-color", grey }
				} 
			},
			{ "roof", new Dictionary<string, object> { 
					{ "top-color", new Color(255f/255f, 195f/255f, 0) },
					{ "side-color", grey }
				}
			}
		};

		var styleConfig = new StyleConfig(styles);
		
		this.shapeConfiguration = new ShapeConfiguration(this.system.Rules, styleConfig);
		this.system.Run(this.shapeConfiguration);
		
		Debug.Log("======= Finished Building System ========");
	}

	private void AddGeometry(ShapeNode tree)
	{
		this.rootGo = new GameObject("Architecture");

		tree.TraverseBreadthFirst(node => {
			if (node.IsLeaf)
			{
				var shapeNode = (ShapeNode)node;
				var vol = shapeNode.Value.Volume;
				if (vol != null)
				{
					var mesh = vol.BuildMesh();
					AddGeometryMesh(vol.GetType().Name, mesh, shapeNode.Value.Transform);
                }
			}
		});
	}

	private void AddGeometryMesh(string name, Mesh mesh, SimpleTransform trans)
	{
		var go = new GameObject(name);
		go.transform.parent = this.rootGo.transform;

		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = this.material;
	}
}