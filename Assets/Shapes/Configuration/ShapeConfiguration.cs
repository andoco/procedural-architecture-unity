using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeConfiguration : IShapeConfiguration
{
	private readonly Stack<IScope> scopeStack = new Stack<IScope>();
	private int counter;

	public ShapeConfiguration()
	{
		this.scopeStack.Push(new Scope(Matrix4x4.identity));
	}

	#region IShapeConfiguration implementation
	
	public IScope CurrentScope
	{
		get { return this.scopeStack.Peek(); }
	}

	public TreeNode<ShapeNodeValue> RootNode { get; private set; }

	public TreeNode<ShapeNodeValue> CurrentNode { get; private set; }
	
	public void PushScope()
	{
		this.scopeStack.Push(new Scope(this.CurrentScope));
	}

	public void PopScope()
	{
		this.scopeStack.Pop();
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
		var node = this.NewNode();
		node.Value.Rule = rule;

		this.AddNode(node);
	}

	public void AddShape(string name)
	{
		var node = this.NewNode();
		node.Value.ShapeName = name;

		this.AddNode(node);
	}
	
	#endregion

	#region Private methods

	private string NextNodeId()
	{
		return (this.counter++).ToString();
	}

	private TreeNode<ShapeNodeValue> NewNode()
	{
		var node = new TreeNode<ShapeNodeValue>(this.NextNodeId(), this.CurrentNode)
		{
			Value = new ShapeNodeValue
			{
				Matrix = this.CurrentScope.Matrix
			}
		};

		return node;
	}

	private void AddNode(TreeNode<ShapeNodeValue> node)
	{
		if (this.RootNode == null)
		{
			this.RootNode = node;
			this.CurrentNode = node;
		}
		else
		{
			this.CurrentNode.Add(node);
		}
	}

	#endregion
}