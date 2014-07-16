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
		this.Matrix = Matrix4x4.identity;
	}

	public Scope(Matrix4x4 matrix)
	{
		this.Matrix = matrix;
	}

	public Scope(IScope scope)
	{
		this.Matrix = scope.Matrix;
	}

	public Matrix4x4 Matrix { get; set; }

	public Volume Volume { get; set; }
	
	public override string ToString ()
	{
		var matrixInfo = this.Matrix == null ? string.Empty : string.Format("[Matrix: (pos={0}, rot={1}, scale={2})]", Matrix.GetPosition(), Matrix.GetRotation(), Matrix.GetScale());
		return string.Format("[Scope: Matrix={0}]", matrixInfo);
	}
}
