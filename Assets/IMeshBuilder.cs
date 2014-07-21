using System.Collections.Generic;
using UnityEngine;

public interface IMeshBuilder
{
	#region Properties

	/// <summary>
	/// The vertex positions of the mesh.
	/// </summary>
	IList<Vector3> Vertices { get; }
	
	/// <summary>
	/// The vertex normals of the mesh.
	/// </summary>
	IList<Vector3> Normals { get; }
	
	/// <summary>
	/// The UV coordinates of the mesh.
	/// </summary>
	IList<Vector2> UVs { get; }

	IList<Color> Colors { get; }

	#endregion

	#region Methods

	Mesh BuildMesh();

	void Clear();

	/// <summary>
	/// Adds a triangle to the mesh.
	/// </summary>
	/// <param name="index0">The vertex index at corner 0 of the triangle.</param>
	/// <param name="index1">The vertex index at corner 1 of the triangle.</param>
	/// <param name="index2">The vertex index at corner 2 of the triangle.</param>
	void AddTriangle(int index0, int index1, int index2);

	#endregion
}