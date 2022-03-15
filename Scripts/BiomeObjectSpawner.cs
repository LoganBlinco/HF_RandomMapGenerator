using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObjectSpawner : MonoBehaviour
{
	private static BiomeObjectSpawner _instance;
	public static BiomeObjectSpawner Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<BiomeObjectSpawner>();
			}
			return _instance;
		}
	}

	[Header("Biome Spawning Settings")]
	public float DesertRadius = 20;
	public float SavannaRadius = 20;
	public float GrasslandRadius = 20;
	public float WoodlandRadius = 20;
	public float SeasonalForestRadius = 20;
	public float RainforestRadius = 20;
	public float BorealForestRadius = 20;
	public float TundraRadius = 20;
	public float IceRadius = 20;


	public Dictionary<BiomeType, float> biomeToWeight;


	//Non-changable values
	private const int REJECTION_SAMPLES = 30;

	public void Initialize()
	{

		biomeToWeight = new Dictionary<BiomeType, float>();

		BiomeType[] enumArray = (BiomeType[])System.Enum.GetValues(typeof(BiomeType));
		foreach (BiomeType b in enumArray)
		{
			biomeToWeight[b] = 0;
		}
		PopulateDictionaries();
	}

	public void PopulateDictionaries()
	{
	}

	public void SpawnObjectsForBiomes(int mapResolution, Tile[,] Tiles, TerrainData terrainData)
	{
		DestroyPreviousChildren();

		BiomeType[] enumArray = (BiomeType[])System.Enum.GetValues(typeof(BiomeType));
		foreach(BiomeType b in enumArray)
		{
			//SpawnObject(b, mapResolution, Tiles, terrainData);
			BiomeObj bOb = BiomeManager.Instance.biomeMapping[b.ToString()];
			AltSpawnObject(bOb, mapResolution, Tiles, terrainData);
		}
	}

	private void DestroyPreviousChildren()
	{
		Transform parant = transform;
		Transform[] children = (Transform[])GetComponentsInChildren<Transform>();
		foreach (Transform tmp in children)
		{
			if (tmp == transform) { continue; }
			try
			{
				//tmp.gameObject.name = "I would be destroyed";
				DestroyImmediate(tmp.gameObject);
			}
			catch (MissingReferenceException)
			{
				//occurs if you delete a parant of an object i think.
			}
		}
	}

	private float GetBiomeRadius(BiomeType b)
	{
		switch(b)
		{
			case BiomeType.Desert:
				return DesertRadius;
			case BiomeType.Savanna:
				return SavannaRadius;
			case BiomeType.Grassland:
				return GrasslandRadius;
			case BiomeType.Woodland:
				return WoodlandRadius;
			case BiomeType.SeasonalForest:
				return SeasonalForestRadius;
			case BiomeType.Rainforest:
				return RainforestRadius;
			case BiomeType.BorealForest:
				return BorealForestRadius;
			case BiomeType.Tundra:
				return TundraRadius;
			case BiomeType.Ice:
				return IceRadius;
			default:
				Debug.Log("this wont happen");
				return -1;
		}
	}




	//NEW OBJECT SPAWNER SYSTEM

	private void AltSpawnObject(BiomeObj biomeToSpawn, int mapResolution, Tile[,] Tiles, TerrainData terrainData)
	{

		if (biomeToSpawn.middleDensityObjects.Count == 0) { Debug.Log("no objects for the biome: "+biomeToSpawn); return; }

		float radius = biomeToSpawn.middleDensityObjectRadius;
		Vector2 heightMapSize = mapResolution * Vector2.one;
		List<Vector2> validPoints = ObjectPlacer.GeneratePoints(radius, heightMapSize, REJECTION_SAMPLES);
		foreach (Vector2 workingPoint in validPoints)
		{
			//Determine if its in the specific biome.
			string workingBiome = Tiles[(int)workingPoint.y, (int)workingPoint.x].primaryBiomeType;
			if (workingBiome != biomeToSpawn.getName()) { continue; }

			//If so, then we determine what object to spawn at the point.
			WorldObject objectToSpawn = biomeToSpawn.GetObjectToSpawn();
			if (objectToSpawn == null) { Debug.Log(string.Format("Biome: {0} returned null.", biomeToSpawn)); continue; }

			//Is angle valid?
			Vector3 normal =  terrainData.GetInterpolatedNormal(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
			float angle = Vector3.Angle(normal, Vector3.up);
			if (angle > objectToSpawn.maxAngle.y) { continue; }//TODO : implement more than just up

			//Spawn chance.
			if (Random.Range(0f, 1f) > objectToSpawn.spawnProbability) { continue; }
			float height = terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
			//Spawn the object in correct position.
			Vector3 targetPosition = new Vector3(workingPoint.x, height, workingPoint.y) + objectToSpawn.heightOffset;
			Quaternion spawnAngle = Quaternion.Euler(0, 0, 0);
			try
			{
				var tmp = Instantiate(objectToSpawn.objectPrefab, targetPosition, spawnAngle, transform);
				tmp.name = biomeToSpawn.getName() +": "+ tmp.name;
				if (objectToSpawn.snapYRotation)
				{
					tmp.transform.up = normal;
				}
			}
			catch (UnassignedReferenceException)
			{
				Debug.Log("error with: " + objectToSpawn.name);
			}

		}
	}

}
