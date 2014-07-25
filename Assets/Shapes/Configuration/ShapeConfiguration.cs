using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeConfiguration : IShapeConfiguration
{
	private readonly Stack<IScope> scopeStack = new Stack<IScope>();
	private int counter;
	private readonly IDictionary<string, ShapeRule> rules;
	private readonly IStyleConfig styleConfig;
	private ShapeNode currentNode;

	public ShapeConfiguration(IDictionary<string, ShapeRule> rules, IStyleConfig styleConfig)
	{
		this.rules = rules;
		this.styleConfig = styleConfig;
		this.scopeStack.Push(new Scope());
	}

	#region IShapeConfiguration implementation
	
	public IScope CurrentScope
	{
		get { return this.scopeStack.Peek(); }
	}

	public ShapeNode RootNode { get; private set; }
	
	public ShapeNode CurrentNode
	{
		set
		{
			this.currentNode = value;
			this.SetScope(new Scope(this.currentNode.Value.Transform));
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
		this.CurrentScope.Transform.Position += delta;
		Debug.Log(string.Format("TRANSFORMED: {0} {1}", delta, this.CurrentScope.Transform));
	}
	
	public void RotateScope(Vector3 delta)
	{
		this.CurrentScope.Transform.Rotation *= Quaternion.Euler(delta);
		Debug.Log(string.Format("ROTATED: {0} {1}", delta, this.CurrentScope.Transform));
	}
	
	public void ScaleScope(Vector3 scale)
	{
		var s = this.CurrentScope.Transform.Scale;
		s.Scale(scale);
		this.CurrentScope.Transform.Scale = s;
		Debug.Log(string.Format("SCALED: {0} {1}", scale, this.CurrentScope.Transform));
	}

	public void AddRule(ShapeRule rule)
	{
		Debug.Log(string.Format("RULE: {0}", rule));
		var node = this.NewNode(this.currentNode);
		node.Value.Rule = rule;

		this.AddNode(node);
	}
	
	public void AddVolume(string name)
	{
		Debug.Log(string.Format("VOLUME: {0}, {1}", name, this.CurrentScope.Transform));

		var vol = (Volume)Activator.CreateInstance(Type.GetType(name + "Volume", true, true), this.styleConfig);
		vol.ApplyTransform(this.CurrentScope.Transform);

		this.currentNode.Value.Volume = vol;
	}

	public void SplitDivideScopeOld(string axis, float[] sizes, string[] shapes)
	{
		if (sizes.Length != shapes.Length)
			throw new System.ArgumentException("The number of supplied shapes does not match the number of size arguments");

		var pos = this.CurrentScope.Transform.Position;
		var rot = this.CurrentScope.Transform.Rotation;
		var scale = this.CurrentScope.Transform.Scale;

		Vector3 axisVector = Vector3.zero;
		if (axis == "X")
			axisVector = new Vector3(1f, 0f, 0f);
		else if (axis == "Y")
			axisVector = new Vector3(0f, 1f, 0f);
		else if (axis == "Z")
			axisVector = new Vector3(0f, 0f, 1f);

		// Get vector with the other axes so we can maintain existing scale on those axes.
		var oppAxisVector = new Vector3(1f-axisVector.x, 1f-axisVector.y, 1f-axisVector.z);

		axisVector = rot * axisVector;
		oppAxisVector = rot * oppAxisVector;

		var numDivisions = sizes.Length;

		// Find the position at the start of the current scope.
		var currentPos = pos - (Vector3.Scale(scale, axisVector) * 0.5f);

		for (int i=0; i < numDivisions; i++)
		{
			currentPos += axisVector * sizes[i];

			var p = currentPos - (axisVector * (sizes[i]/2f));
			var r = rot;
			var s = (axisVector * sizes[i]) + Vector3.Scale(scale, oppAxisVector);

			Debug.Log(s);

			var node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[i]];
			node.Value.Transform = new SimpleTransform(p, r, s);

			this.AddNode(node);
		}
	}

	public void SplitDivideScope(string axis, float[] sizes, string[] shapes)
	{
		if (sizes.Length != shapes.Length)
			throw new System.ArgumentException("The number of supplied shapes does not match the number of size arguments");
		
		var pos = this.CurrentScope.Transform.Position;
		var rot = this.CurrentScope.Transform.Rotation;
		var scale = this.CurrentScope.Transform.Scale;

		if (axis == "X")
		{
			var newScale = new Vector3(scale.x / 2f, scale.y, scale.z);
			var newPos1 = pos + (rot * (new Vector3(newScale.x / 2f, 0f, 0f)));
			var newPos2 = pos - (rot * (new Vector3(newScale.x / 2f, 0f, 0f)));

			var node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[0]];
			node.Value.Transform = new SimpleTransform(newPos1, rot, newScale);
			this.AddNode(node);

			node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[1]];
			node.Value.Transform = new SimpleTransform(newPos2, rot, newScale);
			this.AddNode(node);
		}
		else if (axis == "Y")
		{
			var newScale = new Vector3(scale.x, scale.y / 2f, scale.z);
			var newPos1 = pos + (rot * (new Vector3(0f, newScale.y / 2f, 0f)));
			var newPos2 = pos - (rot * (new Vector3(0f, newScale.y / 2f, 0f)));
			
			var node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[0]];
			node.Value.Transform = new SimpleTransform(newPos1, rot, newScale);
			this.AddNode(node);
			
			node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[1]];
			node.Value.Transform = new SimpleTransform(newPos2, rot, newScale);
			this.AddNode(node);
		}
		else
		{
			throw new ArgumentException(string.Format("Unsupported subdivision axis \"{0}\"", axis), "axis");
		}
	}

	public void SplitComponent(string query, string symbol)
	{
		Debug.Log(string.Format("COMP: {0}, {1}", query, this.CurrentScope.Transform));

		var currentVol = currentNode.Value.Volume;
		var componentTransforms = currentVol.Query(query);

		foreach (var trans in componentTransforms)
		{
			// Get the correct position of the component based on the current scope and volume.
			var newPos = this.CurrentScope.Transform.Position + (this.CurrentScope.Transform.Rotation * Vector3.Scale(currentVol.Transform.Scale, trans.Position));

			// Get the correct rotation of the component based on the current scope.
			var newRot = this.CurrentScope.Transform.Rotation * trans.Rotation;

			// Rotate the volume's scale by the rotation of the component so that we can apply the correct scale to the component.
			// Rotating a scale vector can result in negative values, so we need to make sure that they are all positive.
			var s = currentVol.Transform.Scale;
			s = trans.Rotation * s;
			s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
			var newScale = s;

			var newTrans = new SimpleTransform(newPos, newRot, newScale);

			var node = this.NewNode(this.currentNode);
			node.Value.Transform = newTrans;
			node.Value.Rule = this.rules[symbol];
			
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
				Transform = new SimpleTransform(this.CurrentScope.Transform)
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