using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;

public class CylinderVolume : Volume
{
	public override void OnBuildVolume(Argument[] args)
	{
		var segmentsArg = args.SingleOrDefault(x => x.Name != null && x.Name.Equals("segments", StringComparison.InvariantCultureIgnoreCase));
		var segments = segmentsArg == null ? 8 : int.Parse(segmentsArg.Value);

		for (int i=0; i <= segments; i++)
		{
			var a = Mathf.PI * 2f / segments * -i;

			var p1 = new Vector3(Mathf.Cos(a) * 0.5f, 0f, Mathf.Sin(a) * 0.5f);
			var p2 = new Vector3(Mathf.Cos(a) * 0.5f, 1f, Mathf.Sin(a) * 0.5f);
			
			this.Corners.Add(new Corner(string.Format("corner-bottom-{0}", i), p1));
			this.Corners.Add(new Corner(string.Format("corner-top-{0}", i), p2));

			var numCorners = this.Corners.Count;
			
			if (i > 0)
			{
				this.Edges.Add(new Edge(string.Format("edge-bottom-{0}", i), this.Corners[this.Corners.Count - 2], this.Corners[this.Corners.Count - 4]));
				this.Edges.Add(new Edge(string.Format("edge-top-{0}", i), this.Corners[this.Corners.Count - 1], this.Corners[this.Corners.Count - 3]));
            }
            
            this.Edges.Add(new Edge(string.Format("edge-side-{0}", i), this.Corners[this.Corners.Count - 2], this.Corners[this.Corners.Count - 1]));

			if (i > 0)
			{
				this.Faces.Add(new Face(string.Format("face-side-{0}", i), new List<Corner> { this.Corners[numCorners - 4], this.Corners[numCorners - 2], this.Corners[numCorners - 1], this.Corners[numCorners - 3] }));
			}
		}

//		// right
//		this.Components.Add(new ScopeComponent("face-vert-1", new SimpleTransform(new Vector3(0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.right), new Vector3(1f, 0f, 1f)), x => x.ToZXY()));
//		// left
//		this.Components.Add(new ScopeComponent("face-vert-3", new SimpleTransform(new Vector3(-0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.left), new Vector3(1f, 0f, 1f)), x => x.ToZXY()));
//		// forward
//		this.Components.Add(new ScopeComponent("face-vert-4", new SimpleTransform(new Vector3(0f, 0.5f, 0.5f), Quaternion.LookRotation(Vector3.up, Vector3.forward), new Vector3(1f, 0f, 1f)), x => x.ToXZY()));
//		// backward
//		this.Components.Add(new ScopeComponent("face-vert-2", new SimpleTransform(new Vector3(0f, 0.5f, -0.5f), Quaternion.LookRotation(Vector3.up, Vector3.back), new Vector3(1f, 0f, 1f)), x => x.ToXZY()));
//		// down
//		this.Components.Add(new ScopeComponent("face-horiz-1", new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.down), new Vector3(1f, 0f, 1f)), x => x));
//		// up
//		this.Components.Add(new ScopeComponent("face-horiz-2", new SimpleTransform(new Vector3(0f, 1f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), new Vector3(1f, 0f, 1f)), x => x));
	}

	public override void ApplyStyle (IStyleConfig styleConfig)
	{
		var faceColor = styleConfig.GetColor(this.Style, "face-color");

		foreach (var f in this.Faces)
			f.Color = faceColor;
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

			meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 3);
			meshBuilder.AddTriangle(baseIndex + 1, baseIndex + 2, baseIndex + 3);

			baseIndex = meshBuilder.Vertices.Count;
		}
	}
}