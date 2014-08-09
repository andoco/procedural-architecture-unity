using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ObjLoader.Loader.Loaders;
using UnityEngine;
using System;

public class ObjVolume : Volume
{
	private static readonly Dictionary<string, ObjVolumeCacheItem> objVolCache = new Dictionary<string, ObjVolumeCacheItem>();

	private HashSet<string> groupNames = new HashSet<string>();

	protected override void OnBuildVolume(Argument[] args)
	{
		var objFile = args.GetArgOrDefault<string>("objFile", null);

		ObjVolumeCacheItem cacheItem;

		if (objVolCache.TryGetValue(objFile, out cacheItem))
		{
			RestoreFromCacheItem(cacheItem);
		}
		else
		{
			var result = LoadObj(objFile);
			BuildFromObj(result);
			StoreInCache(objFile);
		}
	}

	protected override void ApplyStyle (IStyleConfig styleConfig)
	{
		foreach (var groupName in this.groupNames)
		{
			var key = string.Format("{0}-color", groupName);
			var color = styleConfig.GetColorOrDefault(this.Style, key, Color.grey);

			foreach (var f in this.Faces)
			{
				if (f.Name.StartsWith(string.Format("face-{0}", groupName), StringComparison.InvariantCultureIgnoreCase))
				{
					f.Color = color;
				}
			}
		}
	}

	private LoadResult LoadObj(string objFile)
	{
		var objLoaderFactory = new ObjLoaderFactory();
		var objLoader = objLoaderFactory.Create();
		
		var text = (TextAsset)Resources.Load(objFile);
		var ms = new MemoryStream(Encoding.UTF8.GetBytes(text.text));
		var result = objLoader.Load(ms);

		return result;
	}

	private void BuildFromObj(LoadResult result)
	{
		var cornerIndex = new Dictionary<Vector3, Corner>();
		
		for (var i=0; i < result.Vertices.Count; i++)
		{
			var v = result.Vertices[i];
			var c = new Corner(string.Format("corner-{0}", i), new Vector3(v.X, v.Y, v.Z));
			this.Corners.Add(c);
			cornerIndex[c.Position] = c;
		}
		
		for (var i=0; i < result.Groups.Count; i++)
		{
			var group = result.Groups[i];

			this.groupNames.Add(group.Name);
			
			for (var j=0; j < group.Faces.Count; j++)
			{
				var face = group.Faces[j];
				var faceCorners = new Corner[face.Count];
				
				for (var k=0; k < face.Count; k++)
				{
					var fv = face[k];
					var v = result.Vertices[fv.VertexIndex - 1];
					var fc = cornerIndex[new Vector3(v.X, v.Y, v.Z)];
					faceCorners[k] = fc;
				}

				this.Faces.Add(new Face(string.Format("face-{0}-{1}-{2}", group.Name, j, i), faceCorners));
			}
		}
	}

	private void StoreInCache(string objFile)
	{
		var cacheItem = new ObjVolumeCacheItem
		{
			Corners = this.Corners,
			Edges = this.Edges,
			Faces = this.Faces,
			Components = this.Components,
			GroupNames = this.groupNames
		};

		objVolCache[objFile] = cacheItem;
	}

	private void RestoreFromCacheItem(ObjVolumeCacheItem cacheItem)
	{
		foreach (var c in cacheItem.Corners)
			this.Corners.Add(c);
		foreach (var e in cacheItem.Edges)
			this.Edges.Add(e);
		foreach (var f in cacheItem.Faces)
			this.Faces.Add(f);
		foreach (var c in cacheItem.Components)
			this.Components.Add(c);
		foreach (var g in cacheItem.GroupNames)
			this.groupNames.Add(g);
	}

	private class ObjVolumeCacheItem
	{
		public IList<Corner> Corners { get; set; }
		public IList<Edge> Edges { get; set; }
		public IList<Face> Faces { get; set; }
		public IList<ScopeComponent> Components { get; set; }
		public HashSet<string> GroupNames { get; set; }
	}
}
