using System;
using UnityEngine;

public class ScopeComponent
{
	public ScopeComponent(string name, SimpleTransform tx, Func<Vector3, Vector3> axisMap)
	{
		this.Name = name;
		this.Transform = tx;
		this.AxisMap = axisMap;
	}
	
	public string Name { get; private set; }
	
	public SimpleTransform Transform { get; private set; }
	
	public Func<Vector3, Vector3> AxisMap { get; private set; }
}