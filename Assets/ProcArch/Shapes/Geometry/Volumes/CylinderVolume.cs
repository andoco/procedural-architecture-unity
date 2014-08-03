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

		var angleDelta = Mathf.PI * 2f / segments;
		var segWidth = Mathf.Sin(angleDelta/2f);

		for (int i=0; i <= segments; i++)
		{
			var a = angleDelta * -i;

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

			if (i > 0)
			{
				var lastFace = this.Faces.Last();

				var upDir = lastFace.GetCentre();
				upDir.y = 0;
				
				this.Components.Add(
					new ScopeComponent(
					string.Format("face-side-{0}", i),
					new SimpleTransform(lastFace.GetCentre(), Quaternion.LookRotation(Vector3.up, upDir), new Vector3(segWidth, 0f, 1f)), x => x.ToZXY()));
			}
		}
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