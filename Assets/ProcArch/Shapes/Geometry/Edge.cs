namespace Andoco.Unity.ProcArch.Shapes.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    public class Edge
    {
        public Edge (string name, Corner a, Corner b)
        {
            this.Name = name;
            this.CornerA = a;
            this.CornerB = b;
        }
    
        public string Name { get; set; }
        
        public Corner CornerA { get; set; }
        
        public Corner CornerB { get; set; }
    }
}
