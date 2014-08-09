namespace Andoco.Unity.ProcArch.Shapes.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    public class Corner
    {
        public Corner (string name, Vector3 pos)
        {
            this.Name = name;
            this.Position = pos;
        }
    
        public string Name { get; set; }
        
        public Vector3 Position { get; set; }
    }
}
