using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class Scope : IScope
{
	public Scope()
	{
		this.Transform = new SimpleTransform();
	}
	
	public Scope(SimpleTransform transform)
	{
		this.Transform = transform;
	}

	public Scope(IScope scope)
	{
		this.Transform = scope.Transform;
	}

	public SimpleTransform Transform { get; set; }

	public Volume Volume { get; set; }

	public override string ToString ()
	{
		return string.Format ("[Scope: Transform={0}, Volume={1}]", Transform, Volume);
	}
}
