using System;
using System.Collections.Generic;
using UnityEngine;

public interface IShapeConfiguration
{
	IScope CurrentScope { get; }

	TreeNode<ShapeNodeValue> RootNode { get; }

	TreeNode<ShapeNodeValue> CurrentNode { get; }

	void Init(ShapeRule axiom);

	void PushScope();

	void PopScope();

	void TransformScope(Vector3 delta);

	void RotateScope(Vector3 delta);

	void ScaleScope(Vector3 factor);

	void AddRule(ShapeRule rule);

	void AddShape(string name);
}

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

	public void Init(ShapeRule axiom)
	{

	}

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
		// TODO: helper method to create node, and create root if necessary
		var node = this.NewNode();
		node.Value.Rule = rule;

//		var nodeValue = new ShapeNodeValue
//		{
//			Status = ShapeStatus.Active,
//			Rule = rule,
//			Matrix = this.CurrentScope.Matrix
//		};
//		
//		var node = new TreeNode<ShapeNodeValue>(this.NextNodeId(), this.CurrentNode)
//		{
//			Value = nodeValue
//		};
		
//		this.CurrentNode.Add(node);
		this.AddNode(node);
	}

	public void AddShape(string name)
	{
		var newNode = new TreeNode<ShapeNodeValue>(this.NextNodeId(), this.CurrentNode)
		{
			Value = new ShapeNodeValue
			{
				Matrix = this.CurrentScope.Matrix,
				ShapeName = name
			}
		};

		this.CurrentNode.Add(newNode);
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