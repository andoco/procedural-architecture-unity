using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using Andoco.Core.Graph.Tree;
using Andoco.Unity.Framework.Core.Meshes;

public class Demo : MonoBehaviour
{	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
	private Architecture architecture;

	private Vector2 scrollPos;
	private const int numColors = 50;
	private TextAsset[] sourceFiles;
	private int currentSourceFileIndex;
	private GameObject rootGo;
	
	public Material material;
	public GUIText sourceGuiText;
	public GameObject faceTextPrefab;

	// Use this for initialization
	void Start () {
		this.sourceFiles = Resources.LoadAll<TextAsset>("");
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

		if (Input.GetKeyUp(KeyCode.RightArrow) & this.currentSourceFileIndex < this.sourceFiles.Length - 1)
		{
			this.ShowNext();
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow) && this.currentSourceFileIndex > 0)
		{
			this.ShowPrevious();
		}
	}

	void OnGUI()
	{
		var screenScale = Screen.width / 960.0f;
		var scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale,screenScale,screenScale));
		GUI.matrix = scaledMatrix;

		if (GUILayout.Button("Previous"))
		{
			this.ShowPrevious();
		}
		else if (GUILayout.Button("Next"))
		{
			this.ShowNext();
		}
		else if (GUILayout.RepeatButton("Rotate Clockwise"))
		{
			this.Rotate(45f);
		}
		else if (GUILayout.RepeatButton("Rotate Anticlockwise"))
		{
			this.Rotate(-45f);
		}

		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, GUILayout.Height(200f));
		for (int i=0; i < this.sourceFiles.Length; i++)
		{
			if (GUILayout.Button(this.sourceFiles[i].name))
			{
				this.currentSourceFileIndex = i;
				this.ShowSystem();
			}
		}
		GUILayout.EndScrollView();
	}

	private void ShowNext()
	{
		if (this.currentSourceFileIndex < this.sourceFiles.Length - 1)
		{
			this.currentSourceFileIndex += 1;
			ShowSystem();
		}
	}

	private void ShowPrevious()
	{
		if (this.currentSourceFileIndex > 0)
		{
			this.currentSourceFileIndex -= 1;
			ShowSystem();
		}
	}

	private void Rotate(float amount)
	{
		Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, amount * Time.deltaTime);
	}

	private void ShowSystem()
	{
		var asset = this.sourceFiles[this.currentSourceFileIndex];

		if (this.sourceGuiText != null)
			this.sourceGuiText.text = asset.name;

		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

		this.rootGo = new GameObject("Architecture");

		try
		{
			this.architecture = architectureBuilder.Build(asset.name, asset.text, new List<string> { "2", "3", "4" });

			BuildGameObject(this.architecture.Mesh);
			AddScopeComponentTextMeshes(this.architecture.Configuration);
		}
		catch (System.Exception e)
		{
			Debug.Log(e);
		}
	}
	
	void OnDrawGizmos()
	{
		if (Application.isPlaying && this.architecture != null)
		{
			this.architecture.Configuration.RootNode.TraverseBreadthFirst(node => {
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
					vol.DrawGizmos();
                }
            });
        }
	}

	private void BuildGameObject(Mesh mesh)
	{
		var go = new GameObject();
		go.transform.parent = this.rootGo.transform;
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer = go.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = mesh;
		meshRenderer.material = this.material;
	}

	private void AddScopeComponentTextMeshes(IShapeConfiguration configuration)
	{
		configuration.RootNode.TraverseBreadthFirst(node => {
			var shapeNode = (ShapeNode)node;
			var vol = shapeNode.Value.Volume;

			if (vol != null)
			{
				foreach (var comp in vol.Components)
				{
					var name = comp.Name;
					var trans = comp.Transform;
					
					var worldPos = vol.Transform.Position + (vol.Transform.Rotation * Vector3.Scale(trans.Position, vol.Transform.Scale));
					
					var textGo = (GameObject)GameObject.Instantiate(this.faceTextPrefab);
                    textGo.transform.parent = this.rootGo.transform;
                    var textMesh = textGo.GetComponent<TextMesh>();
                    textMesh.text = name;
                    textMesh.transform.position = worldPos;
                }
			}
		});
	}
}