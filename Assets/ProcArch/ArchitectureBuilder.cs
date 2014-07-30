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
	private IDictionary<string, TextAsset> assetCache = new Dictionary<string, TextAsset>();
	private IDictionary<string, IShapeProductionSystem> productionSystemCache = new Dictionary<string, IShapeProductionSystem>();

	public Architecture Build(string source)
	{
		var system = GetProductionSystem(source);

		var shapeConfiguration = new ShapeConfiguration(system.Rules);
		system.Run(shapeConfiguration, new List<string> { "2", "3", "4" });

		var styleConfig = new CommonArchitectureStyleConfig();
		var mesh = BuildMesh(shapeConfiguration, styleConfig);

		return new Architecture
		{
			Mesh = mesh,
			Configuration = shapeConfiguration
		};
	}

	private string GetSourceContent(string source)
	{
		TextAsset asset;

		if (!this.assetCache.TryGetValue(source, out asset))
		{
			asset = Resources.Load<TextAsset>(source);
			this.assetCache.Add(source, asset);
		}
		
		return asset.text;
	}

	private IShapeProductionSystem GetProductionSystem(string source)
	{
		IShapeProductionSystem system;

		if (!this.productionSystemCache.TryGetValue(source, out system))
		{
			var sourceContent = GetSourceContent(source);

			var builder = new IronyShapeProductionSystemBuilder();
			system = builder.Build(sourceContent);
			system.Axiom = "root";
			this.productionSystemCache.Add(source, system);
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