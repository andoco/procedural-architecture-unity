namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using Andoco.Unity.ProcArch.Shapes.Geometry.Volumes;
    
    public static class VolumeExtensions
    {
        public static void DrawGizmos (this Volume vol, Transform parent)
        {
            var lineScale = vol.Transform.Scale / 2f;
    
            var p1 = parent.position + (parent.rotation * vol.Transform.Position);
            var rot = parent.rotation * vol.Transform.Rotation;
    
            Gizmos.color = Color.red;
            Gizmos.DrawLine (p1, p1 + rot * Vector3.Scale (Vector3.right, lineScale));
    
            Gizmos.color = Color.green;
            Gizmos.DrawLine (p1, p1 + rot * Vector3.Scale (Vector3.up, lineScale));
    
            Gizmos.color = Color.blue;
            Gizmos.DrawLine (p1, p1 + rot * Vector3.Scale (Vector3.forward, lineScale));
        }
    
        public static void DrawCornerGizmos (this Volume vol, Transform parent)
        {
            foreach (var c in vol.Corners) {
                var world = vol.LocalToWorldPos (c.Position, parent);
    
                Gizmos.DrawSphere(world, 0.025f);
            }
        }
    
        public static void DrawEdgeGizmos (this Volume vol, Transform parent)
        {
            foreach (var e in vol.Edges) {
                var p1 = vol.LocalToWorldPos (e.CornerA.Position, parent);
                var p2 = vol.LocalToWorldPos (e.CornerB.Position, parent);
    
                Gizmos.DrawLine (p1, p2);
            }
        }
    
        public static void DrawComponentGizmos (this Volume vol, Transform parent)
        {
            Gizmos.color = Color.red;
            foreach (var c in vol.Components) {
                var world = vol.LocalToWorldPos (c.Transform.Position, parent);

                Gizmos.DrawWireSphere(world, 0.05f);
            }
        }
    
        /// <summary>
        /// Transforms a point in the volume's local space to a point in the space of a parent <see cref="Transform"/>.
        /// </summary>
        /// <returns>The to world position.</returns>
        /// <param name="vol">Vol.</param>
        /// <param name="p">Vector to find the world position of.</param>
        /// <param name="parent">Parent transform of the volume.</param>
        public static Vector3 LocalToWorldPos (this Volume vol, Vector3 p, Transform parent)
        {
            var local = vol.Transform.Position + (vol.Transform.Rotation * Vector3.Scale(p, vol.Transform.Scale));
            var world = parent.transform.position + parent.transform.rotation * local;
    
            return world;
        }
    }
}
