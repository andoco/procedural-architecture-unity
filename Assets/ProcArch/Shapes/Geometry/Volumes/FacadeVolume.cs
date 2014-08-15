namespace Andoco.Unity.ProcArch.Shapes.Geometry.Volumes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
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
            this.AddEdge("edge-right", 0, 1);
            this.AddEdge("edge-bottom", 1, 2);
            this.AddEdge("edge-left", 2, 3);
            this.AddEdge("edge-top", 3, 0);
            
            // Faces
            this.AddFace("face", 0, 1, 2, 3);
            
            this.AddComponent(this.Faces[0].Name, Vector3.zero, Quaternion.identity, new Vector3 (1f, 0f, 1f), v => v);
        }
    
        protected override void ApplyStyle (IStyleConfig styleConfig)
        {
            this.Faces[0].Color = styleConfig.GetColor(this.Style, this.Theme, "face-color");
        }   
    }
}