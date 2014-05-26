using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class TreeNode<T> : List<TreeNode<T>>
{
	public TreeNode(string id, TreeNode<T> parent)
	{
		this.Id = id;
		this.FullyQualifiedId = parent == null ? id : string.Format("{0}.{1}", parent.FullyQualifiedId, id);
		this.Parent = parent;
	}

	public string Id { get; private set; }

	public string FullyQualifiedId { get; private set; }

	public TreeNode<T> Parent { get; private set; }

	public T Value { get; set; }

	public bool IsRoot { get { return this.Parent == null; } }

	public bool IsLeaf { get { return !this.Any(); } }

	public IEnumerable<TreeNode<T>> LeafNodes()
	{
		return this.Where(child => child.IsLeaf);
	}

	public IEnumerable<TreeNode<T>> LeafNodeDescendants()
	{
		if (this.IsLeaf)
		{
			return new [] { this };
		}

		return this.SelectMany(child => child.LeafNodeDescendants());
	}

	public override string ToString ()
	{
		return string.Format ("[TreeNode({0}): Parent={1}, ChildCount={2}]", FullyQualifiedId, Parent == null ? "NULL" : Parent.FullyQualifiedId, this.Count);
	}
}