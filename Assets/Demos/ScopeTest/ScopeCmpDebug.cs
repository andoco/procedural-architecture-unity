using UnityEngine;
using System.Collections;

public class ScopeCmpDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos()
	{
		var tx = this.transform;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(tx.position, tx.position + tx.rotation * Vector3.Scale(Vector3.right, tx.localScale));

		Gizmos.color = Color.green;
		Gizmos.DrawLine(tx.position, tx.position + tx.rotation * Vector3.Scale(Vector3.up, tx.localScale));

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(tx.position, tx.position + tx.rotation * Vector3.Scale(Vector3.forward, tx.localScale));
	}
}
