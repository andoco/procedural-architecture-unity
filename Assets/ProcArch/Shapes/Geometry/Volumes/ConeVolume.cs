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
    
    public class ConeVolume : Volume
    {
        public int NumSegments { get; private set; }
    
        protected override void ApplyArguments (Argument[] args)
        {
            this.NumSegments = args.GetArgOrDefault ("segments", 8);
            if (this.NumSegments < 3)
                throw new ArgumentException (string.Format ("ConeVolume 'segments' minimum value = 3. Actual value = {0}", this.NumSegments), "NumSegments");
        }
    
        protected override void OnBuildVolume (Argument[] args)
        {
            var angleDelta = Mathf.PI * 2f / this.NumSegments;
            var segWidth = Mathf.Sin (angleDelta / 2f);
    
            var bottomCorner = this.AddCorner("corner-bottom", new Vector3 (0f, 0f, 0f));
            
            var topCorner = this.AddCorner("corner-top", new Vector3 (0f, 1f, 0f));
    
            for (int i=0; i <= this.NumSegments; i++) {
                var a = angleDelta * -i;
    
                var p1 = new Vector3 (Mathf.Cos (a) * 0.5f, 0f, Mathf.Sin (a) * 0.5f);
                
                this.AddCorner(string.Format ("corner-bottom-{0}", i), p1);
    
                var numCorners = this.Corners.Count;
                
                if (i > 0) {
                    this.AddEdge(string.Format ("edge-bottom-{0}", i), this.Corners[numCorners - 2], this.Corners[numCorners - 1]);
                }
                
                this.AddEdge(string.Format ("edge-side-{0}", i), this.Corners[numCorners - 2], this.Corners[numCorners - 1]);
    
                if (i > 0) {
                    this.AddFace(string.Format ("face-side-{0}", i), this.Corners[numCorners - 2], this.Corners[numCorners - 1], topCorner);
                    this.AddFace(string.Format ("face-bottom-{0}", i), this.Corners[numCorners - 2], bottomCorner, this.Corners[numCorners - 1]);
                }
            }
    
            foreach (var face in this.Faces.Where(f => f.Name.StartsWith("face-side", StringComparison.InvariantCultureIgnoreCase))) {
                var upDir = face.GetCentre ();
                upDir.y = 0;
                
                this.AddComponent(
                    face.Name,
                    new SimpleTransform (face.GetCentre (), Quaternion.LookRotation (Vector3.up, upDir), new Vector3 (segWidth, 0f, 1f)), x => x.ToZXY());
            }
    
            this.AddComponent("face-bottom", new SimpleTransform (new Vector3 (0f, 0f, 0f), Quaternion.LookRotation (Vector3.forward, Vector3.down), new Vector3(1f, 0f, 1f)), x => x);
        }
        
        protected override void ApplyStyle(IStyleConfig styleConfig)
        {
            var faceColor = styleConfig.GetColor(this.Style, this.Theme, "face-color");
    
            foreach (var f in this.Faces)
                f.Color = faceColor;
        }
    }
}