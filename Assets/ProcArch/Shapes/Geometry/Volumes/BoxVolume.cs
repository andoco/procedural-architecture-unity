namespace Andoco.Unity.ProcArch.Shapes.Geometry.Volumes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Core.Meshes;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    using Andoco.Unity.ProcArch.Shapes.Styles;

    public class BoxVolume : Volume
    {
        protected override void OnBuildVolume (Argument[] args)
        {
            // Corners
            this.AddCorner("corner-bottom-1", new Vector3 (0.5f, 0f, 0.5f));
            this.AddCorner("corner-bottom-2", new Vector3 (0.5f, 0f, -0.5f));
            this.AddCorner("corner-bottom-3", new Vector3 (-0.5f, 0f, -0.5f));
            this.AddCorner("corner-bottom-4", new Vector3 (-0.5f, 0f, 0.5f));
            
            this.AddCorner("corner-top-1", new Vector3 (0.5f, 1f, 0.5f));
            this.AddCorner("corner-top-2", new Vector3 (0.5f, 1f, -0.5f));
            this.AddCorner("corner-top-3", new Vector3 (-0.5f, 1f, -0.5f));
            this.AddCorner("corner-top-4", new Vector3 (-0.5f, 1f, 0.5f));
            
            // Edges
            this.AddEdge("edge-bottom-1", this.Corners [0], this.Corners [1]);
            this.AddEdge("edge-bottom-2", this.Corners [1], this.Corners [2]);
            this.AddEdge("edge-bottom-3", this.Corners [2], this.Corners [3]);
            this.AddEdge("edge-bottom-4", this.Corners [3], this.Corners [0]);
            
            this.AddEdge("edge-top-1", this.Corners [4], this.Corners [5]);
            this.AddEdge("edge-top-2", this.Corners [5], this.Corners [6]);
            this.AddEdge("edge-top-3", this.Corners [6], this.Corners [7]);
            this.AddEdge("edge-top-4", this.Corners [7], this.Corners [4]);
            
            this.AddEdge("edge-side-1", this.Corners [0], this.Corners [4]);
            this.AddEdge("edge-side-2", this.Corners [1], this.Corners [5]);
            this.AddEdge("edge-side-3", this.Corners [2], this.Corners [6]);
            this.AddEdge("edge-side-4", this.Corners [3], this.Corners [7]);
            
            // Faces
            this.AddFace("face-vert-1", this.Corners[0], this.Corners[1], this.Corners[5], this.Corners[4]);
            
            this.AddFace("face-vert-2", this.Corners[1], this.Corners[2], this.Corners[6], this.Corners[5]);
            this.AddFace("face-vert-3", this.Corners[2], this.Corners[3], this.Corners[7], this.Corners[6]);
            this.AddFace("face-vert-4", this.Corners[3], this.Corners[0], this.Corners[4], this.Corners[7]);
            
            this.AddFace("face-horiz-1", this.Corners[3], this.Corners[2], this.Corners[1], this.Corners[0]);
            this.AddFace("face-horiz-2", this.Corners[4], this.Corners[5], this.Corners[6], this.Corners[7]);
            
            // right
            this.AddComponent("face-vert-1", new SimpleTransform (new Vector3 (0.5f, 0.5f, 0f), Quaternion.LookRotation (Vector3.up, Vector3.right), new Vector3 (1f, 0f, 1f)), x => x.ToZXY ());
            // left
            this.AddComponent("face-vert-3", new SimpleTransform (new Vector3 (-0.5f, 0.5f, 0f), Quaternion.LookRotation (Vector3.up, Vector3.left), new Vector3 (1f, 0f, 1f)), x => x.ToZXY ());
            // forward
            this.AddComponent("face-vert-4", new SimpleTransform (new Vector3 (0f, 0.5f, 0.5f), Quaternion.LookRotation (Vector3.up, Vector3.forward), new Vector3 (1f, 0f, 1f)), x => x.ToXZY ());
            // backward
            this.AddComponent("face-vert-2", new SimpleTransform (new Vector3 (0f, 0.5f, -0.5f), Quaternion.LookRotation (Vector3.up, Vector3.back), new Vector3 (1f, 0f, 1f)), x => x.ToXZY ());
            // down
            this.AddComponent("face-horiz-1", new SimpleTransform (new Vector3 (0f, 0f, 0f), Quaternion.LookRotation (Vector3.forward, Vector3.down), new Vector3 (1f, 0f, 1f)), x => x);
            // up
            this.AddComponent("face-horiz-2", new SimpleTransform (new Vector3 (0f, 1f, 0f), Quaternion.LookRotation (Vector3.forward, Vector3.up), new Vector3(1f, 0f, 1f)), x => x);
        }
    
        protected override void ApplyStyle (IStyleConfig styleConfig)
        {
            var faceColor = styleConfig.GetColor(this.Style, this.Theme, "face-color");
    
            foreach (var f in this.Faces)
                f.Color = faceColor;
        }
    }
}