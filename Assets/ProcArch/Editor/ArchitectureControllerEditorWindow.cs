using UnityEngine;
using UnityEditor;
using Andoco.Unity.ProcArch;

public class ArchitectureControllerEditorWindow : EditorWindow
{
	private Vector2 scrollPos;

	public ArchitectureController architectureController;

	void OnGUI()
	{
		EditorGUILayout.LabelField("Architecture Source");

		if (this.architectureController == null)
		{
			EditorGUILayout.LabelField("No ArchitectureController set");
		}
		else
		{
			this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
			this.architectureController.sourceContent = EditorGUILayout.TextArea(this.architectureController.sourceContent);
			EditorGUILayout.EndScrollView();
		}
	}
}
