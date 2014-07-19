using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeNode : TreeNode
{
	public ShapeNode(string id, ShapeNode parent)
		: base(id, parent)
	{
	}

	public ShapeNodeValue Value { get; set; }
}

public interface IShapeConfiguration
{
	IScope CurrentScope { get; }

	ShapeNode RootNode { get; }

	ShapeNode CurrentNode { get; set; }

	void PushScope();

	void PopScope();

	void SetScope(IScope scope);

	void TransformScope(Vector3 delta);

	void RotateScope(Vector3 delta);

	void ScaleScope(Vector3 factor);

	void AddRule(ShapeRule rule);

	/// <summary>
	/// Adds a shape to the volume of the current scope.
	/// </summary>
	/// <param name="name">Name.</param>
	void AddShape(string name);

	/// <summary>
	/// Adds a volume to the current scope, transformed according to the current matrix of the scope.
	/// </summary>
	/// <param name="name">The name of the volume to add.</param>
	void AddVolume(string name);

	void SplitDivideScope(string axis, float[] sizes, string[] shapes);

	void SplitComponent(string query, string symbol);
}
