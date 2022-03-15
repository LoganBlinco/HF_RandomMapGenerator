using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		Generator myScript = (Generator)target;
		if (GUILayout.Button("Generate Random Seed"))
		{
			myScript.Run_RandomSeed();
		}
		if (GUILayout.Button("Generate Same Seed"))
		{
			myScript.Run_SetSeed();
		}
	}
}
