using UnityEngine;

public static class VolumeExtensions
{
	public static void DrawGizmos(this Volume vol)
	{
//		var lineScale = 0.25f;
		
//		Gizmos.color = Color.red;
//		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.right * lineScale);
//		Gizmos.color = Color.green;
//		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.up * lineScale);
//		Gizmos.color = Color.blue;
//		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.forward * lineScale);

		var lineScale = vol.Transform.Scale/2f;



		Gizmos.color = Color.red;
//		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.Scale(Vector3.right, lineScale));
		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + Vector3.Scale((vol.Transform.Rotation * Vector3.right), lineScale));


		Gizmos.color = Color.green;
		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.Scale(Vector3.up, lineScale));
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(vol.Transform.Position, vol.Transform.Position + vol.Transform.Rotation * Vector3.Scale(Vector3.forward, lineScale));
	}
}
