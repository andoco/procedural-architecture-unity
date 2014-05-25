using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshBuilderExtensions
{
	/// <summary>
	/// Builds a single quad in the XZ plane, facing up the Y axis.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="width">The width of the quad.</param>
	/// <param name="length">The length of the quad.</param>
	public static void BuildQuad(this IMeshBuilder meshBuilder, Vector3 offset, float width, float length)
	{
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);
		
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);
		
		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);
		
		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);
		
		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;
        
        meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
        meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	/// <summary>
	/// Builds a single quad based on a position offset and width and length vectors.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="widthDir">The width vector of the quad.</param>
	/// <param name="lengthDir">The length vector of the quad.</param>
	public static void BuildQuad(this IMeshBuilder meshBuilder, Vector3 offset, Vector3 widthDir, Vector3 lengthDir)
	{
		Vector3 normal = Vector3.Cross(lengthDir, widthDir).normalized;
		
		meshBuilder.Vertices.Add(offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(offset + lengthDir);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(offset + lengthDir + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(offset + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(normal);
		
		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;
		
		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	public static void BuildCube(this IMeshBuilder meshBuilder, float width, float height, float length)
	{
		//calculate directional vectors for all 3 dimensions of the cube:
		Vector3 upDir = Vector3.up * height;
		Vector3 rightDir = Vector3.right * width;
		Vector3 forwardDir = Vector3.forward * length;
		
		//calculate the positions of two corners opposite each other on the cube:
		
		//positions that will place the pivot at the corner of the cube:
		Vector3 nearCorner = Vector3.zero;
		Vector3 farCorner = upDir + rightDir + forwardDir;
		
		////positions that will place the pivot at the centre of the cube:
		//Vector3 farCorner = (upDir + rightDir + forwardDir) / 2;
		//Vector3 nearCorner = -farCorner;
		
		//build the 3 quads that originate from nearCorner:
		BuildQuad(meshBuilder, nearCorner, forwardDir, rightDir);
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);
		
		//build the 3 quads that originate from farCorner:
		BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
		BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
	}

	/// <summary>
	/// Builds a single triangle.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="corner0">The vertex position at index 0 of the triangle.</param>
	/// <param name="corner1">The vertex position at index 1 of the triangle.</param>
	/// <param name="corner2">The vertex position at index 2 of the triangle.</param>
	public static void BuildTriangle(this IMeshBuilder meshBuilder, Vector3 corner0, Vector3 corner1, Vector3 corner2)
	{
		Vector3 normal = Vector3.Cross((corner1 - corner0), (corner2 - corner0)).normalized;
		
		meshBuilder.Vertices.Add(corner0);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(corner1);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(corner2);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		int baseIndex = meshBuilder.Vertices.Count - 3;
        
        meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
	}

	public static void BuildFacade(this IMeshBuilder meshBuilder, float width, float height, float length)
	{
		//build the walls:
		
		//calculate directional vectors for the walls:
		Vector3 upDir = Vector3.up * height;
		Vector3 rightDir = Vector3.right * width;
		Vector3 forwardDir = Vector3.forward * length;
		
		Vector3 farCorner = upDir + rightDir + forwardDir;
		Vector3 nearCorner = Vector3.zero;
		
		//shift the pivot to centre bottom:
		Vector3 pivotOffset = (rightDir + forwardDir) * 0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;
		
		//build the quads for the walls:
		BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
		BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);
		
		BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
        BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
    }

	public static void BuildRoof(this IMeshBuilder meshBuilder, float width, float length, float roofHeight, float roofOverhangFront, float roofOverhangSide, float roofBias)
	{
		//calculate directional vectors for the walls:
		Vector3 rightDir = Vector3.right * width;
		Vector3 forwardDir = Vector3.forward * length;

		Vector3 farCorner = rightDir + forwardDir;
		Vector3 nearCorner = Vector3.zero;
		
		//shift the pivot to centre bottom:
		Vector3 pivotOffset = (rightDir + forwardDir) * 0.5f;
		farCorner -= pivotOffset;
		nearCorner -= pivotOffset;

		//build the roof:
		
		//calculate the position of the roof peak at the near end of the house:
		Vector3 roofPeak = Vector3.up * roofHeight + rightDir * 0.5f - pivotOffset;
		
		//calculate the positions at the tops of the walls at the same end of the house:
		Vector3 wallTopLeft = - pivotOffset;
		Vector3 wallTopRight = rightDir - pivotOffset;
		
		//build triangles at the tops of the walls:
		BuildTriangle(meshBuilder, wallTopLeft, roofPeak, wallTopRight);
		BuildTriangle(meshBuilder, wallTopLeft + forwardDir, wallTopRight + forwardDir, roofPeak + forwardDir);
		
		//calculate the directions from the roof peak to the sides of the house:
		Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
		Vector3 dirFromPeakRight = wallTopRight - roofPeak;
		
		//extend the directions by a length of m_RoofOverhangSide:
		dirFromPeakLeft += dirFromPeakLeft.normalized * roofOverhangSide;
		dirFromPeakRight += dirFromPeakRight.normalized * roofOverhangSide;
		
		//offset the roofpeak position to put it at the beginning of the front overhang:
		roofPeak -= Vector3.forward * roofOverhangFront;
		
		//extend the forward directional vecter ot make it long enough for and overhang at either end:
		forwardDir += Vector3.forward * roofOverhangFront * 2.0f;
		
		//shift the roof slightly upward to stop it intersecting the top of the walls:
		roofPeak += Vector3.up * roofBias;
		
		//build the quads for the roof:
        BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakLeft);
        BuildQuad(meshBuilder, roofPeak, dirFromPeakRight, forwardDir);
        
        BuildQuad(meshBuilder, roofPeak, dirFromPeakLeft, forwardDir);
        BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakRight);
    }
}