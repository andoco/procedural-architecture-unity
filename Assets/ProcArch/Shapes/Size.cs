namespace Andoco.Unity.ProcArch.Shapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    /// <summary>
    /// Structure for a size value that can be an absolute or relative value.
    /// </summary>
    public struct Size
    {
        public Size (float value, bool isRelative)
            : this()
        {
            this.Value = value;
            this.IsRelative = isRelative;
        }
    
        public const string RelativeSuffix = "r";
    
        public float Value { get; set; }
    
        public bool IsRelative { get; set; }
    
        public static Size Parse (string value)
        {
            if (value.EndsWith (RelativeSuffix)) {
                return new Size (float.Parse (value.TrimEnd (RelativeSuffix.ToArray ())), true);
            } else {
                return new Size (float.Parse (value), false);
            }
        }
    
        public static float SumRelative (IEnumerable<Size> sizes)
        {
            var sum = 0f;
    
            foreach (var s in sizes) {
                if (s.IsRelative) {
                    sum += s.Value;
                }
            }
    
            return sum;
        }
    
        public static float SumAbsolute (IEnumerable<Size> sizes)
        {
            var sum = 0f;
    
            foreach (var s in sizes) {
                if (!s.IsRelative) {
                    sum += s.Value;
                }
            }
    
            return sum;
        }

		/// <summary>
		/// Gets the absolute value of the size by scaling by <paramref name="scale"/> if the size if relative.
		/// </summary>
		public float GetAbsolute(float scale)
		{
			return this.IsRelative ? scale * this.Value : this.Value;
		}

		/// <summary>
		/// Gets a <see cref="Vector3"/> for a set of 3 sizes.
		/// </summary>
		public static Vector3 ToVector3(Size x, Size y, Size z, Vector3 scale)
		{
			return new Vector3(x.GetAbsolute(scale.x), y.GetAbsolute(scale.y), z.GetAbsolute(scale.z));
		}
    
        public override string ToString ()
        {
            return string.Format ("[Size: Value={0}, IsRelative={1}]", Value, IsRelative);
        }
    }
}
