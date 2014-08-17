namespace Andoco.Unity.ProcArch
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Core.Graph.Tree;
    using Andoco.Unity.Framework.Core.Meshes;
    using Andoco.Unity.ProcArch.Irony;
    using Andoco.Unity.ProcArch.Shapes;
    using Andoco.Unity.ProcArch.Shapes.Configuration;
    using Andoco.Unity.ProcArch.Shapes.Styles;
    
    public class Architecture
    {
        public Mesh Mesh { get; set; }

        public IDictionary<string, NodeMeshData> MeshData { get; set; }
    
        public IShapeConfiguration Configuration { get; set; }
    }

    public class NodeMeshData
    {
        public int ColorsStart { get; set; }
        
        public int ColorsEnd { get; set; }
    }
    
    public class ArchitectureBuilder
    {
        private IDictionary<string, IShapeProductionSystem> productionSystemCache = new Dictionary<string, IShapeProductionSystem>();
    
        public Architecture Build(string name, string source, IList<string> rootArgs, IDictionary<string, string> globalArgs, string theme = null)
        {
            var system = GetProductionSystem(name, source);
    
            var shapeConfiguration = system.Run(rootArgs, globalArgs);
    
            var styleConfig = new CommonArchitectureStyleConfig();
            if (theme != null)
                styleConfig.DefaultTheme = theme;

            Dictionary<string, NodeMeshData> nodeMeshData;
            Mesh mesh;

            BuildMesh(shapeConfiguration, styleConfig, out mesh, out nodeMeshData);
    
            return new Architecture
            {
                Mesh = mesh,
                MeshData = nodeMeshData,
                Configuration = shapeConfiguration
            };
        }
    
        private IShapeProductionSystem GetProductionSystem(string name, string source)
        {
            IShapeProductionSystem system;
    
            if (!this.productionSystemCache.TryGetValue(name, out system)) {
                var builder = new IronyShapeProductionSystemBuilder();
                system = builder.Build(source);
                system.Axiom = "root";
                this.productionSystemCache.Add(name, system);
            }
    
            return system;
        }
    
        private void BuildMesh(IShapeConfiguration configuration, IStyleConfig styleConfig, out Mesh mesh, out Dictionary<string, NodeMeshData> data)
        {
            var meshBuilder = new MeshBuilder();

            var tmpNodeMeshData = new Dictionary<string, NodeMeshData>();

            configuration.RootNode.TraverseBreadthFirst(node => {
                var shapeNode = (ShapeNode)node;
                var vol = shapeNode.Value.Volume;
                
                if (node.IsLeaf && vol != null) {
                    var colStart = meshBuilder.Colors.Count;

                    vol.BuildMesh(meshBuilder, styleConfig);

                    var colAdded = meshBuilder.Colors.Count - colStart;

                    var md = new NodeMeshData();
                    md.ColorsStart = colStart;
                    md.ColorsEnd = md.ColorsStart + colAdded;

                    tmpNodeMeshData.Add(shapeNode.Id + "-" + shapeNode.Value.Rule.Symbol, md);
                }
            });
            
            mesh = meshBuilder.BuildMesh();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            data = tmpNodeMeshData;
        }
    }
}