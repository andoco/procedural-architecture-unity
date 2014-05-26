using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class Scope : IScope
{
	public Scope(Matrix4x4 matrix)
	{
		this.Matrix = matrix;
	}

	public Scope(IScope scope)
	{
		this.Matrix = scope.Matrix;
	}

	public Matrix4x4 Matrix { get; set; }	
}
