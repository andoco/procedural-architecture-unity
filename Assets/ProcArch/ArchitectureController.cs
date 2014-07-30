using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ArchitectureController : MonoBehaviour {

	[Serializable]
	public class Arg
	{
		public string name;
		public string value;
	}

	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();

	public string source;
	public Material material;
	public string[] args;

	// Use this for initialization
	void Start () {
		if (this.source != null)
		{
			var architecture = this.architectureBuilder.Build(this.source, this.args.ToList());

			var meshFilter = this.gameObject.AddComponent<MeshFilter>();
			var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
			meshFilter.sharedMesh = architecture.Mesh;
			meshRenderer.material = this.material;
		}
	}
}
