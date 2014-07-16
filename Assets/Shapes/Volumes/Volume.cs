using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Corner
{
	public Corner(string name, Vector3 pos)
	{
		this.Name = name;
		this.Position = pos;
	}

	public string Name { get; set; }
	
	public Vector3 Position { get; set; }
}

public class Edge
{
	public Edge(string name, Corner a, Corner b)
	{
		this.Name = name;
		this.CornerA = a;
		this.CornerB = b;
	}

	public string Name { get; set; }
	
	public Corner CornerA { get; set; }
	
	public Corner CornerB { get; set; }
}

public class Face
{
	public Face(string name, IEnumerable<Corner> corners)
	{
		this.Name = name;
		this.Corners = corners.ToList();
	}

	public string Name { get; set; }
	
	public IList<Corner> Corners { get; private set; }
}

public class Volume
{
	public Volume()
	{
		this.Corners = new List<Corner>();
		this.Edges = new List<Edge>();
		this.Faces = new List<Face>();
	}

	public IList<Corner> Corners { get; private set; }
	
	public IList<Edge> Edges { get; private set; }
	
	public IList<Face> Faces { get; private set; }
	
	public IEnumerable<Corner> GetCorners(string query)
	{
		return this.Corners.Where(c => c.Name.StartsWith(query));
	}
	
	public IEnumerable<Edge> GetEdges(string query)
	{
		return this.Edges.Where(e => e.Name.StartsWith(query));
	}
	
	public IEnumerable<Face> GetFaces(string query)
	{
		return this.Faces.Where(f => f.Name.StartsWith(query));
	}
	
	public void ApplyTransform(Matrix4x4 matrix)
	{
		foreach (var c in this.Corners)
		{
			c.Position = matrix.MultiplyPoint3x4(c.Position);
		}
	}
}