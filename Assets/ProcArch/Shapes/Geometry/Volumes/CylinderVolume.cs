using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Meshes;

public class CylinderVolume : Volume
{
	public int NumSegments { get; private set; }

	protected override void ApplyArguments (Argument[] args)
	{
		this.NumSegments = args.GetArgOrDefault("segments", 8);
		if (this.NumSegments < 3)
			throw new ArgumentException(string.Format("CylinderVolume 'segments' minimum value = 3. Actual value = {0}", this.NumSegments), "NumSegments");
	}

	protected override void OnBuildVolume(Argument[] args)
	{
		var angleDelta = Mathf.PI * 2f / this.NumSegments;
		var segWidth = Mathf.Sin(angleDelta/2f);

		var bottomCorner = new Corner("corner-bottom", new Vector3(0f, 0f, 0f));
		this.Corners.Add(bottomCorner);
        
        var topCorner = new Corner("corner-top", new Vector3(0f, 1f, 0f));
		this.Corners.Add(topCorner);

		for (int i=0; i <= this.NumSegments; i++)
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
				this.Faces.Add(new Face(string.Format("face-bottom-{0}", i), new List<Corner> { this.Corners[numCorners - 4], bottomCorner, this.Corners[numCorners - 2] }));
				this.Faces.Add(new Face(string.Format("face-top-{0}", i), new List<Corner> { this.Corners[numCorners - 3], this.Corners[numCorners - 1], topCorner }));
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

		this.Components.Add(new ScopeComponent("face-top", new SimpleTransform(new Vector3(0f, 1f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.up), new Vector3(1f, 0f, 1f)), x => x));
		this.Components.Add(new ScopeComponent("face-bottom", new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.LookRotation(Vector3.forward, Vector3.down), new Vector3(1f, 0f, 1f)), x => x));
        
    }
    
    protected override void ApplyStyle (IStyleConfig styleConfig)
	{
		var faceColor = styleConfig.GetColor(this.Style, this.Theme, "face-color");

		foreach (var f in this.Faces)
			f.Color = faceColor;
	}
}