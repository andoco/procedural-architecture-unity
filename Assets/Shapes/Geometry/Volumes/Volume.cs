using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core.Meshes;

public abstract class Volume
{
	public Volume()
	{
		this.Corners = new List<Corner>();
		this.Edges = new List<Edge>();
		this.Faces = new List<Face>();
		this.Components = new List<ScopeComponent>();
		this.Transform = new SimpleTransform();
		this.Style = StyleConfig.DefaultStyle;
	}

	public IList<Corner> Corners { get; private set; }
	
	public IList<Edge> Edges { get; private set; }
	
	public IList<Face> Faces { get; private set; }

	public IList<ScopeComponent> Components { get; private set; }

	public SimpleTransform Transform { get; private set; }

	public string Style { get; set; }

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

	public IEnumerable<ScopeComponent> Query(string query)
	{
		var matching = this.Components.Where(c => c.Name.StartsWith(query, StringComparison.InvariantCultureIgnoreCase));

		return matching;
	}
	
	public virtual void ApplyTransform(SimpleTransform transform)
	{
		this.Transform.Position = transform.Position;
		this.Transform.Rotation = transform.Rotation;
		this.Transform.Scale = transform.Scale;
	}

	public virtual void ApplyStyle(IStyleConfig styleConfig)
	{
	}
	
	public abstract void BuildMesh(IMeshBuilder meshBuilder);

	public Mesh BuildMesh()
	{
		var meshBuilder = new MeshBuilder();
		this.BuildMesh(meshBuilder);

		var mesh = meshBuilder.BuildMesh();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();

		return mesh;
	}
}