using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Andoco.Unity.Framework.Core.Meshes;
using System.Text.RegularExpressions;

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
		var matching = this.Components.Where(c => Regex.IsMatch(c.Name, query));

		return matching;
	}
	
	public void BuildVolume(Argument[] args)
	{
		this.ApplyArguments(args);

		var style = args.SingleOrDefault(x => x.Name == "style");
		if (style != null)
			this.Style = style.Value;

		this.OnBuildVolume(args);
	}
	
	public virtual void ApplyTransform(SimpleTransform transform)
	{
		this.Transform.Position = transform.Position;
		this.Transform.Rotation = transform.Rotation;
		this.Transform.Scale = transform.Scale;
	}

	public virtual void BuildMesh(IMeshBuilder meshBuilder, IStyleConfig styleConfig)
	{
		this.ApplyStyle(styleConfig);

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
			
			if (verts.Length == 3)
			{
				meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
			}
			else if (verts.Length == 4)
			{
				meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 3);
				meshBuilder.AddTriangle(baseIndex + 1, baseIndex + 2, baseIndex + 3);
			}
			else
			{
				throw new InvalidOperationException(string.Format("Cannot build mesh for faces with {0} vertices", verts.Length));
			}
			
			baseIndex = meshBuilder.Vertices.Count;
		}
	}

	public Mesh BuildMesh(IStyleConfig styleConfig)
	{
		var meshBuilder = new MeshBuilder();
		this.BuildMesh(meshBuilder, styleConfig);

		var mesh = meshBuilder.BuildMesh();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();

		return mesh;
	}

	#region Protected methods

	protected virtual void ApplyArguments(Argument[] args)
	{
	}
	
	protected abstract void OnBuildVolume(Argument[] args);
	
	protected virtual void ApplyStyle(IStyleConfig styleConfig)
	{
	}

	#endregion
}