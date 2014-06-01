using System;
using System.Collections.Generic;
using System.Linq;

public static class TreeNodeTraversalExtensions
{
	public delegate void TreeNodeAction(TreeNode node);

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
}