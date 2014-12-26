namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Core.Graph.Tree;

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ArchitectureController : MonoBehaviour
    {
        private ArchitectureBuilder architectureBuilder = new ArchitectureBuilder();
        private Architecture architecture;

        public string sourceName;
        public string sourceContent;
        public TextAsset sourceAsset;
        public Material material;
        public string rootArgs;
        public string globalArgs;
        public bool autoBuild;

        public bool showCornerGizmos;
        public bool showEdgeGizmos;
        public bool showComponentGizmos;

        public Architecture CurrentArchitecture { get { return this.architecture; } }
        
        void Start()
        {
            if (string.IsNullOrEmpty(this.sourceName)) {
                throw new ArgumentException("Requires sourceName");
            }

            if (this.autoBuild)
            {
                this.Build();
            }
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying && this.architecture != null) {
                this.architecture.Configuration.DrawGizmos(this.transform, this.showCornerGizmos, this.showEdgeGizmos, this.showComponentGizmos);
            }
        }

        public void Build()
        {
            var src = this.GetSource();

            if (!string.IsNullOrEmpty(src))
            {
                this.Build(src);
            }
        }

        #region Private methods

        private string GetSource()
        {
            if (!string.IsNullOrEmpty(this.sourceContent))
            {
                return this.sourceContent;
            }
            else if (this.sourceAsset != null)
            {
                return this.sourceAsset.text;
            }

            return null;
        }

        private void Build(string source)
        {
            this.architecture = this.architectureBuilder.Build(
                this.sourceName, 
                source, 
                this.rootArgs == null ? new List<string>() : this.rootArgs.Split(',').ToList(), 
                this.globalArgs.ParseArgs());
            
            var meshFilter = this.GetComponent<MeshFilter>();
            var meshRenderer = this.GetComponent<MeshRenderer>();
            meshFilter.sharedMesh = this.architecture.Mesh;
            meshRenderer.material = this.material;
        }

        #endregion
    }
}
