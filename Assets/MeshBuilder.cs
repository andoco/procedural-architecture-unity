using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshBuilder : IMeshBuilder
{
	//indices for the triangles:
	private IList<int> indices = new List<int>();

	/// <summary>
	/// The vertex positions of the mesh.
	/// </summary>
	public IList<Vector3> Vertices { get; private set; }
	
	/// <summary>
	/// The vertex normals of the mesh.
	/// </summary>
	public IList<Vector3> Normals { get; private set; }
	
	/// <summary>
	/// The UV coordinates of the mesh.
	/// </summary>
	public IList<Vector2> UVs { get; private set; }

	public IList<Color> Colors { get; private set; }

	public MeshBuilder()
	{
		this.Vertices = new List<Vector3>();
		this.Normals = new List<Vector3>();
		this.UVs = new List<Vector2>();
		this.Colors = new List<Color>();
	}

	/// <summary>
	/// Adds a triangle to the mesh.
	/// </summary>
	/// <param name="index0">The vertex index at corner 0 of the triangle.</param>
	/// <param name="index1">The vertex index at corner 1 of the triangle.</param>
	/// <param name="index2">The vertex index at corner 2 of the triangle.</param>
	public void AddTriangle(int index0, int index1, int index2)
	{
		indices.Add(index0);
		indices.Add(index1);
		indices.Add(index2);
	}

	public Mesh BuildMesh()
	{
		//Create an instance of the Unity Mesh class:
		var mesh = new Mesh();
		
		//add our vertex and triangle values to the new mesh:
		mesh.vertices = this.Vertices.ToArray();
		mesh.triangles = this.indices.ToArray();
		mesh.colors = this.Colors.ToArray();
		
		//Normals are optional. Only use them if we have the correct amount:
		if (this.Normals.Count == this.Vertices.Count)
			mesh.normals = this.Normals.ToArray();
		
		//UVs are optional. Only use them if we have the correct amount:
		if (this.UVs.Count == this.Vertices.Count)
			mesh.uv = this.UVs.ToArray();
		
		//have the mesh recalculate its bounding box (required for proper rendering):
		mesh.RecalculateBounds();
		
		return mesh;
	}

	public void Clear()
	{
		this.Vertices.Clear();
		this.Normals.Clear();
		this.UVs.Clear();
		this.indices.Clear();
	}
}

