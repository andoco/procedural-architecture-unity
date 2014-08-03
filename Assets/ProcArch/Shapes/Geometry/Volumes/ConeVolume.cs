using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;

public class ConeVolume : Volume
{
	protected override void OnBuildVolume(Argument[] args)
	{
		var segmentsArg = args.SingleOrDefault(x => x.Name != null && x.Name.Equals("segments", StringComparison.InvariantCultureIgnoreCase));
		var segments = segmentsArg == null ? 8 : int.Parse(segmentsArg.Value);

		var angleDelta = Mathf.PI * 2f / segments;
		var segWidth = Mathf.Sin(angleDelta/2f);

		var bottomCorner = new Corner("corner-bottom", new Vector3(0f, 0f, 0f));
		this.Corners.Add(bottomCorner);
        
        var topCorner = new Corner("corner-top", new Vector3(0f, 1f, 0f));
		this.Corners.Add(topCorner);

		for (int i=0; i <= segments; i++)
		{
			var a = angleDelta * -i;

			var p1 = new Vector3(Mathf.Cos(a) * 0.5f, 0f, Mathf.Sin(a) * 0.5f);
			
			this.Corners.Add(new Corner(string.Format("corner-bottom-{0}", i), p1));

			var numCorners = this.Corners.Count;
			
			if (i > 0)
			{
				this.Edges.Add(new Edge(string.Format("edge-bottom-{0}", i), this.Corners[numCorners - 2], this.Corners[numCorners - 1]));
            }
            
            this.Edges.Add(new Edge(string.Format("edge-side-{0}", i), this.Corners[numCorners - 2], this.Corners[numCorners - 1]));

			if (i > 0)
			{
				this.Faces.Add(new Face(string.Format("face-side-{0}", i), new List<Corner> { this.Corners[numCorners - 2], this.Corners[numCorners - 1], topCorner }));
				this.Faces.Add(new Face(string.Format("face-bottom-{0}", i), new List<Corner> { this.Corners[numCorners - 2], bottomCorner, this.Corners[numCorners - 1] }));
			}
		}

		foreach (var face in this.Faces.Where(f => f.Name.StartsWith("face-side", StringComparison.InvariantCultureIgnoreCase)))
		{
			var upDir = face.GetCentre();
			upDir.y = 0;
			
			this.Components.Add(
				new ScopeComponent(
				face.Name,
				new SimpleTransform(face.GetCentre(), Quaternion.LookRotation(Vector3.up, upDir), new Vector3(segWidth, 0f, 1f)), x => x.ToZXY()));
        }

		this.Components.Add(new ScopeComponent("face-bottom", new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.down), new Vector3(1f, 0f, 1f)), x => x));
    }
    
    protected override void ApplyStyle(IStyleConfig styleConfig)
	{
		var faceColor = styleConfig.GetColor(this.Style, "face-color");

		foreach (var f in this.Faces)
			f.Color = faceColor;
	}
}