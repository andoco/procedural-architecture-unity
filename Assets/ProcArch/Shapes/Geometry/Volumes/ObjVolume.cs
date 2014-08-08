using UnityEngine;
using System.Collections.Generic;
using ObjLoader.Loader.Loaders;
using System.IO;
using System.Text;
using System.Globalization;
using Andoco.Unity.Framework.Core.Meshes;

public class ObjVolume : Volume
{
	private LoadResult result;

	protected override void OnBuildVolume(Argument[] args)
	{
		var objFile = args.GetArgOrDefault<string>("objFile", null);

		var objLoaderFactory = new ObjLoaderFactory();
		var objLoader = objLoaderFactory.Create();
		
		var text = (TextAsset)Resources.Load(objFile);
		var ms = new MemoryStream(Encoding.UTF8.GetBytes(text.text));
		this.result = objLoader.Load(ms);

		var cornerIndex = new Dictionary<Vector3, Corner>();

		for (var i=0; i < this.result.Vertices.Count; i++)
		{
			var v = this.result.Vertices[i];
			var c = new Corner(string.Format("corner-{0}", i), new Vector3(v.X, v.Y, v.Z));
			this.Corners.Add(c);
			cornerIndex[c.Position] = c;
		}

		for (var i=0; i < this.result.Groups.Count; i++)
		{
			var group = this.result.Groups[i];

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

				this.Faces.Add(new Face(string.Format("face-{0}-{1}", j, i), faceCorners));
			}
		}
	}
}
