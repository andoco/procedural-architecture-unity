using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ArchitectureController))]
public class TestUI : Editor {
	
	public override void OnInspectorGUI ()
	{
		var arch = this.target as ArchitectureController;

		EditorGUILayout.LabelField("Architecture Name");
		arch.sourceName = EditorGUILayout.TextField(arch.sourceName);

		EditorGUILayout.LabelField("Architecture Source");
		arch.sourceContent = EditorGUILayout.TextArea(arch.sourceContent);

		EditorUtility.SetDirty(arch);
	}
}
