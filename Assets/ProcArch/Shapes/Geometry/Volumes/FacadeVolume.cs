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

    public class FacadeVolume : Volume
    {
        public FacadeVolume ()
            : base()
        {
            this.Style = "facade";
        }
        				    
        protected override void OnBuildVolume (Argument[] args)
        {
            // Corners
            this.AddCorner("corner-top-right", new Vector3 (0.5f, 0f, 0.5f));
            this.AddCorner("corner-bottom-right", new Vector3 (0.5f, 0f, -0.5f));
            this.AddCorner("corner-bottom-left", new Vector3 (-0.5f, 0f, -0.5f));
            this.AddCorner("corner-top-left", new Vector3 (-0.5f, 0f, 0.5f));
            
            // Edges
            var edgeRight = this.AddEdge("edge-right", 0, 1);
            var edgeBottom = this.AddEdge("edge-bottom", 1, 2);
            var edgeLeft = this.AddEdge("edge-left", 2, 3);
            var edgeTop = this.AddEdge("edge-top", 3, 0);
            
            // Faces
            this.AddFace("face", 0, 1, 2, 3);
            
            this.AddComponent(this.Faces[0].Name, Vector3.zero, Quaternion.identity, new Vector3 (1f, 0f, 1f), v => v);

            // Edge components
            this.AddComponent(edgeRight.Name, Vector3.Lerp(edgeRight.CornerA.Position, edgeRight.CornerB.Position, 0.5f), Quaternion.identity, new Vector3(0f, 0f, 1f), v => v.ToZXY());
            this.AddComponent(edgeBottom.Name, Vector3.Lerp(edgeBottom.CornerA.Position, edgeBottom.CornerB.Position, 0.5f), Quaternion.identity, new Vector3(1f, 0f, 0f), v => v);
            this.AddComponent(edgeLeft.Name, Vector3.Lerp(edgeLeft.CornerA.Position, edgeLeft.CornerB.Position, 0.5f), Quaternion.identity, new Vector3(0f, 0f, 1f), v => v.ToZXY());
            this.AddComponent(edgeTop.Name, Vector3.Lerp(edgeTop.CornerA.Position, edgeTop.CornerB.Position, 0.5f), Quaternion.identity, new Vector3(1f, 0f, 0f), v => v);
        }
    
        protected override void ApplyStyle (IStyleConfig styleConfig)
        {
            this.Faces[0].Color = styleConfig.GetColor(this.Style, this.Theme, "face-color");
        }   
    }
}