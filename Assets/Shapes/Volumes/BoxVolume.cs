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
//		this.Faces.Add(new Face("face-vert-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4] }, Vector3.right, new SimpleTransform(new Vector3(0.5f, 0.5f, 0f),  Quaternion.AngleAxis(-90f, Vector3.forward), Vector3.one)));
//		this.Faces.Add(new Face("face-vert-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[6], this.Corners[5] }, Vector3.back, new SimpleTransform(new Vector3(0f, 0.5f, -0.5f), Quaternion.Euler(Vector3.back), Vector3.one)));
//		this.Faces.Add(new Face("face-vert-3", new List<Corner> { this.Corners[2], this.Corners[3], this.Corners[7], this.Corners[6] }, Vector3.left, new SimpleTransform(new Vector3(-0.5f, 0.5f, 0f), Quaternion.Euler(Vector3.left), Vector3.one)));
//		this.Faces.Add(new Face("face-vert-4", new List<Corner> { this.Corners[3], this.Corners[0], this.Corners[4], this.Corners[0] }, Vector3.forward, new SimpleTransform(new Vector3(0f, 0.5f, 0.5f), Quaternion.Euler(Vector3.forward), Vector3.one)));
//
//		this.Faces.Add(new Face("face-horiz-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }, Vector3.down, new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.Euler(Vector3.down), Vector3.one)));
//		this.Faces.Add(new Face("face-horiz-2", new List<Corner> { this.Corners[4], this.Corners[5], this.Corners[6], this.Corners[7] }, Vector3.up, new SimpleTransform(new Vector3(0f, 1f, 0f), Quaternion.Euler(Vector3.up), Vector3.one)));

		this.Faces.Add(new Face("face-vert-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4] }, new SimpleTransform(new Vector3(0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.right, Vector3.up), Vector3.one)));
//		this.Faces.Add(new Face("face-vert-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[6], this.Corners[5] }, new SimpleTransform(new Vector3(0f, 0.5f, -0.5f), Quaternion.LookRotation(Vector3.back, Vector3.up), Vector3.one)));
		this.Faces.Add(new Face("face-vert-2", new List<Corner> { this.Corners[1], this.Corners[2], this.Corners[6], this.Corners[5] }, new SimpleTransform(new Vector3(0f, 0.5f, -0.5f), Quaternion.AngleAxis(180f, Vector3.up), Vector3.one)));
		this.Faces.Add(new Face("face-vert-3", new List<Corner> { this.Corners[2], this.Corners[3], this.Corners[7], this.Corners[6] }, new SimpleTransform(new Vector3(-0.5f, 0.5f, 0f), Quaternion.LookRotation(Vector3.left, Vector3.up), Vector3.one)));
		this.Faces.Add(new Face("face-vert-4", new List<Corner> { this.Corners[3], this.Corners[0], this.Corners[4], this.Corners[0] }, new SimpleTransform(new Vector3(0f, 0.5f, 0.5f), Quaternion.LookRotation(Vector3.forward, Vector3.up), Vector3.one)));
		
		this.Faces.Add(new Face("face-horiz-1", new List<Corner> { this.Corners[0], this.Corners[1], this.Corners[2], this.Corners[3] }, new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.LookRotation(Vector3.down, Vector3.forward), Vector3.one)));
		this.Faces.Add(new Face("face-horiz-2", new List<Corner> { this.Corners[4], this.Corners[5], this.Corners[6], this.Corners[7] }, new SimpleTransform(new Vector3(0f, 1f, 0f), Quaternion.LookRotation(Vector3.up, Vector3.forward), Vector3.one)));

	}
}