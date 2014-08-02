using UnityEngine;
using Andoco.Core.Graph.Tree;

public static class ShapeConfigurationExtensions
{
	public static void DrawGizmos(this IShapeConfiguration configuration)
	{
		configuration.RootNode.TraverseBreadthFirst(node => {
			var shapeNode = (ShapeNode)node;
			
			if (node.IsLeaf)
			{
				Gizmos.color = Color.white;
			}
			else
			{
				Gizmos.color = Color.grey;
			}
			
			var vol = shapeNode.Value.Volume;
			
			if (vol != null)
			{
				vol.DrawGizmos();
			}
		});
	}
}