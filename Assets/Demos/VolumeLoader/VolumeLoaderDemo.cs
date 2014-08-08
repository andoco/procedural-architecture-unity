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

	public ObjVolume()
		: base()
	{
	}

	#region implemented abstract members of Volume

	protected override void OnBuildVolume(Argument[] args)
	{
		var objFile = args[0].Value;

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
	
	#endregion
}

public class VolumeLoaderDemo : MonoBehaviour {

	public Material material;
	
	void Start()
	{
//		var objLoaderFactory = new ObjLoaderFactory();
//		var objLoader = objLoaderFactory.Create();
//
//		var text = (TextAsset)Resources.Load("Door");
//		var ms = new MemoryStream(Encoding.UTF8.GetBytes(text.text));
//		var result = objLoader.Load(ms);
//		PrintResult(result);

		var vol = new ObjVolume();
		vol.BuildVolume(new Argument[] { new Argument("objFile", "Door") });
		var meshBuilder = new MeshBuilder();
		vol.BuildMesh(meshBuilder, new CommonArchitectureStyleConfig());
		meshBuilder.CreateGameObject(this.material);
	}

//	private static void PrintResult(LoadResult result)
//	{
//		var sb = new StringBuilder();
//		
//		sb.AppendLine("Load result:");
//		sb.Append("Vertices: ");
//		sb.AppendLine(result.Vertices.Count.ToString(CultureInfo.InvariantCulture));
//		sb.Append("Textures: ");
//		sb.AppendLine(result.Textures.Count.ToString(CultureInfo.InvariantCulture));
//		sb.Append("Normals: ");
//		sb.AppendLine(result.Normals.Count.ToString(CultureInfo.InvariantCulture));
//		sb.AppendLine();
//		sb.AppendLine("Groups: ");
//		
//		foreach (var loaderGroup in result.Groups)
//		{
//			sb.AppendLine(loaderGroup.Name);
//			sb.Append("Faces: ");
//			sb.AppendLine(loaderGroup.Faces.Count.ToString(CultureInfo.InvariantCulture));
//		}
//		
//		Debug.Log(sb);
//	}
}
