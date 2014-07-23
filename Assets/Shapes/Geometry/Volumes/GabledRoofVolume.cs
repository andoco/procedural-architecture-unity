using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GabledRoofVolume : Volume
{
	public GabledRoofVolume(IStyleConfig styleConfig)
		: base()
	{
		// Corners
		this.Corners.Add(new Corner("corner-bottom-1", new Vector3(0.5f, 0f, 0.5f)));
		this.Corners.Add(new Corner("corner-bottom-2", new Vector3(0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-bottom-3", new Vector3(-0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-bottom-4", new Vector3(-0.5f, 0f, 0.5f)));
		
		this.Corners.Add(new Corner("corner-top-1", new Vector3(0f, 1f, 0.5f)));
		this.Corners.Add(new Corner("corner-top-2", new Vector3(0f, 1f, -0.5f)));

		// Edges
		this.Edges.Add(new Edge("edge-bottom-1", this.Corners[0], this.Corners[1]));
		this.Edges.Add(new Edge("edge-bottom-2", this.Corners[1], this.Corners[2]));
		this.Edges.Add(new Edge("edge-bottom-3", this.Corners[2], this.Corners[3]));
		this.Edges.Add(new Edge("edge-bottom-4", this.Corners[3], this.Corners[0]));
		
		this.Edges.Add(new Edge("edge-top-1", this.Corners[4], this.Corners[5])); // Top-spine

		this.Edges.Add(new Edge("edge-top-2", this.Corners[4], this.Corners[0])); // upper end
		this.Edges.Add(new Edge("edge-top-3", this.Corners[4], this.Corners[3])); // upper end

		this.Edges.Add(new Edge("edge-top-4", this.Corners[5], this.Corners[1])); // lower end
		this.Edges.Add(new Edge("edge-top-5", this.Corners[5], this.Corners[2])); // lower end

		// Faces
		this.Faces.Add(new Face("face-top-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4] }, new SimpleTransform(new Vector3(0.25f, 0.5f, 0f), Quaternion.AngleAxis(45f, Vector3.forward), Vector3.one)));
		this.Faces.Add(new Face("face-top-2", new List<Corner> { this.Corners[4], this.Corners[5], this.Corners[2], this.Corners[3] }, new SimpleTransform(new Vector3(-0.25f, 0.5f, 0f), Quaternion.AngleAxis(-45, Vector3.forward), Vector3.one)));

		this.Faces.Add(new Face("face-end-1", new List<Corner> { this.Corners[0], this.Corners[4], this.Corners[3] }, new SimpleTransform(new Vector3(-0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one)));
		this.Faces.Add(new Face("face-end-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[5] }, new SimpleTransform(new Vector3(0f, 0.5f, 0.5f), Quaternion.LookRotation(Vector3.back, Vector3.up), Vector3.one)));

		var styles = styleConfig.GetByName("roof");
		var topColor = (Color)styles["top-color"];
		var sideColor = (Color)styles["side-color"];

		this.Faces[0].Color = topColor;
		this.Faces[1].Color = topColor;
		this.Faces[2].Color = sideColor;
		this.Faces[3].Color = sideColor;

		foreach (var face in this.Faces)
		{
			this.Components[face.Name] = face.Transform;
		}
	}
	
	public override Mesh BuildMesh()
	{
		var meshBuilder = new MeshBuilder();
		
		var baseIndex = 0;
		
		foreach (var face in this.Faces)
		{
			var verts = face.Corners.Select(c => c.Position).ToArray();
			
			foreach (var v in verts)
			{
				meshBuilder.Vertices.Add(v);
				meshBuilder.UVs.Add(Vector2.zero);
				meshBuilder.Colors.Add(face.Color);
			}
			
			meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);

			if (verts.Length == 4)
				meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
			
			baseIndex = meshBuilder.Vertices.Count;
		}
		
		var mesh = meshBuilder.BuildMesh();
		
		mesh.RecalculateNormals();
		mesh.Optimize();
		
		return mesh;
	}
}