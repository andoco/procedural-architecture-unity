using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;

public class MultipleHouses : MonoBehaviour
{	
	private IShapeProductionSystem system;
	private IStyleConfig styleConfig;

	private GameObject rootGo;

	public string houseSource;
	public Material material;
	public int numHouses = 10;
	public float maxDist = 10f;

	// Use this for initialization
	void Start () {
		this.ShowSystem();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			this.Rotate(45f);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			this.Rotate(-45f);
		}
	}

	void OnGUI()
	{
		var screenScale = Screen.width / 960.0f;
		var scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale,screenScale,screenScale));
		GUI.matrix = scaledMatrix;

		if (GUILayout.Button("Build"))
		{
			this.ShowSystem();
		}
		else if (GUILayout.RepeatButton("Rotate Clockwise"))
		{
			this.Rotate(45f);
		}
		else if (GUILayout.RepeatButton("Rotate Anticlockwise"))
		{
			this.Rotate(-45f);
		}
	}

	private void Rotate(float amount)
	{
		Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, amount * Time.deltaTime);
	}

	private void ShowSystem()
	{
		var asset = (TextAsset)Resources.Load(this.houseSource);
		var source = asset.text;

		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

		this.rootGo = new GameObject("Architecture");

		try
		{
			BuildProductionSystem(source);
			BuildStyleConfig();

			for (int i=0; i < this.numHouses; i++)
			{
				var configuration = BuildProductionConfiguration();
				var mesh = this.BuildMesh(configuration);
				var go = BuildGameObject(mesh);

				var d = this.maxDist;
				go.transform.position = new Vector3(Random.Range(-d, d), 0f, Random.Range(-d, d));
				go.transform.Rotate(Vector3.up, Random.Range(0f, 180f));
			}
		}
		catch (System.Exception e)
		{
			Debug.Log(e);
		}
	}
	
//	void OnDrawGizmos()
//	{
//		if (Application.isPlaying && this.shapeConfiguration != null)
//		{
//			this.shapeConfiguration.RootNode.TraverseBreadthFirst(node => {
//				var shapeNode = (ShapeNode)node;
//				
//				if (node.IsLeaf)
//				{
//					Gizmos.color = Color.white;
//				}
//				else
//                {
//                    Gizmos.color = Color.grey;
//                }
//                
//				var vol = shapeNode.Value.Volume;
//
//				if (vol != null)
//				{
//					vol.DrawGizmos();
//                }
//            });
//        }
//    }
	    
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

	private void BuildStyleConfig()
	{
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
			},
			{ "vert", new Dictionary<string, object> { 
					{ "face-color", grey }
				} 
			},
			{ "horiz", new Dictionary<string, object> { 
					{ "face-color", beige }
				} 
			},
		};
		
		this.styleConfig = new StyleConfig(styles);
	}

	private IShapeConfiguration BuildProductionConfiguration()
	{
		Debug.Log("======= Building System ========");

		var args = new List<int> {
			Random.Range(2, 5),
			Random.Range(2, 5),
			Random.Range(4, 8)
		};

		var shapeConfiguration = new ShapeConfiguration(this.system.Rules);
		this.system.Run(shapeConfiguration, args.Select(x => x.ToString()).ToList());
		
		Debug.Log("======= Finished Building System ========");

		return shapeConfiguration;
	}

	private Mesh BuildMesh(IShapeConfiguration configuration)
	{
		var meshBuilder = new MeshBuilder();

		configuration.RootNode.TraverseBreadthFirst(node => {
			if (node.IsLeaf)
			{
				var shapeNode = (ShapeNode)node;
				var vol = shapeNode.Value.Volume;
				if (vol != null)
				{
					vol.ApplyStyle(this.styleConfig);
					vol.BuildMesh(meshBuilder);
				}
			}
		});

		var mesh = meshBuilder.BuildMesh();
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();

		return mesh;
	}

	private GameObject BuildGameObject(Mesh mesh)
	{
		var go = new GameObject();
		go.transform.parent = this.rootGo.transform;
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = this.material;
		
		return go;
	}
}