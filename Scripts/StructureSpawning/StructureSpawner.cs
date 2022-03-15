using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureSpawner : MonoBehaviour
{
	private static StructureSpawner _instance;
	public static StructureSpawner Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<StructureSpawner>();
			}
			return _instance;
		}
	}

	public List<Structures> structures = new List<Structures>();
	public Dictionary<string, List<Structures>> biomeToStructures;
	public Dictionary<string, float> biomeToWeight;



	//Non-changable values
	private const int REJECTION_SAMPLES = 30;
	public float radiusBetweenStructures = 80;


	public void Initialize()
	{
		biomeToStructures = new Dictionary<string, List<Structures>>();
		biomeToWeight = new Dictionary<string, float>();


		BiomeType[] enumArray = (BiomeType[])System.Enum.GetValues(typeof(BiomeType));
		foreach (BiomeType b in enumArray)
		{
			biomeToStructures[b.ToString()] = new List<Structures>();
			biomeToWeight[b.ToString()] = 0;
		}
		PopulateDictionaries();
	}

	public struct returnData
	{
		public Vector2 point;
		public Vector2 direction;
	}

	public returnData GetNextLocations(Vector2 direction, Vector2 point, float randomVariation)
	{
		returnData temp = new returnData();

		Vector2 directionNormal = new Vector2(direction.y, -direction.x);
		directionNormal += Random.Range(-randomVariation, randomVariation) * Vector2.right + Random.Range(-randomVariation, randomVariation) * Vector2.up;
		directionNormal.Normalize();

		temp.direction = directionNormal;
		temp.point = point;

		return temp;
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





	private void PopulateDictionaries()
	{
		foreach (Structures config in structures)
		{
			foreach (BiomeType b in config.biomes)
			{
				biomeToStructures[b.ToString()].Add(config);
				biomeToWeight[b.ToString()] = biomeToWeight[b.ToString()] + config.spawnWeight;
			}
		}
	}








	//NEW STUFF

	
	/// <summary>
	/// Spawns the objects in a line. Returns the position of the final point.
	/// </summary>
	/// <param name="structureToSpawn"></param>
	/// <param name="initialPoint"></param>
	/// <param name="mapResolution"></param>
	/// <param name="terrainData"></param>
	/// <param name="mapSize"></param>
	/// <param name="directionToSpawn"></param>
	/// <param name="amountToSpawn"></param>
	/// <param name="parantObject"></param>
	/// <returns></returns>
	private Vector2 HandleLineSpawn(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData, 
		int mapSize,Vector2 directionToSpawn, int amountToSpawn, GameObject parantObject)
	{
		float maxAngle = structureToSpawn.maxAngle.y;

		int amountOfPrimaryObjects = structureToSpawn.primaryObjects.Count;

		Vector2 workingPoint = new Vector2(initialPoint.x, initialPoint.y);
		Vector3 targetPos = new Vector3(workingPoint.x, 0, workingPoint.y);
		WorldObject objectToSpawn = structureToSpawn.primaryObjects[0];//default
		for (int i = 0; i < amountToSpawn; i++)
		{
			objectToSpawn = structureToSpawn.primaryObjects[Random.Range(0, amountOfPrimaryObjects)];

			targetPos = new Vector3(workingPoint.x, 0, workingPoint.y);
			targetPos += objectToSpawn.heightOffset;
			targetPos += terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution) * Vector3.up;

			float y_01 = (float)workingPoint.y / (float)terrainData.alphamapHeight;
			float x_01 = (float)workingPoint.x / (float)terrainData.alphamapWidth;
			float angle = terrainData.GetSteepness(y_01, x_01);

			if (angle > maxAngle) { continue; }

			if (Random.Range(0f, 1f) > objectToSpawn.spawnProbability) { continue; }

			//wobble amount


			float objectSize = objectToSpawn.approxSize.x;


			//calculate next point

			workingPoint += directionToSpawn * objectSize;

			Vector3 lookAngle = new Vector3(workingPoint.x, terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution), workingPoint.y);
			//Spawn Object

			if (ValidPoint(workingPoint, mapSize))
			{
				var tmp = SpawnObjectLookTowards(objectToSpawn.objectPrefab, targetPos, lookAngle, parantObject.transform, objectToSpawn.pivotFix);
			}
		}
		targetPos.x += directionToSpawn.x * objectToSpawn.approxSize.x;
		targetPos.z += directionToSpawn.y * objectToSpawn.approxSize.x;
		return new Vector2(targetPos.x, targetPos.z);
	}


	public void NewSpawnLineObject(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData,
		int mapSize,Transform parantTransform, bool spawnAsSquare = false)
	{
		//spawning in a line.
		int amountToSpawn = Random.Range(structureToSpawn.minObjectsToSpawn, structureToSpawn.maxObjectsToSpawn+1);
		Vector2 directionToSpawn = Random.insideUnitCircle.normalized;

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		//Spawn the line(s)
		for (int i =0; i<4;i++)
		{
			initialPoint = HandleLineSpawn(structureToSpawn, initialPoint, mapResolution, terrainData, mapSize, directionToSpawn, amountToSpawn, parantObject);
			directionToSpawn = Vector2.Perpendicular(directionToSpawn);

			//if not spawning a square then we end after spawning first line.
			if (spawnAsSquare == false)
			{
				return;
			}
		}
	}

	private bool ValidPoint(Vector2 workingPoint, int mapSize)
	{
		//Must be inside the map.
		if (workingPoint.x < 0 || workingPoint.x > mapSize)
		{
			return false;
		}
		if (workingPoint.y < 0 || workingPoint.y > mapSize)
		{
			return false;
		}
		return true;
	}

	private GameObject SpawnObjectLookTowards(GameObject objectToSpawn, Vector3 targetPos, Vector3 lookAngle, Transform parant,Vector3 pivotFix)
	{
		Quaternion spawnAngle = Quaternion.identity;
		GameObject spawnedObj = Instantiate(objectToSpawn, targetPos, spawnAngle, parant);
		spawnedObj.transform.LookAt(lookAngle);
		spawnedObj.transform.Rotate(pivotFix);
		return spawnedObj;
	}

	private void NewSpawnObjectSnap(GameObject objectToSpawn, Vector3 targetPos, Vector3 normal, Transform parant,
		float randomYAngle)
	{
		Quaternion spawnAngle = Quaternion.identity;
		GameObject spawnedObj = Instantiate(objectToSpawn, targetPos, spawnAngle, parant);
		spawnedObj.transform.up = normal;

		spawnedObj.transform.Rotate(new Vector3(0, randomYAngle));
	}


	private void SpawnObjectOnTerrainSnap(WorldObject objectToSpawn, Vector2 workingPoint,
		int mapSize, int mapResolution, TerrainData terrainData,GameObject parantObject)
	{

		//HANDLE PHYSCIALLY SPAWNING

		Vector3 targetPos = new Vector3(workingPoint.x, 0, workingPoint.y);
		targetPos += objectToSpawn.heightOffset;
		targetPos += terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution) * Vector3.up;

		float y_01 = (float)workingPoint.y / (float)terrainData.alphamapHeight;
		float x_01 = (float)workingPoint.x / (float)terrainData.alphamapWidth;
		float angle = terrainData.GetSteepness(y_01, x_01);
		Vector3 normal = terrainData.GetInterpolatedNormal(workingPoint.x / mapResolution, workingPoint.y / mapResolution);

		if (angle > objectToSpawn.maxAngle.y) { return; }

		if (Random.Range(0f, 1f) > objectToSpawn.spawnProbability) { return; }


		float randomAngle = Random.Range(0, objectToSpawn.yAxisRotationRandomness);


		if (ValidPoint(workingPoint, mapSize))
		{
			NewSpawnObjectSnap(objectToSpawn.objectPrefab, targetPos, normal, parantObject.transform, randomAngle);
		}
	}




	private void AltSpawnObject(BiomeObj biomeToSpawn, int mapResolution, Tile[,] Tiles, TerrainData terrainData, int mapSize)
	{

		if (biomeToSpawn.lowDensityObjects.Count == 0) { Debug.Log("no structures for the biome: " + biomeToSpawn); return; }

		float radius = biomeToSpawn.lowDensityObjectRadius;
		Vector2 heightMapSize = mapResolution * Vector2.one;
		List<Vector2> validPoints = ObjectPlacer.GeneratePoints(radius, heightMapSize, REJECTION_SAMPLES);
		foreach (Vector2 workingPoint in validPoints)
		{

			//Determine if its in the specific biome.
			string workingBiome = Tiles[(int)workingPoint.y, (int)workingPoint.x].primaryBiomeType;
			if (workingBiome != biomeToSpawn.getName()) { continue; }

			//If so, then we determine what object to spawn at the point.
			WorldStructure structureToSpawn = biomeToSpawn.GetStructureToSpawn();
			if (structureToSpawn == null) { Debug.Log(string.Format("Biome: {0} returned null.", biomeToSpawn)); continue; }

			//Is angle valid?
			Vector3 normal = terrainData.GetInterpolatedNormal(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
			float angle = Vector3.Angle(normal, Vector3.up);
			if (angle > structureToSpawn.maxAngle.y) { continue; }//TODO : implement more than just up

			//Spawn chance.
			if (Random.Range(0f, 1f) > structureToSpawn.structureSpawnProbability) { continue; }

			//Spawn the structure.
			AltSpawnStructre(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize,this.transform);

			HandleSubStructure(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize);
		}
	}

	private void HandleSubStructure(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize)
	{

		if (structureToSpawn.subStructure == null) { return; }
		if (structureToSpawn == structureToSpawn.subStructure) { return; }//infinite loop.
		if (Random.Range(0f, 1f) > structureToSpawn.subStructure.structureSpawnProbability) { return; }

		//alright we will spawn it but where do we base the point off?

		workingPoint = CalculateSubStructureWorkingPoint(workingPoint, structureToSpawn.subStructure);

		AltSpawnStructre(structureToSpawn.subStructure, workingPoint, mapResolution, terrainData, mapSize,this.transform);
	}

	private Vector2 CalculateSubStructureWorkingPoint(Vector2 workingPoint, WorldStructure subStructure)
	{
		switch(subStructure.subStructure_spawnFrom)
		{
			case SubStructureSpawnTypes.spawnFromPoint:
				return workingPoint;
			default:
				Debug.Log("Do not have a calculation for: " + subStructure.subStructure_spawnFrom);
				return new Vector2();
		}
	}

	private void AltSpawnStructre(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform)
	{
		switch (structureToSpawn.spawnType)
		{
			case WorldObjectSpawnTypes.Line:
				NewSpawnLineObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform);
				break;
			case WorldObjectSpawnTypes.Square:
				NewSpawnLineObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform, true);
				break;
			case WorldObjectSpawnTypes.Scatter:
				NewSpawnScatterObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform);
				break;
			case WorldObjectSpawnTypes.Perpendicular:
				NewSpawnPerpendicularObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize,parantTransform);
				break;

			default:
				Debug.Log("Not calculated for: " + structureToSpawn.spawnType);
				break;
		}
	}

	private void NewSpawnPerpendicularObject(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform)
	{
		//spawning in a line.
		Vector3 maxAngle = structureToSpawn.maxAngle;

		int amountToSpawn = Random.Range(structureToSpawn.minObjectsToSpawn, structureToSpawn.maxObjectsToSpawn + 1);
		Vector2 directionToSpawn = Random.insideUnitCircle.normalized;

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		returnData currentData = new returnData()
		{
			direction = directionToSpawn,
			point = workingPoint
		};

		int amountOfObjects = structureToSpawn.primaryObjects.Count;

		for (int i =0;i<amountToSpawn;i++)
		{
			WorldObject objectToSpawn = structureToSpawn.primaryObjects[Random.Range(0, amountOfObjects)];

			currentData = GetNextLocations(currentData.direction, currentData.point, structureToSpawn.perpendicular_AngleVariance);
			currentData.point += currentData.direction * Random.Range(structureToSpawn.perpendicular_MinDistanceToSpawn, structureToSpawn.perpendicular_MaxDistanceToSpawn);


			SpawnObjectOnTerrainSnap(objectToSpawn, currentData.point, mapSize, mapResolution, terrainData, parantObject);
		}

	}

	private void NewSpawnScatterObject(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform)
	{

		int amountOfPrimaryObjects = structureToSpawn.primaryObjects.Count;

		if (amountOfPrimaryObjects == 0) { Debug.LogError(string.Format("Structure: {0} does not have any primary objects", structureToSpawn.name)); return; }

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		float radius = structureToSpawn.scatterRadius;
		Vector2 scatterSize = structureToSpawn.scatterSize;
		Vector2 offset = new Vector2(initialPoint.x, initialPoint.y);

		List <Vector2> validPoints = ObjectPlacer.GeneratePoints(radius, scatterSize, REJECTION_SAMPLES);


		foreach (Vector2 workingPoint in validPoints)
		{
			Vector2 currentPoint = workingPoint + offset;

			WorldObject objectToSpawn = structureToSpawn.primaryObjects[Random.Range(0, amountOfPrimaryObjects)];

			float y_01 = (float)currentPoint.y / (float)terrainData.alphamapHeight;
			float x_01 = (float)currentPoint.x / (float)terrainData.alphamapWidth;
			float angle = terrainData.GetSteepness(y_01, x_01);

			if (angle > objectToSpawn.maxAngle.y) { continue; }

			//Spawn chance.
			if (Random.Range(0f, 1f) > objectToSpawn.spawnProbability) { continue; }


			float height = terrainData.GetInterpolatedHeight(currentPoint.x / mapResolution, currentPoint.y / mapResolution);
			//Spawn the object in correct position.
			Vector3 targetPosition = new Vector3(currentPoint.x, height, currentPoint.y) + objectToSpawn.heightOffset;
			Vector3 normal = terrainData.GetInterpolatedNormal(currentPoint.x / mapResolution, currentPoint.y / mapResolution);

			if (ValidPoint(currentPoint, mapSize))
			{
				NewSpawnObjectSnap(objectToSpawn.objectPrefab, targetPosition, normal, parantObject.transform,0);
			}
		}
	}

	public void AltSpawnStructuresForBiomes(int mapResolution, Tile[,] Tiles, TerrainData terrainData, int mapSize)
	{
		DestroyPreviousChildren();

		BiomeType[] enumArray = (BiomeType[])System.Enum.GetValues(typeof(BiomeType));
		foreach (BiomeType b in enumArray)
		{
			//SpawnObject(b, mapResolution, Tiles, terrainData);
			BiomeObj bOb = BiomeManager.Instance.biomeMapping[b.ToString()];
			AltSpawnObject(bOb, mapResolution, Tiles, terrainData, mapSize);
		}
	}
}
