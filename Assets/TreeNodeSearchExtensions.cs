using System;
using System.Collections.Generic;
using System.Linq;

public static class TreeNodeSearchExtensions
{
	public static TreeNode BreadthFirstSearch(this TreeNode root, Predicate<TreeNode> match)
	{
		if (match(root))
			return root;

		var queue = new Queue<TreeNode>();
		queue.Enqueue(root);
		
		while (queue.Any())
		{
			var current = queue.Dequeue();
			foreach (var child in current)
			{
				if (match(child))
				{
					return child;
				}

				queue.Enqueue(child);
			}
		}

		return null;
	}
}