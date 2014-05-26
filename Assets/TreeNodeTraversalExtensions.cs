using System;
using System.Collections.Generic;
using System.Linq;

public static class TreeNodeTraversalExtensions
{
	public static void TraverseBreadthFirst<T>(this TreeNode<T> root, Action<TreeNode<T>> action)
	{
		var queue = new Queue<TreeNode<T>>();
		queue.Enqueue(root);
		
		while (queue.Any())
		{
			var current = queue.Dequeue();
			current.ForEach(queue.Enqueue);
			action(current);
		}
	}
}