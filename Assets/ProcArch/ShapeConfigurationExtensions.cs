namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using Andoco.Core.Graph.Tree;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    
    public static class ShapeConfigurationExtensions
    {
        public static void DrawGizmos (this IShapeConfiguration configuration, Transform parent, bool showCorners = true, bool showEdges = true, bool showComponents = true)
        {
            configuration.RootNode.TraverseBreadthFirst (node => {
                var shapeNode = (ShapeNode)node;
    
                var vol = shapeNode.Value.Volume;
                
                if (vol != null) {
                    vol.DrawGizmos (parent);
                    if (showCorners)
                        vol.DrawCornerGizmos (parent);
                    if (showEdges)
                        vol.DrawEdgeGizmos(parent);
                    if (showComponents)
                        vol.DrawComponentGizmos(parent);
                }
            });
        }
    }
}