using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructuresConfig", menuName = "Procedural Generation/CreateStructures", order = -1)]

public class Structures : ScriptableObject
{

	public List<StructureObject> objects = new List<StructureObject>();
	public List<BiomeType> biomes  = new List<BiomeType>();
	[Range(0,1)]
	public float spawnProbability = 1;
	[Range(0, 1)]
	public float spawnWeight = 0.5f;
	[Tooltip("Only used if non 0 otherwise uses the indivual objects radius.")]
	[Range(0, 25)]
	public float radiusToAffect = 0;
	[Range(0, 90f)]
	public float maxAngle = 90;

	public List<Structures> subStructures = new List<Structures>();
}
