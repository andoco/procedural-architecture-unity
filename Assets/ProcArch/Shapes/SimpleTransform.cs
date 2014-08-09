namespace Andoco.Unity.ProcArch.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    public class SimpleTransform
    {
        public SimpleTransform ()
        {
            this.Position = Vector3.zero;
            this.Rotation = Quaternion.identity;
            this.Scale = Vector3.one;
        }
    
        public SimpleTransform (Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }
    
        public SimpleTransform (SimpleTransform other)
        {
            this.Position = other.Position;
            this.Rotation = other.Rotation;
            this.Scale = other.Scale;
        }
    
        public Vector3 Position { get; set; }
    
        public Quaternion Rotation { get; set; }
    
        public Vector3 Scale { get; set; }
    
        public static SimpleTransform operator + (SimpleTransform t1, SimpleTransform t2)
        {
            return new SimpleTransform (t1.Position + t2.Position, t1.Rotation * t2.Rotation, Vector3.Scale (t1.Scale, t2.Scale));
        }
    
        public override string ToString ()
        {
            return string.Format ("[SimpleTransform: Position={0}, Rotation={1}, Scale={2}]", Position, Rotation, Scale);
        }
    }
}
