﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Demo : MonoBehaviour {

	public Mesh cubeMesh;
	public Mesh rootMesh;

	// Use this for initialization
	void Start () {
		var root = new ScopeNode("root", null, Matrix4x4.identity);

		var facadeScope = root.AddScopeNode("facadeScope", root.Matrix);
		facadeScope.AddGeometryNode("geo1", this.cubeMesh);

		var roofScope = root.AddScopeNode("rootScope", root.Matrix * Matrix4x4.TRS(Vector3.up*1.5f, Quaternion.Euler(Vector3.right*90f), Vector3.one));
		roofScope.AddGeometryNode("geo2", this.rootMesh);

		foreach (var leaf in root.LeafNodeDescendants())
		{
			Debug.Log(leaf);

			if (leaf is GeometryNode)
			{
				var geoNode = (GeometryNode)leaf;
				AddGeometryToScene(geoNode);
			}
		}
	}

	private void AddGeometryToScene(GeometryNode geoNode)
	{
		var go = new GameObject();
		go.transform.FromMatrix4x4(geoNode.Matrix);
		var meshFilter = go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		
		meshFilter.sharedMesh = geoNode.Geometry;
	}
}

public class ScopeNode : TreeNode
{
	public ScopeNode(string id, TreeNode parent, Matrix4x4 matrix)
		: base(id, parent)
	{
		this.Matrix = matrix;
	}

	public Matrix4x4 Matrix { get; private set; }	
}

public class GeometryNode : ScopeNode
{
	public GeometryNode(string id, ScopeNode parent, Mesh geometry)
		: base(id, parent, parent.Matrix)
	{
		this.Geometry = geometry;
	}

	public Mesh Geometry { get; private set; }
}

public static class NodeExtensions
{
	public static ScopeNode AddScopeNode(this ScopeNode root, string id, Matrix4x4 matrix)
	{
		var node = new ScopeNode(id, root, matrix);
		root.Add(node);
		return node;
	}

	public static GeometryNode AddGeometryNode(this ScopeNode root, string id, Mesh geometry)
	{
		var node = new GeometryNode(id, root, geometry);
		root.Add(node);
		return node;
	}
}