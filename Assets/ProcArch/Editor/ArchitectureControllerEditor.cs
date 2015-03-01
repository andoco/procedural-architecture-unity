using UnityEngine;
using System.Collections;
using UnityEditor;
using Andoco.Unity.ProcArch;

[CustomEditor(typeof(ArchitectureController))]
public class TestUI : Editor {

	private Vector2 scrollPos;

	public override void OnInspectorGUI ()
	{
		var arch = this.target as ArchitectureController;

		EditorGUILayout.LabelField("Architecture Name");
		arch.sourceName = EditorGUILayout.TextField(arch.sourceName);

		arch.sourceAsset = (TextAsset)EditorGUILayout.ObjectField("Source Asset", arch.sourceAsset, typeof(TextAsset), allowSceneObjects: false);

		arch.sourceInputField = (UnityEngine.UI.InputField)EditorGUILayout.ObjectField("Source InputField", arch.sourceInputField, typeof(UnityEngine.UI.InputField), allowSceneObjects: true);

		EditorGUILayout.LabelField("Architecture Source");
		this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, GUILayout.MinHeight(200f));
		arch.sourceContent = EditorGUILayout.TextArea(arch.sourceContent);
		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Edit"))
		{
			var win = EditorWindow.GetWindow<ArchitectureControllerEditorWindow>();
			win.architectureController = arch;
		}

		EditorGUILayout.LabelField("Root Args");
		arch.rootArgs = EditorGUILayout.TextField(arch.rootArgs);

		EditorGUILayout.LabelField("Global Args");
		arch.globalArgs = EditorGUILayout.TextField(arch.globalArgs);

		arch.autoBuild = EditorGUILayout.Toggle("Auto Build", arch.autoBuild);

		arch.material = (Material)EditorGUILayout.ObjectField("Material", arch.material, typeof(Material), allowSceneObjects: false);

		arch.showCornerGizmos = EditorGUILayout.Toggle("Corner Gizmos", arch.showCornerGizmos);
		arch.showEdgeGizmos = EditorGUILayout.Toggle("Edge Gizmos", arch.showEdgeGizmos);
		arch.showComponentGizmos = EditorGUILayout.Toggle("Component Gizmos", arch.showComponentGizmos);

		if (GUILayout.Button("Build"))
		{
			arch.Build();
		}

		EditorUtility.SetDirty(arch);
	}
}
