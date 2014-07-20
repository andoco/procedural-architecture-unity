using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FacadeVolume : Volume
{
	public FacadeVolume()
		: base()
	{
		// Corners
		this.Corners.Add(new Corner("corner-top-right", new Vector3(-0.5f, 0.5f, 0f)));
		this.Corners.Add(new Corner("corner-bottom-right", new Vector3(-0.5f, -0.5f, 0f)));
		this.Corners.Add(new Corner("corner-bottom-left", new Vector3(0.5f, -0.5f, 0f)));
		this.Corners.Add(new Corner("corner-top-left", new Vector3(0.5f, 0.5f, 0f)));

		// Edges
		this.Edges.Add(new Edge("edge-right", this.Corners[0], this.Corners[1]));
		this.Edges.Add(new Edge("edge-bottom", this.Corners[1], this.Corners[2]));
		this.Edges.Add(new Edge("edge-left", this.Corners[2], this.Corners[3]));
		this.Edges.Add(new Edge("edge-top", this.Corners[3], this.Corners[0]));

		// Faces
		this.Faces.Add(new Face("face", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }, new SimpleTransform(new Vector3(0f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one)));
	}

	public override Mesh BuildMesh ()
	{
		var meshBuilder = new MeshBuilder();

		var face = this.Faces[0];
		var verts = face.Corners.Select(c => c.Position).ToArray();

		foreach (var v in verts)
		{
			meshBuilder.Vertices.Add(v);
			meshBuilder.UVs.Add(Vector2.zero);
		}
		
		meshBuilder.AddTriangle(0, 1, 2);
		meshBuilder.AddTriangle(0, 2, 3);
		
		var mesh = meshBuilder.BuildMesh();
		
		mesh.RecalculateNormals();
		mesh.Optimize();
		
		return mesh;
	}
}