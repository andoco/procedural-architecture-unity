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
                var numVerts = face.Corners.Count;

                for (int i=0; i < face.Corners.Count; i++)
                {
                    var v = face.Corners[i].Position;
                    var uv = face.UVs[i];

                    var worldPos = this.Transform.Position + (this.Transform.Rotation * Vector3.Scale (v, this.Transform.Scale));
                    
                    meshBuilder.Vertices.Add(worldPos);
                    meshBuilder.UVs.Add(uv);
                    meshBuilder.Colors.Add(face.Color);
                }
                
                if (numVerts == 3) {
                    meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                } else if (numVerts == 4) {
                    meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 3);
                    meshBuilder.AddTriangle(baseIndex + 1, baseIndex + 2, baseIndex + 3);
                } else {
                    throw new InvalidOperationException(string.Format ("Cannot build mesh for faces with {0} vertices", numVerts));
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

        protected Corner AddCorner(string name, Vector3 position)
        {
            var corner = new Corner(name, position);
            this.Corners.Add(corner);

            return corner;
        }

        protected Edge AddEdge(string name, Corner corner1, Corner corner2)
        {
            var edge = new Edge(name, corner1, corner2);
            this.Edges.Add(edge);

            return edge;
        }

        protected Edge AddEdge(string name, int cornerIndex1, int cornerIndex2)
        {
            return this.AddEdge(name, this.Corners[cornerIndex1], this.Corners[cornerIndex2]);
        }

        protected Face AddFace(string name, params int[] cornerIndices)
        {
            var corners = new Corner[cornerIndices.Length];

            for (int i=0; i < cornerIndices.Length; i++)
            {
                corners[i] = this.Corners[cornerIndices[i]];
            }

            return this.AddFace(name, corners);
        }
    
        protected Face AddFace(string name, params Corner[] corners)
        {
            var face = new Face(name, corners);
            this.Faces.Add(face);

            if (corners.Length == 3)
                face.UVs = new Vector2[] { new Vector2(0f,0f), new Vector2(1f,0f), new Vector2(1f,1f) };
            else if (corners.Length == 4)
                face.UVs = new Vector2[] { new Vector2(0f,0f), new Vector2(1f,0f), new Vector2(1f,1f), new Vector2(0f, 1f) };
            else
                throw new System.InvalidOperationException();

            return face;
        }

        protected ScopeComponent AddComponent(string name, Vector3 pos, Quaternion rot, Vector3 scale, Func<Vector3, Vector3> axisMap)
        {
            return this.AddComponent(name, new SimpleTransform(pos, rot, scale), axisMap);
        }

        protected ScopeComponent AddComponent(string name, SimpleTransform transform, Func<Vector3, Vector3> axisMap)
        {
            var component = new ScopeComponent(name, transform, axisMap);
            this.Components.Add(component);

            return component;
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