using UnityEngine;
using System.Collections;
using UnityEditor;
using Andoco.Unity.ProcArch;

[CustomEditor(typeof(ArchitectureController))]
public class TestUI : Editor {
	
	public override void OnInspectorGUI ()
	{
		var arch = this.target as ArchitectureController;

		EditorGUILayout.LabelField("Architecture Name");
		arch.sourceName = EditorGUILayout.TextField(arch.sourceName);

		arch.sourceAsset = (TextAsset)EditorGUILayout.ObjectField("Source Asset", arch.sourceAsset, typeof(TextAsset), allowSceneObjects: false);

		EditorGUILayout.LabelField("Architecture Source");
		arch.sourceContent = EditorGUILayout.TextArea(arch.sourceContent);

		EditorGUILayout.LabelField("Root Args");
		arch.rootArgs = EditorGUILayout.TextField(arch.rootArgs);

		EditorGUILayout.LabelField("Global Args");
		arch.globalArgs = EditorGUILayout.TextField(arch.globalArgs);

		arch.material = (Material)EditorGUILayout.ObjectField("Material", arch.material, typeof(Material), allowSceneObjects: false);

		arch.showCornerGizmos = EditorGUILayout.Toggle("Corner Gizmos", arch.showCornerGizmos);
		arch.showEdgeGizmos = EditorGUILayout.Toggle("Edge Gizmos", arch.showEdgeGizmos);
		arch.showComponentGizmos = EditorGUILayout.Toggle("Component Gizmos", arch.showComponentGizmos);

		EditorUtility.SetDirty(arch);
	}
}
