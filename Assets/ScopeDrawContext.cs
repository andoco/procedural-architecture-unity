using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

public class ScopeDrawContext
{
	private int counter = 0;

	public ScopeDrawContext()
	{
		this.RootScope = new TreeNode<IScope>(this.NextId(), null);
		this.RootScope.Value = new Scope(Matrix4x4.identity);
		this.CurrentScope = this.RootScope;
	}

	public TreeNode<IScope> RootScope { get; private set; }

	public TreeNode<IScope> CurrentScope { get; private set; }

	public IDictionary<string, Mesh> Shapes { get; set; }

	public void AddScope(Vector3 trans, Quaternion rot, Vector3 scale)
	{
		Debug.Log(string.Format("Adding scope [trans={0}, rot={1}, scale={2}] to {3}", trans, rot, scale, this.CurrentScope));
		var newMatrix = this.CurrentScope.Value.Matrix * Matrix4x4.TRS(trans, rot, scale);
		this.CurrentScope = this.CurrentScope.AddScope(this.NextId(), newMatrix);
	}

	public void AddShape(string name)
	{
		Debug.Log(string.Format("Adding shape {0} to {1}", name, this.CurrentScope));
		var mesh = this.Shapes[name];
		this.CurrentScope.AddGeometry(this.NextId(), mesh);
	}

	private string NextId()
	{
		return (this.counter++).ToString();
	}
}
