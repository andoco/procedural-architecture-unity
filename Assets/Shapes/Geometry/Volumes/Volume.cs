using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
	
	public virtual void ApplyTransform(SimpleTransform transform)
	{
		this.Transform.Position = transform.Position;
		this.Transform.Rotation = transform.Rotation;
		this.Transform.Scale = transform.Scale;
	}

	public abstract Mesh BuildMesh();	
}