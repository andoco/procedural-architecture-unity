using System;
using System.Collections.Generic;
using System.Linq;

public static class TreeNodeSearchExtensions
{
	public static TreeNode<T> BreadthFirstSearch<T>(this TreeNode<T> root, Predicate<T> match)
	{
		if (match(root.Value))
			return root;

		var queue = new Queue<TreeNode<T>>();
		queue.Enqueue(root);
		
		while (queue.Any())
		{
			var current = queue.Dequeue();
			foreach (var child in current)
			{
				if (match(child.Value))
				{
					return child;
				}

				queue.Enqueue(child);
			}
		}

		return null;
	}
}