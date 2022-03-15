using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GetColliderSize))]
public class GetSizeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GetColliderSize myScript = (GetColliderSize)target;
		if (GUILayout.Button("Get size!"))
		{
			myScript.GetMySize();
		}
	}
}
