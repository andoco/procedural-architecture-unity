namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using Andoco.Core.Graph.Tree;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    
    public static class ShapeConfigurationExtensions
    {
        public static void DrawGizmos (this IShapeConfiguration configuration, Transform parent)
        {
            configuration.RootNode.TraverseBreadthFirst (node => {
                var shapeNode = (ShapeNode)node;
    
                var vol = shapeNode.Value.Volume;
                
                if (vol != null) {
                    vol.DrawGizmos (parent);
                    vol.DrawCornerGizmos (parent);
                    vol.DrawEdgeGizmos(parent);
                    vol.DrawComponentGizmos(parent);
                }
            });
        }
    }
}