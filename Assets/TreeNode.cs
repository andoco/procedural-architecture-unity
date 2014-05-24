using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class TreeNode : List<TreeNode>
{
	protected TreeNode(string id, TreeNode parent)
	{
		this.Id = id;
		this.FullyQualifiedId = parent == null ? id : string.Format("{0}.{1}", parent.FullyQualifiedId, id);
		this.Parent = parent;
	}

	public string Id { get; private set; }

	public string FullyQualifiedId { get; private set; }

	public TreeNode Parent { get; private set; }

	public bool IsRoot { get { return this.Parent == null; } }

	public bool IsLeaf { get { return !this.Any(); } }

	public IEnumerable<TreeNode> LeafNodes()
	{
		return this.Where(child => child.IsLeaf);
	}

	public IEnumerable<TreeNode> LeafNodeDescendants()
	{
		if (this.IsLeaf)
			yield return this;

		foreach (var child in this)
		{
			Debug.Log(child);
			foreach (var leaf in child.LeafNodes())
				yield return leaf;
		}
	}

	public override string ToString ()
	{
		return string.Format ("[Node({0} {1}): Parent={2} IsLeaf={3}]", GetType().Name, FullyQualifiedId, Parent.FullyQualifiedId, IsLeaf);
	}
}
