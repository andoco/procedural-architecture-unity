using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleTransform
{
	public SimpleTransform()
	{
		this.Position = Vector3.zero;
		this.Rotation = Quaternion.identity;
		this.Scale = Vector3.one;
	}

	public SimpleTransform(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.Position = position;
		this.Rotation = rotation;
		this.Scale = scale;
	}

	public Vector3 Position { get; set; }

	public Quaternion Rotation { get; set; }

	public Vector3 Scale { get; set; }

	public static SimpleTransform operator +(SimpleTransform t1, SimpleTransform t2)
	{
		return new SimpleTransform(t1.Position + t2.Position, t1.Rotation * t2.Rotation, Vector3.Scale(t1.Scale, t2.Scale));
	}

	public override string ToString ()
	{
		return string.Format ("[SimpleTransform: Position={0}, Rotation={1}, Scale={2}]", Position, Rotation, Scale);
	}
}

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
	public Face(string name, IEnumerable<Corner> corners, SimpleTransform transform)
	{
		this.Name = name;
		this.Corners = corners.ToList();
		this.Transform = transform;
		this.Color = Color.grey;
	}

	public string Name { get; set; }
	
	public IList<Corner> Corners { get; private set; }

	public SimpleTransform Transform { get; set; }

	public Color Color { get; set; }

//	public Vector3 GetCentre()
//	{
////		var sum = this.Corners.Sum(x => x.Position);
//		var x = this.Corners.Aggregate(Vector3.zero, (accum, c) => accum += c.Position);
//
//		return x / this.Corners.Count;
//	}
}

public abstract class Volume
{
	public Volume()
	{
		this.Corners = new List<Corner>();
		this.Edges = new List<Edge>();
		this.Faces = new List<Face>();
		this.Components = new Dictionary<string, SimpleTransform>();
		this.Transform = new SimpleTransform();
	}

	public IList<Corner> Corners { get; private set; }
	
	public IList<Edge> Edges { get; private set; }
	
	public IList<Face> Faces { get; private set; }

	public IDictionary<string, SimpleTransform> Components { get; private set; }

	public SimpleTransform Transform { get; private set; }

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

	public IEnumerable<SimpleTransform> Query(string query)
	{
		var matching = this.Components.Where(c => c.Key.StartsWith(query, StringComparison.InvariantCultureIgnoreCase));

		return matching.Select(c => c.Value);
	}
	
	public void ApplyTransform(SimpleTransform transform)
	{
		this.Transform.Position = transform.Position;
		this.Transform.Rotation = transform.Rotation;
		this.Transform.Scale = transform.Rotation * transform.Scale;

		foreach (var c in this.Corners)
		{
			c.Position = this.Transform.Rotation * Vector3.Scale(c.Position, this.Transform.Scale);
		}

		foreach (var f in this.Faces)
		{
			f.Transform.Position = Vector3.Scale(f.Transform.Position, transform.Scale);
		}
	}

	public abstract Mesh BuildMesh();
}