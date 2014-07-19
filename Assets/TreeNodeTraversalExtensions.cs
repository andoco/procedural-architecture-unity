using System;
using System.Collections.Generic;
using System.Linq;

public static class TreeNodeTraversalExtensions
{
	public delegate void TreeNodeAction(TreeNode node);

	public delegate void TreeNodeDepthFirstAction(TreeNode node, int depth);

	public static void TraverseBreadthFirst(this TreeNode root, TreeNodeAction action)
	{
		var queue = new Queue<TreeNode>();
		queue.Enqueue(root);
		
		while (queue.Any())
		{
			var current = queue.Dequeue();
			current.ForEach(queue.Enqueue);
			action(current);
		}
	}

	public static void TraverseDepthFirst(this TreeNode root, TreeNodeDepthFirstAction action)
	{
		TraverseDepthFirst(root, 0, action);
	}

	private static void TraverseDepthFirst(this TreeNode root, int depth, TreeNodeDepthFirstAction action)
	{
		action(root, depth);
		
		foreach (var child in root)
		{
			TraverseDepthFirst(child, depth + 1, action);
		}
	}
}