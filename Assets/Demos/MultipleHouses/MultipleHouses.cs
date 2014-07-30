using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Andoco.Core.Graph.Tree;
using UnityEngine;
using Andoco.Unity.Framework.Core.Meshes;

public class MultipleHouses : MonoBehaviour
{	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
	private IStyleConfig styleConfig = new CommonArchitectureStyleConfig();

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

		for (int i=0; i < this.numHouses; i++)
		{
			var architecture = architectureBuilder.Build(this.houseSource, new List<string> { "2", "3", "4" });

			var go = BuildGameObject(architecture.Mesh);

			var d = this.maxDist;
			go.transform.position = new Vector3(Random.Range(-d, d), 0f, Random.Range(-d, d));
			go.transform.Rotate(Vector3.up, Random.Range(0f, 180f));
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