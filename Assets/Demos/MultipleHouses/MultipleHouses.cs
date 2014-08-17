using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Andoco.Core;
using Andoco.Core.Graph.Tree;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;
using Andoco.Unity.ProcArch;

[System.Serializable]
public class ArchitectureItem
{
	public string assetName;
	public float weight;
	public string theme = "default";
}

public class ArchState
{
    public Architecture Arch { get; set; }

    public bool IsLightsOn { get; set; }
}

public class MultipleHouses : MonoBehaviour
{	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
    private IList<ArchState> builtArchitectures;

	private GameObject rootGo;

	public ArchitectureItem[] architectures;
	public Material material;
    public DayNightController dayNight;
    public int lightsOnHour = 19;
    public int lightsOffHour = 6;
	
	void Start () {
		this.ShowSystem();
        StartCoroutine(this.ToggleLights());
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			this.Rotate(45f);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			this.Rotate(-45f);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			this.Zoom(1f);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			this.Zoom(-1f);
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
		else if (GUILayout.RepeatButton("Zoom In"))
		{
			this.Zoom(1f);
		}
		else if (GUILayout.RepeatButton("Zoom Out"))
		{
			this.Zoom(-1f);
		}
	}

	private void Rotate(float amount)
	{
		Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, amount * Time.deltaTime);
	}

	private void Zoom(float amount)
	{
		Camera.main.transform.Translate(Vector3.forward * amount, Space.Self);
	}

	private void ShowSystem()
	{
		if (this.rootGo != null)
		{
			GameObject.Destroy(this.rootGo);
		}

        this.builtArchitectures = new List<ArchState>();

		this.rootGo = new GameObject("Architecture");

		var gridSize = new GridSize(10f, 10, 10);
		var offset = gridSize.GetSizeVector() * -0.5f + (Vector3.one * (gridSize.nodeSize / 2f));

		for (int x=0; x < gridSize.horizNodes; x++)
		{
			for (int y=0; y < gridSize.vertNodes; y++)
			{
				var w = 3f + (gridSize.nodeSize - 4f) * Random.value;
				var d = 3f + (gridSize.nodeSize - 4f) * Random.value;
				var h = 3f + 5f * Random.value;

				var rootArgs = new List<string> { w.ToString(), h.ToString(), d.ToString() };
				var globalArgs = new Dictionary<string, string>();

				var archWeights = this.architectures.Select(item => item.weight).ToArray();
				var archItem = this.architectures.PickRandomWeighted(archWeights, UnityRandomNumber.Instance);
				var asset = Resources.Load<TextAsset>(archItem.assetName);

				var architecture = architectureBuilder.Build(asset.name, asset.text, rootArgs, globalArgs, archItem.theme);
                this.builtArchitectures.Add(new ArchState { Arch = architecture });

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

    private IEnumerator ToggleLights()
    {
        while (true)
        {
            var isDark = this.dayNight.worldTimeHour > this.lightsOnHour || this.dayNight.worldTimeHour < this.lightsOffHour;

            var candidates = this.builtArchitectures.Where(x => x.IsLightsOn == !isDark).ToList();

            var picked = candidates.PickRandom(Mathf.Min(3, candidates.Count), UnityRandomNumber.Instance);

            foreach (var state in picked)
            {
                var arch = state.Arch;

                var cArr = arch.Mesh.colors;
                
                foreach (var item in arch.MeshData)
                {
                    if (Regex.IsMatch(item.Key, "windowRecess"))
                    {
                        for (var i=item.Value.ColorsStart; i < item.Value.ColorsEnd; i++)
                        {
                            cArr[i] = isDark ? Color.white : Color.black;
                        }
                    }
                }
                
                arch.Mesh.colors = cArr;

                state.IsLightsOn = !state.IsLightsOn;
            }

            yield return new WaitForSeconds(Random.value * 0.25f);
        }
    }
}