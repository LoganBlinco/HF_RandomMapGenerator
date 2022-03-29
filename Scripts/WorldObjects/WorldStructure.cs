using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace eLF_RandomMaps
{

	[CreateAssetMenuAttribute(fileName = "WorldStructure:", menuName = "TerrainGen/CreatreStructure")]

	public class WorldStructure : ScriptableObject
	{
		public WorldObjectSpawnTypes spawnType = WorldObjectSpawnTypes.Line;

		public List<WorldObject> primaryObjects = new List<WorldObject>();

		[Range(0, 1)]
		public float structureSpawnProbability = 1;
		[Range(0, 1)]
		public float structureSpawnWeight = 1;

		public Vector3 maxAngle = new Vector3(0, 90, 0);


		public int minObjectsToSpawn = 1;
		public int maxObjectsToSpawn = 4;

		[Header("Scatter settings -- ONLY USED FOR SCATTER MODE")]
		public float scatterRadius = 5;
		public Vector2 scatterSize = new Vector2(10, 10);

		[Header("Perpendicular settings -- ONLY USED FOR Perpendicular MODE")]
		public float perpendicular_AngleVariance = 15f;
		public float perpendicular_MinDistanceToSpawn = 1.5f;
		public float perpendicular_MaxDistanceToSpawn = 7;

		[Header("SubStructure settings -- WIP")]
		//public WorldStructure subStructure;
		//public SubStructureSpawnTypes subStructure_spawnFrom = SubStructureSpawnTypes.spawnFromPointStartPoint;

		[SerializeField]
		public List<Wrapper_WorldStructures> subStructures = new List<Wrapper_WorldStructures>();

		public List<HeightMap_ModifierData> HeightMapModifiers = new List<HeightMap_ModifierData>();
	}



	[System.Serializable]
	public class Wrapper_WorldStructures
	{
		[SerializeField]
		public WorldStructure subStructure;
		[SerializeField]
		public SubStructureSpawnTypes subStructure_spawnFrom = SubStructureSpawnTypes.spawnFromPointStartPoint;
	}
}