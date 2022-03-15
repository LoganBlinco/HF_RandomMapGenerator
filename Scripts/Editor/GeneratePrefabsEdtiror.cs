using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePrefabs))]
public class GeneratePrefabsEdtiror : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GeneratePrefabs myScript = (GeneratePrefabs)target;
		if (GUILayout.Button("Generate all assets"))
		{
			myScript.Generate();
		}
	}
}
