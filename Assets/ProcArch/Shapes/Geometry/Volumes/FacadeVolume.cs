using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core.Meshes;

public class FacadeVolume : Volume
{
	public FacadeVolume()
		: base()
	{
		this.Style = "facade";
	}

	protected override void OnBuildVolume(Argument[] args)
	{
		// Corners
		this.Corners.Add(new Corner("corner-top-right", new Vector3(0.5f, 0f, 0.5f)));
		this.Corners.Add(new Corner("corner-bottom-right", new Vector3(0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-bottom-left", new Vector3(-0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-top-left", new Vector3(-0.5f, 0f, 0.5f)));
		
		// Edges
		this.Edges.Add(new Edge("edge-right", this.Corners[0], this.Corners[1]));
		this.Edges.Add(new Edge("edge-bottom", this.Corners[1], this.Corners[2]));
		this.Edges.Add(new Edge("edge-left", this.Corners[2], this.Corners[3]));
		this.Edges.Add(new Edge("edge-top", this.Corners[3], this.Corners[0]));
		
		// Faces
		this.Faces.Add(new Face("face", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }));
		
		this.Components.Add(new ScopeComponent(this.Faces[0].Name, new SimpleTransform(Vector3.zero, Quaternion.identity, new Vector3(1f, 0f, 1f)), v => v));
	}

	protected override void ApplyStyle(IStyleConfig styleConfig)
	{
		this.Faces[0].Color = styleConfig.GetColor(this.Style, this.Theme, "face-color");
	}	
}