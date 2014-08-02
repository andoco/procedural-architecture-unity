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
	private TextAsset currentSourceFile;
	private GameObject rootGo;
	
	public Material material;
	public GUIText sourceGuiText;
	public GameObject faceTextPrefab;
	public Menu menu;
	
	void Start () {
		this.sourceFiles = Resources.LoadAll<TextAsset>("");

		for (int i=0; i < this.sourceFiles.Length; i++)
		{
			var file = this.sourceFiles[i];

			this.menu.AddItem(file.name, () => {
				this.ShowSystem(file);
			});
		}

		this.ShowSystem(this.sourceFiles[0]);
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

		if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			this.ShowNext();
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			this.ShowPrevious();
		}
	}

	void OnGUI()
	{
		var originalMatrix = GUI.matrix;
		var screenScale = Screen.width / 960.0f;
		var scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale,screenScale,screenScale));
		GUI.matrix = scaledMatrix;

		this.menu.DrawMenuButton();

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

		GUI.matrix = originalMatrix;
	}

	private void ShowNext()
	{
		var nextIndex = System.Array.IndexOf(this.sourceFiles, this.currentSourceFile) + 1;
		if (nextIndex < this.sourceFiles.Length)
		{
			var nextAsset = this.sourceFiles[nextIndex];
			this.ShowSystem(nextAsset);
		}
	}

	private void ShowPrevious()
	{
		var nextIndex = System.Array.IndexOf(this.sourceFiles, this.currentSourceFile) - 1;
		if (nextIndex >= 0)
		{
			var nextAsset = this.sourceFiles[nextIndex];
			this.ShowSystem(nextAsset);
		}
	}

	private void Rotate(float amount)
	{
		Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, amount * Time.deltaTime);
	}

	private void ShowSystem(TextAsset asset)
	{
		this.currentSourceFile = asset;

		if (this.sourceGuiText != null)
			this.sourceGuiText.text = asset.name;

		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

		this.rootGo = new GameObject("Architecture");

		try
		{
			var rootArgs = new List<string> { "2", "3", "4" };
			var globalArgs = new Dictionary<string, string>();

			this.architecture = architectureBuilder.Build(asset.name, asset.text, rootArgs, globalArgs);

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
			this.architecture.Configuration.DrawGizmos();
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