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

	void AddShape(string name);

	void AddVolume(string name);

	void SplitDivideScope(string axis, float[] sizes, string[] shapes);

	void SplitComponent(string componentType, string componentParam, string symbol);
}
