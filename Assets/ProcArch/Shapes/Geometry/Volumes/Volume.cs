namespace Andoco.Unity.ProcArch.Shapes.Geometry.Volumes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Andoco.Unity.Framework.Core.Meshes;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    using Andoco.Unity.ProcArch.Shapes.Styles;

    public abstract class Volume
    {
        public Volume ()
        {
            this.Corners = new List<Corner> ();
            this.Edges = new List<Edge> ();
            this.Faces = new List<Face> ();
            this.Components = new List<ScopeComponent> ();
            this.Transform = new SimpleTransform ();
        }
    
        public IList<Corner> Corners { get; private set; }
        
        public IList<Edge> Edges { get; private set; }
        
        public IList<Face> Faces { get; private set; }
    
        public IList<ScopeComponent> Components { get; private set; }
    
        public SimpleTransform Transform { get; private set; }
    
        public string Style { get; set; }
    
        public string Theme { get; set; }
    
        public IEnumerable<Corner> GetCorners (string query)
        {
            return this.Corners.Where (c => c.Name.StartsWith (query));
        }
        
        public IEnumerable<Edge> GetEdges (string query)
        {
            return this.Edges.Where (e => e.Name.StartsWith (query));
        }
        
        public IEnumerable<Face> GetFaces (string query)
        {
            return this.Faces.Where (f => f.Name.StartsWith (query));
        }
    
        public IEnumerable<ScopeComponent> Query (string query)
        {
            var matching = this.Components.Where (c => Regex.IsMatch (c.Name, query));
    
            return matching;
        }
        
        public void BuildVolume (Argument[] args)
        {
            var style = args.SingleOrDefault (x => x.Name == "style");
            if (style != null)
                this.Style = style.Value;
    
            this.ApplyArguments (args);
    
            this.OnBuildVolume (args);
        }
        
        public virtual void ApplyTransform (SimpleTransform transform)
        {
            this.Transform.Position = transform.Position;
            this.Transform.Rotation = transform.Rotation;
            this.Transform.Scale = transform.Scale;
        }
    
        public virtual void BuildMesh (IMeshBuilder meshBuilder, IStyleConfig styleConfig)
        {
            this.ApplyStyle (styleConfig);
    
            var baseIndex = meshBuilder.Vertices.Count;
            
            foreach (var face in this.Faces) {
                var verts = face.Corners.Select (c => c.Position).ToArray ();
                
                foreach (var v in verts) {
                    var worldPos = this.Transform.Position + (this.Transform.Rotation * Vector3.Scale (v, this.Transform.Scale));
                    
                    meshBuilder.Vertices.Add (worldPos);
                    meshBuilder.UVs.Add (Vector2.zero);
                    meshBuilder.Colors.Add (face.Color);
                }
                
                if (verts.Length == 3) {
                    meshBuilder.AddTriangle (baseIndex, baseIndex + 1, baseIndex + 2);
                } else if (verts.Length == 4) {
                    meshBuilder.AddTriangle (baseIndex, baseIndex + 1, baseIndex + 3);
                    meshBuilder.AddTriangle (baseIndex + 1, baseIndex + 2, baseIndex + 3);
                } else {
                    throw new InvalidOperationException (string.Format ("Cannot build mesh for faces with {0} vertices", verts.Length));
                }
                
                baseIndex = meshBuilder.Vertices.Count;
            }
        }
    
        public Mesh BuildMesh (IStyleConfig styleConfig)
        {
            var meshBuilder = new MeshBuilder();
            this.BuildMesh(meshBuilder, styleConfig);
    
            var mesh = meshBuilder.BuildMesh();
    
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();
    
            return mesh;
        }
    
        #region Protected methods

        protected void AddCorner(string name, Vector3 position)
        {
            this.Corners.Add(new Corner(name, position));
        }

        protected void AddEdge(string name, Corner corner1, Corner corner2)
        {
            this.Edges.Add(new Edge(name, corner1, corner2));
        }

        protected void AddEdge(string name, int cornerIndex1, int cornerIndex2)
        {
            this.Edges.Add(new Edge(name, this.Corners[cornerIndex1], this.Corners[cornerIndex2]));
        }

        protected void AddFace(string name, params int[] cornerIndices)
        {
            var corners = new Corner[cornerIndices.Length];

            for (int i=0; i < cornerIndices.Length; i++)
            {
                corners[i] = this.Corners[cornerIndices[i]];
            }

            this.Faces.Add(new Face(name, corners));
        }
    
        protected void AddFace(string name, params Corner[] corners)
        {
            this.Faces.Add(new Face(name, corners));
        }

        protected void AddComponent(string name, Vector3 pos, Quaternion rot, Vector3 scale, Func<Vector3, Vector3> axisMap)
        {
            this.Components.Add(new ScopeComponent(name, new SimpleTransform(pos, rot, scale), axisMap));
        }

        protected void AddComponent(string name, SimpleTransform transform, Func<Vector3, Vector3> axisMap)
        {
            this.Components.Add(new ScopeComponent(name, transform, axisMap));
        }

        protected virtual void ApplyArguments(Argument[] args)
        {
        }
        
        protected abstract void OnBuildVolume(Argument[] args);
        
        protected virtual void ApplyStyle(IStyleConfig styleConfig)
        {
        }
    
        #endregion
    }
}