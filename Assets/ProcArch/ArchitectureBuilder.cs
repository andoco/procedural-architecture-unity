using UnityEngine;
using System.Collections.Generic;
using Andoco.Core.Graph.Tree;
using Andoco.Unity.Framework.Core.Meshes;

public class Architecture
{
	public Mesh Mesh { get; set; }

	public IShapeConfiguration Configuration { get; set; }
}

public class ArchitectureBuilder
{
	private IDictionary<string, IShapeProductionSystem> productionSystemCache = new Dictionary<string, IShapeProductionSystem>();

	public Architecture Build(string name, string source, IList<string> rootArgs, IDictionary<string, string> globalArgs)
	{
		var system = GetProductionSystem(name, source);

		var shapeConfiguration = new ShapeConfiguration(system.Rules);
		shapeConfiguration.AddGlobalArgs(globalArgs);
		system.Run(shapeConfiguration, rootArgs);

		var styleConfig = new CommonArchitectureStyleConfig();
		var mesh = BuildMesh(shapeConfiguration, styleConfig);

		return new Architecture
		{
			Mesh = mesh,
			Configuration = shapeConfiguration
		};
	}

	private IShapeProductionSystem GetProductionSystem(string name, string source)
	{
		IShapeProductionSystem system;

		if (!this.productionSystemCache.TryGetValue(name, out system))
		{
			var builder = new IronyShapeProductionSystemBuilder();
			system = builder.Build(source);
			system.Axiom = "root";
			this.productionSystemCache.Add(name, system);
		}

		return system;
	}

	private Mesh BuildMesh(IShapeConfiguration configuration, IStyleConfig styleConfig)
	{
		var meshBuilder = new MeshBuilder();
		
		configuration.RootNode.TraverseBreadthFirst(node => {
			var shapeNode = (ShapeNode)node;
			var vol = shapeNode.Value.Volume;
			
			if (node.IsLeaf && vol != null)
			{
				vol.ApplyStyle(styleConfig);
				vol.BuildMesh(meshBuilder);
			}
		});
		
		var mesh = meshBuilder.BuildMesh();
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();
		
		return mesh;
	}
}