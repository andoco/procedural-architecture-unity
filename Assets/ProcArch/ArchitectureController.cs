using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ArchitectureController : MonoBehaviour {
	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
	
	public string sourceName;
	public string sourceContent;
	public Material material;
	public string[] args;
	
	void Start()
	{
		if (string.IsNullOrEmpty(this.sourceName) || string.IsNullOrEmpty(this.sourceContent))
		{
			throw new ArgumentException("Requires sourceName and sourceContent");
		}

		var architecture = this.architectureBuilder.Build(this.sourceName, this.sourceContent, this.args.ToList());

		var meshFilter = this.gameObject.AddComponent<MeshFilter>();
		var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = architecture.Mesh;
		meshRenderer.material = this.material;
	}
}
