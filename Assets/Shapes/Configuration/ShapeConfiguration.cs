using System;
using System.Collections.Generic;
using System.Linq;
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
		this.CurrentScope.Transform.Position += this.CurrentScope.Transform.Rotation * delta;
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

	public void SplitDivideScope(string axis, Size[] sizes, string[] shapes)
	{
		if (sizes.Length != shapes.Length)
			throw new System.ArgumentException("The number of supplied shapes does not match the number of size arguments");
		
		var pos = this.CurrentScope.Transform.Position;
		var rot = this.CurrentScope.Transform.Rotation;
		var scale = this.CurrentScope.Transform.Scale;

		Func<Vector3, Vector3> startPosAction;
		Func<float, Vector3> deltaAction;
		Func<float, Vector3> newScaleAction;

		switch (axis)
		{
		case "X":
			startPosAction = (s) => new Vector3(s.x / 2f, 0f, 0f);
			deltaAction = (s) => new Vector3(s / 2f, 0f, 0f);
			newScaleAction = (s) => new Vector3(s, scale.y, scale.z);
			break;
		case "Y":
			startPosAction = (s) => new Vector3(0f, s.y / 2f, 0f);
			deltaAction = (s) => new Vector3(0f, s / 2f, 0f);
			newScaleAction = (s) => new Vector3(scale.x, s, scale.z);
			break;
		case "Z":
			startPosAction = (s) => new Vector3(0f, 0f, s.z / 2f);
			deltaAction = (s) => new Vector3(0f, 0f, s / 2f);
			newScaleAction = (s) => new Vector3(scale.x, scale.y, s);
			break;
		default:
			throw new ArgumentException(string.Format("Unsupported subdivision axis \"{0}\"", axis), "axis");
		}

		// Start at one end of the selected scope axis.
		var startPos = pos - (rot * startPosAction(scale));

		// Calculate total relative and absolute sizes supplied.
		float totalRelSize = sizes.Select(x => x.IsRelative ? x.Value : 0f).Sum();
		float totalAbsSize = sizes.Select(x => x.IsRelative ? 0f : x.Value).Sum();

		for (int i=0; i < sizes.Length; i++)
		{
			float size;

			// Calculate size of segment based on it being absolute or relative.
			if (sizes[i].IsRelative)
			{
				var relativeSize = sizes[i].Value;
				size = relativeSize * (scale.x - totalAbsSize) / totalRelSize;
			}
			else
			{
				size = sizes[i].Value;
			}

			var delta = rot * deltaAction(size);

			var newScale = newScaleAction(size);
			var newPos = startPos + delta;

			var node = this.NewNode(this.currentNode);
			node.Value.Rule = this.rules[shapes[i]];
			node.Value.Transform = new SimpleTransform(newPos, rot, newScale);
			this.AddNode(node);

			startPos += delta * 2f; // Move to the end of the current segment.
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