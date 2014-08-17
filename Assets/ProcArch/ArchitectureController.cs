namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Core.Graph.Tree;
    
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

        public bool showCornerGizmos;
        public bool showEdgeGizmos;
        public bool showComponentGizmos;

        public Architecture CurrentArchitecture { get { return this.architecture; } }
        
        void Start()
        {    
            if (string.IsNullOrEmpty(this.sourceName)) {
                throw new ArgumentException("Requires sourceName");
            }

            if (!string.IsNullOrEmpty(this.sourceContent))
            {
                this.Build(this.sourceContent);
            }
            else if (this.sourceAsset != null)
            {
                this.Build(this.sourceAsset.text);
            }
        }
        
        void OnDrawGizmos()
        {
            if (Application.isPlaying && this.architecture != null) {
                this.architecture.Configuration.DrawGizmos(this.transform, this.showCornerGizmos, this.showEdgeGizmos, this.showComponentGizmos);
            }
        }

        private void Build(string source)
        {
            this.architecture = this.architectureBuilder.Build(
                this.sourceName, 
                source, 
                this.rootArgs == null ? new List<string>() : this.rootArgs.Split(',').ToList(), 
                this.globalArgs.ParseArgs());
            
            var meshFilter = this.gameObject.AddComponent<MeshFilter>();
            var meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = this.architecture.Mesh;
            meshRenderer.material = this.material;
        }
    }
}
