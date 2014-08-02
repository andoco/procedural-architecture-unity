using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Andoco.Core.Graph.Tree;
using UnityEngine;
using Andoco.Unity.Framework.Core.Meshes;
using Andoco.Unity.Framework.Core;

public class MultipleHouses : MonoBehaviour
{	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();

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
		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

		this.rootGo = new GameObject("Architecture");

		var asset = Resources.Load<TextAsset>(this.houseSource);

		var gridSize = new GridSize(10f, 10, 10);
		var offset = gridSize.GetSizeVector() * -0.5f;

		for (int x=0; x < gridSize.horizNodes; x++)
		{
			for (int y=0; y < gridSize.vertNodes; y++)
			{
				var w = 3f + (gridSize.nodeSize - 4f) * Random.value;
				var d = 3f + (gridSize.nodeSize - 4f) * Random.value;
				var h = 3f + 3f * Random.value;

				var rootArgs = new List<string> { w.ToString(), h.ToString(), d.ToString() };
				var globalArgs = new Dictionary<string, string>();

				var architecture = architectureBuilder.Build(asset.name, asset.text, rootArgs, globalArgs);

				var pos = offset + new Vector3((float)x * gridSize.nodeSize, 0f, (float)y * gridSize.nodeSize);
				var go = BuildGameObject(architecture.Mesh);
				go.transform.position = pos;

			}
		}
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