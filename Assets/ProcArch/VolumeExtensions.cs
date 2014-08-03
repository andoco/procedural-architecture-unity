using UnityEngine;

public static class VolumeExtensions
{
	public static void DrawGizmos(this Volume vol, Transform parent)
	{
		var lineScale = vol.Transform.Scale/2f;

		var p1 = parent.position + (parent.rotation * vol.Transform.Position);
		var rot = parent.rotation * vol.Transform.Rotation;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(p1, p1 + rot * Vector3.Scale(Vector3.right, lineScale));

		Gizmos.color = Color.green;
		Gizmos.DrawLine(p1, p1 + rot * Vector3.Scale(Vector3.up, lineScale));

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(p1, p1 + rot * Vector3.Scale(Vector3.forward, lineScale));
	}
}
