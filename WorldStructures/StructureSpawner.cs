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

	[Header("Spawn Points")]
	public SpawnCreator attacking;
	public float spawnSafeRadius = 30;
	public List<Vector2> spawnPoints = new List<Vector2>();

	//Non-changable values
	private const int REJECTION_SAMPLES = 30;
	public float radiusBetweenStructures = 80;

	public int[,] objectTiles;

	public void initValues(int mapSize)
	{
		spawnPoints.Add(attacking.spawnLocation);
		objectTiles = new int[mapSize, mapSize];
	}


	//Spawn stuff

	public void RunSpawnCreator(TerrainData terrainData, int mapSize, int mapResolution)
	{
		if (attacking != null)
		{
			//Then we run it.
			attacking.PlaceMe(terrainData, this.gameObject, mapSize, mapResolution);
		}
	}




	public struct ReturnData
	{
		public Vector2 point;
		public Vector2 direction;
	}

	public ReturnData GetNextLocations(Vector2 direction, Vector2 point, float randomVariation)
	{
		ReturnData temp = new ReturnData();

		Vector2 directionNormal = new Vector2(direction.y, -direction.x);
		directionNormal += Random.Range(-randomVariation, randomVariation) * Vector2.right + Random.Range(-randomVariation, randomVariation) * Vector2.up;
		directionNormal.Normalize();

		temp.direction = directionNormal;
		temp.point = point;

		return temp;
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
	public Vector2 HandleLineSpawn(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData, 
		int mapSize,Vector2 directionToSpawn, int amountToSpawn, GameObject parantObject, bool ignoreSpawn)
	{
		int amountOfPrimaryObjects = structureToSpawn.primaryObjects.Count;

		Vector2 workingPoint = new Vector2(initialPoint.x, initialPoint.y);
		Vector3 targetPos = new Vector3(workingPoint.x, 0, workingPoint.y);
		WorldObject objectToSpawn = structureToSpawn.primaryObjects[0];//default
		for (int i = 0; i < amountToSpawn; i++)
		{
			objectToSpawn = structureToSpawn.primaryObjects[Random.Range(0, amountOfPrimaryObjects)];

			targetPos = new Vector3(workingPoint.x, 0, workingPoint.y);
			targetPos += terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution) * Vector3.up;
			
			
			float angle = GetSteepnessValue(terrainData, workingPoint);

			float objectSize = objectToSpawn.approxSize.x;


			//calculate next point

			workingPoint += directionToSpawn * objectSize;

			Vector3 lookAngle = new Vector3(workingPoint.x, terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution), workingPoint.y);
			//Spawn Object

			if (ValidPoint(workingPoint, mapSize,ignoreSpawn))
			{
				objectToSpawn.CreateMeLookAt(targetPos, lookAngle, parantObject.transform, angle);
			}
		}
		targetPos.x += directionToSpawn.x * objectToSpawn.approxSize.x;
		targetPos.z += directionToSpawn.y * objectToSpawn.approxSize.x;
		return new Vector2(targetPos.x, targetPos.z);
	}


	public CustomStructureReturnInfo NewSpawnLineObject(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData,
		int mapSize,Transform parantTransform, bool spawnAsSquare, bool ignoreSpawn)
	{
		//spawning in a line.
		int amountToSpawn = Random.Range(structureToSpawn.minObjectsToSpawn, structureToSpawn.maxObjectsToSpawn+1);
		Vector2 directionToSpawn = Random.insideUnitCircle.normalized;

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		Vector2 workingPoint = initialPoint;
		Vector2 workingDirection = directionToSpawn;

		//Spawn the line(s)
		for (int i =0; i<4;i++)
		{
			workingPoint = HandleLineSpawn(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, workingDirection, amountToSpawn, parantObject, ignoreSpawn);
			workingDirection = Vector2.Perpendicular(workingDirection);

			//if not spawning a square then we end after spawning first line.
			if (spawnAsSquare == false)
			{
				return new CustomStructureReturnInfo(initialPoint,directionToSpawn, parantObject.transform);
			}
		}
		return new CustomStructureReturnInfo(initialPoint, directionToSpawn, parantObject.transform);
	}

	private bool ValidPoint(Vector2 workingPoint, int mapSize, bool ignoreSpawn = false)
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
		if (ignoreSpawn)
		{
			return true;
		}

		//Check is it close to a spawn point.
		if (CloseToSpawnCheck(workingPoint) == false)
		{
			return false;//could just return closeToSpawnCheck but i will add more in future
		}

		return true;
	}

	private bool CloseToSpawnCheck(Vector2 point)
	{
		foreach(Vector2 spawnPoint in spawnPoints)
		{
			if (Vector2.SqrMagnitude(point-spawnPoint) < spawnSafeRadius * spawnSafeRadius)
			{
				//Debug.Log("Distance: " + Vector2.SqrMagnitude(point - spawnPoint));
				return false;
			}
		}
		return true;
	}

	private void SpawnObjectOnTerrainSnap(WorldObject objectToSpawn, Vector2 workingPoint,
		int mapSize, int mapResolution, TerrainData terrainData,GameObject parantObject, Vector2 direction, bool ignoreSpawn)
	{

		Vector3 targetPos = new Vector3(workingPoint.x,
			terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution), 
			workingPoint.y);

		if (ValidPoint(workingPoint, mapSize, ignoreSpawn))
		{
			float angle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x);
			Vector3 rotationDirection = new Vector3(0, angle, 0);
			objectToSpawn.CreateMeRotate(targetPos, parantObject.transform, "", terrainData, rotationDirection);
		}
	}




	private void SpawnObject(BiomeObj biomeToSpawn, int mapResolution, Tile[,] Tiles, TerrainData terrainData, int mapSize, bool ignoreSpawn)
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
			//Vector3 normal = terrainData.GetInterpolatedNormal(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
			Vector3 normal = GetNormalValue(terrainData, workingPoint);
			float angle = Vector3.Angle(normal, Vector3.up);
			if (angle > structureToSpawn.maxAngle.y) { continue; }//TODO : implement more than just up

			//Spawn chance.
			if (Random.Range(0f, 1f) > structureToSpawn.structureSpawnProbability) { continue; }

			//Spawn the structure.
			CustomStructureReturnInfo structureInfo = SpawnStructre(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize,this.transform, ignoreSpawn);

			if (structureInfo.parantObject.childCount == 0 && structureInfo.parantObject != this.transform)
			{
				//No strutures got placed. So lets delete the transform.
				DestroyImmediate(structureInfo.parantObject.gameObject);
				continue;
			}
			HandleSubStructures(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);

		}
	}

	public void HandleSubStructures(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize, CustomStructureReturnInfo structureInfo, bool ignoreSpawn)
	{

		if (structureToSpawn.subStructures.Count == 0) { return; }

		foreach(Wrapper_WorldStructures sub in structureToSpawn.subStructures)
		{
			HandleSingleSubStructure(structureToSpawn, sub, workingPoint, structureInfo, mapResolution, mapSize, terrainData, structureInfo.parantObject, ignoreSpawn);
		}
	}

	private void HandleSingleSubStructure(WorldStructure structureToSpawn, Wrapper_WorldStructures sub, Vector2 workingPoint, CustomStructureReturnInfo structureInfo,
		int mapResolution, int mapSize, TerrainData terrainData, Transform parantTransform,bool ignoreSpawn)
	{

		if (structureToSpawn == sub.subStructure) { return; }//infinite loop.
		if (Random.Range(0f, 1f) > sub.subStructure.structureSpawnProbability) { return; }

		//alright we will spawn it but where do we base the point off?

		workingPoint = CalculateSubStructureWorkingPoint(workingPoint, structureToSpawn, structureInfo,sub.subStructure_spawnFrom);

		SpawnStructre(sub.subStructure, workingPoint, mapResolution, terrainData, mapSize, parantTransform, ignoreSpawn);
	}


	private Vector2 CalculateSubStructureWorkingPoint(Vector2 workingPoint, WorldStructure structure, CustomStructureReturnInfo structureInfo, SubStructureSpawnTypes subSpawnType)
	{
		switch (subSpawnType)
		{
			case SubStructureSpawnTypes.spawnFromPointStartPoint:
				return workingPoint;
			case SubStructureSpawnTypes.spawnFromMidPoint:
				return FindMidPoint(structure, structureInfo);
			case SubStructureSpawnTypes.spawnFromPointAndDirectionWithOffset:
				//custom method
				return FindPointFromPointAndDirectionWithOffset(structure, structureInfo);
			case SubStructureSpawnTypes.spawnFromInitialPointWithSingleObjectSizeOffset:
				return FindPointFromSizeOffsett(structure, structureInfo);


			default:
				Debug.Log("Do not have a calculation for: " + subSpawnType);
				return new Vector2();
		}
	}

	private Vector2 FindPointFromSizeOffsett(WorldStructure structure, CustomStructureReturnInfo structureInfo)
	{
		//This should only be used from "one object" strcutures. Its a special case for placing objects "around" a building.
		Vector2 initialPoint = structureInfo.initialPoint;
		float sizeMod = 0.8f;

		if (structure.primaryObjects.Count != 0)
		{
			Vector3 buildingSize = structure.primaryObjects[0].approxSize;
			return initialPoint + sizeMod * (new Vector2(buildingSize.x, buildingSize.z)); 
		}
		Debug.LogError("FindPointFromSizeOffsett error, primary object list is empty.");
		return initialPoint;
	}

	/// <summary>
	/// Method chooses a distance along the line to act as "mid point" then makes a line off it as a certain distance which is the new point to work from.
	/// </summary>
	/// <param name="structure"></param>
	/// <param name="structureInfo"></param>
	/// <returns></returns>
	private Vector2 FindPointFromPointAndDirectionWithOffset(WorldStructure structure, CustomStructureReturnInfo structureInfo)
	{
		switch(structure.spawnType)
		{
			case WorldObjectSpawnTypes.Scatter:
				return structureInfo.initialPoint; // no meaning


			case WorldObjectSpawnTypes.Perpendicular:
				return GetOffSetPointFromDirection(structureInfo.initialPoint, structureInfo.typeDependantInfo, structure);

			case WorldObjectSpawnTypes.Line:
				//gets direction of the line.
				Vector2 direction = (structureInfo.typeDependantInfo - structureInfo.initialPoint).normalized;
				return GetOffSetPointFromDirection(structureInfo.initialPoint, direction, structure);
			case WorldObjectSpawnTypes.Square:
				//gets direction of the line.
				Vector2 lineDirection = (structureInfo.typeDependantInfo - structureInfo.initialPoint).normalized;
				return GetOffSetPointFromDirection(structureInfo.initialPoint, lineDirection, structure);

			default:
				Debug.LogError("Do not have a way to calculate: FindPointFromPointAndDirectionWithOffset from: " + structure.spawnType);
				return Vector2.zero;
		}
		//


		throw new NotImplementedException();
	}


	private Vector2 GetOffSetPointFromDirection(Vector2 initialPoint, Vector2 direction, WorldStructure structure)
	{
		//We start at the initial point and go along the direction a distance X.
		//We then move perpendicular to the direction with a distane of Y (max distance). This is our new point for the SubStructure.
		float disMod = Random.Range(structure.perpendicular_MinDistanceToSpawn, structure.perpendicular_MaxDistanceToSpawn);
		Vector2 directionAway = Vector2.Perpendicular(direction);

		Vector2 startPoint = initialPoint + direction * disMod;
		float disMod2 = Random.Range(structure.perpendicular_MinDistanceToSpawn, structure.perpendicular_MaxDistanceToSpawn);

		return startPoint + disMod2 * directionAway;
	}
	private Vector2 FindMidPoint(WorldStructure structure, CustomStructureReturnInfo structureInfo)
	{
		switch(structure.spawnType)
		{
			case WorldObjectSpawnTypes.Line:
				return (structureInfo.initialPoint + structureInfo.typeDependantInfo) / 2;
			case WorldObjectSpawnTypes.Square:
				return (structureInfo.initialPoint + structureInfo.typeDependantInfo) / 2;


			case WorldObjectSpawnTypes.Perpendicular:
				return structureInfo.initialPoint;//mid point has no meaning in a scatter
			case WorldObjectSpawnTypes.Scatter:
				return structureInfo.initialPoint;//mid point has no meaning in a scatter
			default:
				Debug.LogError("Could not find a method for finding midpoint for type: " + structure.spawnType);
				return Vector2.zero;
		}
		throw new NotImplementedException();
	}

	public CustomStructureReturnInfo SpawnStructre(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform,bool ignoreSpawn)
	{
		switch (structureToSpawn.spawnType)
		{
			case WorldObjectSpawnTypes.Single:
				return SingleObjectPlacement(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform, ignoreSpawn);
			case WorldObjectSpawnTypes.Line:
				return NewSpawnLineObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform,false, ignoreSpawn);
			case WorldObjectSpawnTypes.Square:
				return NewSpawnLineObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform, true, ignoreSpawn);
			case WorldObjectSpawnTypes.Scatter:
				return SpawnScatterObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize, parantTransform, ignoreSpawn);
			case WorldObjectSpawnTypes.Perpendicular:
				return SpawnPerpendicularObject(structureToSpawn, workingPoint, mapResolution, terrainData, mapSize,parantTransform, ignoreSpawn);

			default:
				Debug.Log("Not calculated for: " + structureToSpawn.spawnType);
				return new CustomStructureReturnInfo();
		}
	}

	private CustomStructureReturnInfo SingleObjectPlacement(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution,
		TerrainData terrainData, int mapSize, Transform parantTransform,bool ignoreSpawn)
	{
		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		if (structureToSpawn.primaryObjects.Count == 0) { Debug.LogError("Error: SingleObjectPlacement"); return new CustomStructureReturnInfo(workingPoint, workingPoint, parantObject.transform); }

		//place the object.
		WorldObject building = structureToSpawn.primaryObjects[Random.Range(0, structureToSpawn.primaryObjects.Count)];

		float height = terrainData.GetInterpolatedHeight(workingPoint.x / mapResolution, workingPoint.y / mapResolution);
		Vector3 targetPosition = new Vector3(workingPoint.x, height, workingPoint.y);
		if (ValidPoint(workingPoint, mapSize, ignoreSpawn))
		{
			building.CreateMeSnapInternal(targetPosition, parantObject.transform, "", terrainData);
			return new CustomStructureReturnInfo(workingPoint, workingPoint, parantObject.transform);
		}
		return new CustomStructureReturnInfo(workingPoint, workingPoint, parantObject.transform);
	}

	private CustomStructureReturnInfo SpawnPerpendicularObject(WorldStructure structureToSpawn, Vector2 workingPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform, bool ignoreSpawn)
	{
		//spawning in a line.
		Vector3 maxAngle = structureToSpawn.maxAngle;

		int amountToSpawn = Random.Range(structureToSpawn.minObjectsToSpawn, structureToSpawn.maxObjectsToSpawn + 1);
		Vector2 directionToSpawn = Random.insideUnitCircle.normalized;

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		ReturnData currentData = new ReturnData()
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


			SpawnObjectOnTerrainSnap(objectToSpawn, currentData.point, mapSize, mapResolution, terrainData, parantObject,currentData.direction, ignoreSpawn);
		}
		return new CustomStructureReturnInfo(currentData.point,currentData.direction, parantObject.transform);

	}

	private CustomStructureReturnInfo SpawnScatterObject(WorldStructure structureToSpawn, Vector2 initialPoint, int mapResolution, TerrainData terrainData, int mapSize,
		Transform parantTransform, bool ignoreSpawn)
	{

		int amountOfPrimaryObjects = structureToSpawn.primaryObjects.Count;

		if (amountOfPrimaryObjects == 0) { Debug.LogError(string.Format("Structure: {0} does not have any primary objects", structureToSpawn.name)); return new CustomStructureReturnInfo(); }

		//Create Parant Object. 
		GameObject parantObject = new GameObject(structureToSpawn.name);
		parantObject.transform.SetParent(parantTransform);

		float radius = structureToSpawn.scatterRadius;
		Vector2 scatterSize = structureToSpawn.scatterSize;
		Vector2 offset = new Vector2(initialPoint.x, initialPoint.y) - 0.5f * structureToSpawn.scatterSize;

		List <Vector2> validPoints = ObjectPlacer.GeneratePoints(radius, scatterSize, REJECTION_SAMPLES);

		int amountToSpawn = Random.Range(structureToSpawn.minObjectsToSpawn, structureToSpawn.maxObjectsToSpawn+1);

		int maxItterations = 60;
		while (amountToSpawn > 0)
		{
			if (validPoints.Count == 0 || maxItterations < 0) { break; }
			maxItterations -= 1;

			//Choose a random point.
			Vector2 workingPoint2 = validPoints[Random.Range(0, validPoints.Count)];
			//Remove it from the list.
			validPoints.Remove(workingPoint2);
			//Try spawn at the place.

			Vector2 currentPoint = workingPoint2 + offset;

			WorldObject objectToSpawn = structureToSpawn.primaryObjects[Random.Range(0, amountOfPrimaryObjects)];

			float height = terrainData.GetInterpolatedHeight(currentPoint.x / mapResolution, currentPoint.y / mapResolution);
			Vector3 targetPosition = new Vector3(currentPoint.x, height, currentPoint.y);

			if (ValidPoint(currentPoint, mapSize, ignoreSpawn))
			{
				objectToSpawn.CreateMeSnapInternal(targetPosition, parantObject.transform, "", terrainData);
				amountToSpawn -= 1;
			}
		}
		return new CustomStructureReturnInfo(initialPoint, Vector2.one * radius, parantObject.transform);
	}

	public void SpawnStructuresForBiomes(int mapResolution, Tile[,] Tiles, TerrainData terrainData, int mapSize)
	{
		DestroyPreviousChildren();

		BiomeType[] enumArray = (BiomeType[])System.Enum.GetValues(typeof(BiomeType));
		foreach (BiomeType b in enumArray)
		{
			//SpawnObject(b, mapResolution, Tiles, terrainData);
			BiomeObj bOb = BiomeManager.Instance.biomeMapping[b.ToString()];
			SpawnObject(bOb, mapResolution, Tiles, terrainData, mapSize, false);
		}
	}


	// PRIVATE METHODS


	public static float GetSteepnessValue(TerrainData terrainData, Vector2 workingPoint)
	{
		float y_01 = (float)workingPoint.y / (float)terrainData.heightmapResolution;
		float x_01 = (float)workingPoint.x / (float)terrainData.heightmapResolution;
		return terrainData.GetSteepness(y_01, x_01);

	}

	public static float GetSteepnessValue(TerrainData terrainData, Vector3 workingPoint)
	{
		float y_01 = (float)workingPoint.z / (float)terrainData.heightmapResolution;
		float x_01 = (float)workingPoint.x / (float)terrainData.heightmapResolution;
		return terrainData.GetSteepness(y_01, x_01);

	}


	public static Vector3 GetNormalValue(TerrainData terrainData, Vector2 workingPoint)
	{
		int heightMapResolution = terrainData.heightmapResolution;
		return terrainData.GetInterpolatedNormal(workingPoint.x / heightMapResolution, workingPoint.y / heightMapResolution);
	}

	public static Vector3 GetNormalValue(TerrainData terrainData, Vector3 workingPoint)
	{
		int heightMapResolution = terrainData.heightmapResolution;
		return terrainData.GetInterpolatedNormal(workingPoint.x / heightMapResolution, workingPoint.z / heightMapResolution);
	}

	public struct CustomStructureReturnInfo
	{
		public Vector2 initialPoint;
		public Vector2 typeDependantInfo;
		public Transform parantObject;

		public CustomStructureReturnInfo(Vector2 i , Vector2 t, Transform p)
		{
			initialPoint = i;
			typeDependantInfo = t;
			parantObject = p;
		}
	}
}
