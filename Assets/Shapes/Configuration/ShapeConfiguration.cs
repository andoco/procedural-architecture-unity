using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeConfiguration : IShapeConfiguration
{
	private readonly Stack<IScope> scopeStack = new Stack<IScope>();
	private int counter;
	private IDictionary<string, ShapeRule> rules;

	public ShapeConfiguration(IDictionary<string, ShapeRule> rules)
	{
		this.rules = rules;
		this.scopeStack.Push(new Scope());
	}

	#region IShapeConfiguration implementation
	
	public IScope CurrentScope
	{
		get { return this.scopeStack.Peek(); }
	}

	public ShapeNode RootNode { get; private set; }

	private ShapeNode currentNode;
	public ShapeNode CurrentNode
	{
		set
		{
			this.currentNode = value;
			this.SetScope(new Scope(this.currentNode.Value.Matrix));
		}
		get
		{
			return this.currentNode;
		}
	}
	
	public void PushScope()
	{
		this.scopeStack.Push(new Scope(this.CurrentScope));
		Debug.Log(string.Format("PUSH: {0}", this.scopeStack.Peek()));
	}

	public void PopScope()
	{
		Debug.Log(string.Format("POP: {0}", this.scopeStack.Peek()));
		this.scopeStack.Pop();
	}

	public void SetScope(IScope scope)
	{
		this.scopeStack.Clear();
		this.scopeStack.Push(scope);
	}

	public void TransformScope(Vector3 delta)
	{
		this.CurrentScope.Matrix *= Matrix4x4.TRS(delta, Quaternion.identity, Vector3.one);
	}
	
	public void RotateScope(Vector3 delta)
	{
		this.CurrentScope.Matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(delta), Vector3.one);
	}
	
	public void ScaleScope(Vector3 scale)
	{
		this.CurrentScope.Matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
	}

	public void AddRule(ShapeRule rule)
	{
		Debug.Log(string.Format("RULE: {0}", rule));
		var node = this.NewNode(this.currentNode);
		node.Value.Rule = rule;

		this.AddNode(node);
	}

	public void AddShape(string name)
	{
		Debug.Log(string.Format("SHAPE: {0}", name));
		var node = this.NewNode(this.currentNode);
		node.Value.ShapeName = name;

		this.AddNode(node);
	}

	public void SplitDivideScope(string axis, float[] sizes, string[] shapes)
	{
		if (sizes.Length != shapes.Length)
			throw new System.ArgumentException("The number of supplied shapes does not match the number of size arguments");

		var pos = this.CurrentScope.Matrix.GetPosition();
		var rot = this.CurrentScope.Matrix.GetRotation();
		var scale = this.CurrentScope.Matrix.GetScale();

		Vector3 axisVector = Vector3.zero;
		if (axis == "X")
			axisVector = new Vector3(1f, 0f, 0f);
		else if (axis == "Y")
			axisVector = new Vector3(0f, 1f, 0f);
		else if (axis == "Z")
			axisVector = new Vector3(0f, 0f, 1f);

		var numDivisions = sizes.Length;

		// Find the position at the start of the current scope.
		var currentPos = pos - (Vector3.Scale(scale, axisVector) * 0.5f);

		for (int i=0; i < numDivisions; i++)
		{
			var node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[i]];

			currentPos += axisVector * sizes[i];

			var p = currentPos - (axisVector * (sizes[i]/2f));
			var r = rot;
			var s = new Vector3(sizes[i], scale.y, scale.z);

			node.Value.Matrix = Matrix4x4.TRS(p, r, s);

			this.AddNode(node);
		}
	}
	
	#endregion

	#region Private methods

	private string NextNodeId()
	{
		return (this.counter++).ToString();
	}

	private ShapeNode NewNode(ShapeNode parent)
	{
		var node = new ShapeNode(this.NextNodeId(), parent)
		{
			Value = new ShapeNodeValue
			{
				Matrix = this.CurrentScope.Matrix
			}
		};

		return node;
	}

	private void AddNode(ShapeNode node)
	{
		if (this.RootNode == null)
		{
			this.RootNode = node;
		}
		else
		{
			if (this.currentNode != node.Parent)
				throw new InvalidOperationException("The parent of the node is not the current node");
			this.currentNode.Add(node);
		}
	}

	#endregion
}