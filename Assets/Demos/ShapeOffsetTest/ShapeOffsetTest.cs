using UnityEngine;
using System.Collections;
using Andoco.Unity.Framework.Core;

public class ShapeOffsetTest : MonoBehaviour {

	public Transform cube;

	// Use this for initialization
	void Start () {
		var faceOffset = Vector3.right * 0.5f;
		var faceNormal = Vector3.right;

		var faceMatrix = Matrix4x4.TRS(cube.localPosition + faceOffset, cube.localRotation * Quaternion.Euler(faceNormal), Vector3.one);

		var cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube2.transform.localScale = Vector3.one * 0.5f;
		cube2.GetComponent<Renderer>().material.color = Color.red;

		cube2.transform.position = faceMatrix.GetPosition();
		cube2.transform.rotation = faceMatrix.GetRotation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
