using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureObjectConfig", menuName = "Procedural Generation/StructureObject", order = -1)]
public class StructureObject : ScriptableObject
{
	public GameObject prefab;
	[Range(0,20f)]
	public float radiusToAffect = 0;
	[Range(-5f, 5f)]
	public float heightOffset = -1.2f;
	[Range(0,1f)]
	public float spawnProbability = 1;
}
