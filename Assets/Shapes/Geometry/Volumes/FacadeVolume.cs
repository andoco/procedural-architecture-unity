using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FacadeVolume : Volume
{
	public FacadeVolume(IStyleConfig styleConfig)
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
//		this.Faces.Add(new Face("face", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }, new SimpleTransform(new Vector3(0f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one)));
		this.Faces.Add(new Face("face", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }, new SimpleTransform(new Vector3(0f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), new Vector3(1f, 1f, 0f))));

		this.Components["face"] = this.Faces[0].Transform; // TODO: possibly need to make new Transform instance?

		var styles = styleConfig.GetByName("facade");
		var faceColor = (Color)styles["face-color"];
		this.Faces[0].Color = faceColor;
	}

	public override void BuildMesh(IMeshBuilder meshBuilder)
	{
		var baseIndex = meshBuilder.Vertices.Count;

		var face = this.Faces[0];

		var verts = face.Corners.Select(c => c.Position).ToArray();

		foreach (var v in verts)
		{
			var worldPos = this.Transform.Position + (this.Transform.Rotation * Vector3.Scale(v, this.Transform.Scale));

			meshBuilder.Vertices.Add(worldPos);
			meshBuilder.UVs.Add(Vector2.zero);
			meshBuilder.Colors.Add(face.Color);
		}
		
		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);		
	}
}