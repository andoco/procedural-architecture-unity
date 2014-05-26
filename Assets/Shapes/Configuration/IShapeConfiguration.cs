using System;
using System.Collections.Generic;
using UnityEngine;

public interface IShapeConfiguration
{
	IScope CurrentScope { get; }

	TreeNode<ShapeNodeValue> RootNode { get; }

	TreeNode<ShapeNodeValue> CurrentNode { get; }

	void PushScope();

	void PopScope();

	void TransformScope(Vector3 delta);

	void RotateScope(Vector3 delta);

	void ScaleScope(Vector3 factor);

	void AddRule(ShapeRule rule);

	void AddShape(string name);
}
