using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;

public class GabledRoofVolume : Volume
{
	public GabledRoofVolume()
		: base()
	{
		this.Style = "sided-roof";

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
		this.Faces.Add(new Face("face-top-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4] }));
		this.Faces.Add(new Face("face-top-2", new List<Corner> { this.Corners[4], this.Corners[5], this.Corners[2], this.Corners[3] }));

		this.Faces.Add(new Face("face-end-1", new List<Corner> { this.Corners[0], this.Corners[4], this.Corners[3] }));
		this.Faces.Add(new Face("face-end-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[5] }));

		this.Components.Add(new ScopeComponent(this.Faces[0].Name, new SimpleTransform(new Vector3(0.25f, 0.5f, 0f), Quaternion.AngleAxis(45f, Vector3.forward), Vector3.one), v => v));
		this.Components.Add(new ScopeComponent(this.Faces[1].Name, new SimpleTransform(new Vector3(-0.25f, 0.5f, 0f), Quaternion.AngleAxis(-45, Vector3.forward), Vector3.one), v => v));
		this.Components.Add(new ScopeComponent(this.Faces[2].Name, new SimpleTransform(new Vector3(-0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one), v => v.ToZXY()));
		this.Components.Add(new ScopeComponent(this.Faces[3].Name, new SimpleTransform(new Vector3(0f, 0.5f, 0.5f), Quaternion.LookRotation(Vector3.back, Vector3.up), Vector3.one), v => v.ToZXY()));
	}

	public override void ApplyStyle (IStyleConfig styleConfig)
	{
		var topColor = styleConfig.GetStyle<Color>(this.Style, "top-color");
		var sideColor = styleConfig.GetStyle<Color>(this.Style, "side-color");
		
		this.Faces[0].Color = topColor;
		this.Faces[1].Color = topColor;
		this.Faces[2].Color = sideColor;
		this.Faces[3].Color = sideColor;
	}
	
	public override void BuildMesh(IMeshBuilder meshBuilder)
	{
		var baseIndex = meshBuilder.Vertices.Count;
		
		foreach (var face in this.Faces)
		{
			var verts = face.Corners.Select(c => c.Position).ToArray();
			
			foreach (var v in verts)
			{
				var worldPos = this.Transform.Position + (this.Transform.Rotation * Vector3.Scale(v, this.Transform.Scale));

				meshBuilder.Vertices.Add(worldPos);
				meshBuilder.UVs.Add(Vector2.zero);
				meshBuilder.Colors.Add(face.Color);
			}
			
			meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);

			if (verts.Length == 4)
				meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
			
			baseIndex = meshBuilder.Vertices.Count;
		}
	}
}