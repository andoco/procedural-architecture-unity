namespace Andoco.Unity.ProcArch.Shapes.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    public class Face
    {
        public Face (string name, IEnumerable<Corner> corners)
        {
            this.Name = name;
            this.Corners = corners.ToList ();
            this.Color = Color.grey;
        }
    
        public string Name { get; set; }
        
        public IList<Corner> Corners { get; private set; }
    
        public Color Color { get; set; }

        public Vector2[] UVs { get; set; }
    
        public Vector3 GetCentre ()
        {
            //      var sum = this.Corners.Sum(x => x.Position);
            var x = this.Corners.Aggregate (Vector3.zero, (accum, c) => accum += c.Position);
    
            return x / this.Corners.Count;
        }
    }
}
