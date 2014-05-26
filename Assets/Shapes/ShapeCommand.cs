using System.Linq;
using UnityEngine;

public interface IShapeCommand
{
	string Name { get; set; }

	string[] Arguments { get; set; }

	void Execute(TreeNode<ShapeNodeValue> currentNode);
}

public class ShapeCommand : IShapeCommand
{
	public string Name { get; set; }
	
	public string[] Arguments { get; set; }

	public void Execute(TreeNode<ShapeNodeValue> currentNode)
	{
		switch (Name)
		{
		case "Set":
			var shapeName = Arguments[0].Trim('"');
//			this.drawCtx.AddShape(shapeName);
			Debug.Log(string.Format("SHAPE: {0}", shapeName));
			var newNode = new TreeNode<ShapeNodeValue>("-1", currentNode)
			{
				Value = new ShapeNodeValue
				{
					Matrix = currentNode.Value.Matrix,
					ShapeName = shapeName
				}
			};
			currentNode.Add(newNode);
			break;
		case "Trans":
			var axes = Arguments.Select(x => float.Parse(x)).ToArray();
			var transDelta = new Vector3(axes[0], axes[1], axes[2]);
			currentNode.Value.Matrix = currentNode.Value.Matrix * Matrix4x4.TRS(transDelta, Quaternion.identity, Vector3.one);
			break;
		case "Rot":
			var rotAxes = Arguments.Select(x => float.Parse(x)).ToArray();
			//this.drawCtx.AddScope(Vector3.zero, Quaternion.Euler(rotAxes[0], rotAxes[1], rotAxes[2]), Vector3.one);
			var rot = Quaternion.Euler(rotAxes[0], rotAxes[1], rotAxes[2]);
			currentNode.Value.Matrix = currentNode.Value.Matrix * Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
			break;
		case "Scale":
			var scaleAxes = Arguments.Select(x => float.Parse(x)).ToArray();
			//this.drawCtx.AddScope(Vector3.zero, Quaternion.identity, new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]));
			var scaleDelta = new Vector3(scaleAxes[0], scaleAxes[1], scaleAxes[2]);
			currentNode.Value.Matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scaleDelta);
			break;
		}
	}
}