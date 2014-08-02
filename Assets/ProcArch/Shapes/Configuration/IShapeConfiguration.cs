using System;
using System.Collections.Generic;
using UnityEngine;
using Andoco.Core.Graph.Tree;

public class ShapeNode : TreeGraphNode
{
	public ShapeNode(string id, ShapeNode parent)
		: base(id, parent)
	{
	}

	public ShapeNodeValue Value { get; set; }
}

public interface IShapeConfiguration
{
	IDictionary<string, ShapeRule> Rules { get; }

	IScope CurrentScope { get; }

	ShapeNode RootNode { get; }

	ShapeNode CurrentNode { get; set; }

	void AddGlobalArgs(IDictionary<string, string> args);

	void PushScope();

	void PopScope();

	void SetScope(IScope scope);

	void TransformScope(Vector3 delta);

	void RotateScope(Vector3 delta);

	void ScaleScope(Size x, Size y, Size z);

	void AddRule(ShapeRule rule, IList<string> args);
	
	/// <summary>
	/// Adds a volume to the current scope, transformed according to the current matrix of the scope.
	/// </summary>
	/// <param name="name">The name of the volume to add.</param>
	void AddVolume(string name, string style);

	void SplitDivideScope(string axis, Size[] sizes, ShapeSymbol[] shapes);

	void SplitComponent(string query, ShapeSymbol symbol);

	string[] ResolveArgs(IEnumerable<string> unresolvedArgs);
}
