using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Andoco.Core.Graph.Tree;

public class ArchitectureController : MonoBehaviour {
	
	private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
	private Architecture architecture;
	
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

		this.architecture = this.architectureBuilder.Build(this.sourceName, this.sourceContent, this.args.ToList());

		var meshFilter = this.gameObject.AddComponent<MeshFilter>();
		var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = this.architecture.Mesh;
		meshRenderer.material = this.material;
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
}
