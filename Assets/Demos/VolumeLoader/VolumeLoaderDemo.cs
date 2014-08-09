using UnityEngine;
using System.Collections.Generic;
using ObjLoader.Loader.Loaders;
using System.IO;
using System.Text;
using System.Globalization;
using Andoco.Unity.Framework.Core.Meshes;
using Andoco.Unity.ProcArch.Shapes.Geometry.Volumes;
using Andoco.Unity.ProcArch.Shapes;
using Andoco.Unity.ProcArch;

public class VolumeLoaderDemo : MonoBehaviour {

	public Material material;
	
	void Start()
	{
		var vol = new ObjVolume();
		vol.BuildVolume(new Argument[] { new Argument("objFile", "Door") });
		var meshBuilder = new MeshBuilder();
		vol.BuildMesh(meshBuilder, new CommonArchitectureStyleConfig());
		meshBuilder.CreateGameObject(this.material);
	}
}
