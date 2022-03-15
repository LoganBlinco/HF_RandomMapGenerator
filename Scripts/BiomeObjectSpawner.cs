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
	//Non-changable values
	private const int REJECTION_SAMPLES = 30;

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

			//Then get infomation for the point
			float height = terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
			Vector3 targetPosition = new Vector3(workingPoint.x, height, workingPoint.y);

			//Spawn object.
			objectToSpawn.CreateMeSnapInternal(targetPosition, transform, biomeToSpawn.getName(), terrainData);
		}
	}

}
