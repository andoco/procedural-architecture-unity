using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxVolume : Volume
{
	public BoxVolume()
		: base()
	{
		// Corners
		this.Corners.Add(new Corner("corner-bottom-1", new Vector3(0.5f, 0f, 0.5f)));
		this.Corners.Add(new Corner("corner-bottom-2", new Vector3(0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-bottom-3", new Vector3(-0.5f, 0f, -0.5f)));
		this.Corners.Add(new Corner("corner-bottom-4", new Vector3(-0.5f, 0f, 0.5f)));

		this.Corners.Add(new Corner("corner-top-1", new Vector3(0.5f, 1f, 0.5f)));
		this.Corners.Add(new Corner("corner-top-2", new Vector3(0.5f, 1f, -0.5f)));
		this.Corners.Add(new Corner("corner-top-3", new Vector3(-0.5f, 1f, -0.5f)));
		this.Corners.Add(new Corner("corner-top-4", new Vector3(-0.5f, 1f, 0.5f)));

		// Edges
		this.Edges.Add(new Edge("edge-bottom-1", this.Corners[0], this.Corners[1]));
		this.Edges.Add(new Edge("edge-bottom-2", this.Corners[1], this.Corners[2]));
		this.Edges.Add(new Edge("edge-bottom-3", this.Corners[2], this.Corners[3]));
		this.Edges.Add(new Edge("edge-bottom-4", this.Corners[3], this.Corners[0]));

		this.Edges.Add(new Edge("edge-top-1", this.Corners[4], this.Corners[5]));
		this.Edges.Add(new Edge("edge-top-2", this.Corners[5], this.Corners[6]));
		this.Edges.Add(new Edge("edge-top-3", this.Corners[6], this.Corners[7]));
		this.Edges.Add(new Edge("edge-top-4", this.Corners[7], this.Corners[4]));

		this.Edges.Add(new Edge("edge-side-1", this.Corners[0], this.Corners[4]));
		this.Edges.Add(new Edge("edge-side-2", this.Corners[1], this.Corners[5]));
		this.Edges.Add(new Edge("edge-side-3", this.Corners[2], this.Corners[6]));
		this.Edges.Add(new Edge("edge-side-4", this.Corners[3], this.Corners[7]));

		// Faces
		this.Faces.Add(new Face("face-vert-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4] }));

		this.Faces.Add(new Face("face-vert-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[6], this.Corners[5] }));
		this.Faces.Add(new Face("face-vert-3", new List<Corner> { this.Corners[2], this.Corners[3], this.Corners[7], this.Corners[6] }));
		this.Faces.Add(new Face("face-vert-4", new List<Corner> { this.Corners[3], this.Corners[0], this.Corners[4], this.Corners[7] }));
		
		this.Faces.Add(new Face("face-horiz-1", new List<Corner> { this.Corners[3], this.Corners[2], this.Corners[1], this.Corners[0] }));
		this.Faces.Add(new Face("face-horiz-2", new List<Corner> { this.Corners[4], this.Corners[5], this.Corners[6], this.Corners[7] }));

		// right
		this.Components.Add(new ScopeComponent("face-vert-1", new SimpleTransform(new Vector3(0.5f, 0f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.right), new Vector3(1f, 0.1f, 1f)), x => x.ToZXY()));
		// left
		this.Components.Add(new ScopeComponent("face-vert-3", new SimpleTransform(new Vector3(-0.5f, 0f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.left), new Vector3(1f, 0.1f, 1f)), x => x.ToZXY()));
		// forward
		this.Components.Add(new ScopeComponent("face-vert-4", new SimpleTransform(new Vector3(0f, 0f, 0.5f), Quaternion.LookRotation(Vector3.up, Vector3.forward), new Vector3(1f, 0.1f, 1f)), x => x.ToXZY()));
		// backward
		this.Components.Add(new ScopeComponent("face-vert-2", new SimpleTransform(new Vector3(0f, 0f, -0.5f), Quaternion.LookRotation(Vector3.up, Vector3.back), new Vector3(1f, 0.1f, 1f)), x => x.ToXZY()));
		// down
		this.Components.Add(new ScopeComponent("face-horiz-1", new SimpleTransform(new Vector3(0f, -0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.down), new Vector3(1f, 0.1f, 1f)), x => x));
		// up
		this.Components.Add(new ScopeComponent("face-horiz-2", new SimpleTransform(new Vector3(0f, 0.5f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), new Vector3(1f, 0.1f, 1f)), x => x));
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
			meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);

			baseIndex = meshBuilder.Vertices.Count;
		}
	}
}