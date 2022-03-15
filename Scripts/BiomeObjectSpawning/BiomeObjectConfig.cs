using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeObjectConfigObj", menuName = "Procedural Generation/BiomeObject", order = -1)]
public class BiomeObjectConfig : ScriptableObject
{
	[Range(0, 1)]
	public float spawnProbability = 1;
	[Range(0, 1)]
	public float spawnWeight = 1;
	[Range(0, 90)]
	public float maxAngle = 25;
	public float heightOffset = -0.1f;
	public List<BiomeType> biomes = new List<BiomeType>()
	{
		BiomeType.BorealForest,
		BiomeType.Desert,
		BiomeType.Grassland,
		BiomeType.Ice,
		BiomeType.Rainforest,
		BiomeType.Savanna,
		BiomeType.SeasonalForest,
		BiomeType.Tundra,
		BiomeType.Woodland,
	};
	[SerializeField]
	public GameObject objectPrefab;
}
