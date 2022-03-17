using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Spawn:", menuName = "TerrainGen/CreateSpawn")]
public class SpawnCreator : ScriptableObject
{
	public Vector2 spawnLocation;
	public Vector2 pointToFace;

	public List<WorldStructure> centerStructures = new List<WorldStructure>();

	public List<WorldStructure> Zone3_Props = new List<WorldStructure>();
	public float minForwardAngle = 20;
	public float maxForwardAngle = 45;
	public float minArtyDistance = 10;
	public float maxArtyDistance = 25;


	public List<WorldStructure> Zone2_Props = new List<WorldStructure>();
	public float Zone2SpawnChance = 0.5f;
	public float minZone2Distance = 5;
	public float maxZone2Distance = 10;

	public List<WorldStructure> Zone4_Props = new List<WorldStructure>();
	public float Zone4_CloseMinDistance = 5;
	public float Zone4_CloseMaxDistance = 10;
	public float Zone4_FarMinDistance = 20;
	public float Zone4_FarMaxDistance = 30;

	public List<WorldStructure> Zone5_Props = new List<WorldStructure>();
	public float Zone5SpawnChance = 0.5f;
	public float Zone5_MinDistanceAddition = 5;
	public float Zone5_MaxDistanceAddition = 10;
	public float Zone5_MinAngleAddition = 5;
	public float Zone5_MaxAngle = 60;

	public List<WorldStructure> Zone6_Props = new List<WorldStructure>();
	public float Zone6SpawnChance = 0.45f;
	public float Zone6_MinDistanceAddition = 5;
	public float Zone6_MaxDistanceAddition = 10;
	public float Zone6_MaxAngle = 45;


	private bool ignoreSpawn = true;


	public void PlaceMe(TerrainData terrainData, GameObject parant, int mapSize, int mapResolution)
	{

		Vector2 spawnToSpawnVector = (pointToFace - spawnLocation).normalized;



		GameObject parantObject = new GameObject(this.name);
		parantObject.transform.SetParent(parant.transform);


		//Place center structure.
		PlaceCenterStructure(terrainData, mapSize, mapResolution, parantObject.transform);

		float positiveAngle = Random.Range(minForwardAngle, maxForwardAngle);

		Vector2 positiveDir = Rotate(spawnToSpawnVector, positiveAngle);


		Vector2 nativeDir = Rotate(spawnToSpawnVector, -positiveAngle);

		float artyDistance = Random.Range(minArtyDistance, maxArtyDistance);

		Vector2 workingPositive = spawnLocation + artyDistance * positiveDir;
		Vector2 workingNegative = spawnLocation + artyDistance * nativeDir;
		Vector2 straightArtyCoverPosition = spawnLocation + artyDistance * spawnToSpawnVector;


		WorldStructure positiveS = Zone3_Props[Random.Range(0, Zone3_Props.Count)];
		WorldStructure negativeS = Zone3_Props[Random.Range(0, Zone3_Props.Count)];
		WorldStructure straightArtyCover = Zone3_Props[Random.Range(0, Zone3_Props.Count)];




		PlaceZone2(terrainData, mapResolution,mapSize,parantObject, spawnToSpawnVector);

		//Placeing zone 3
		CustomSingleStructurePlacer(positiveS, workingPositive, mapResolution, terrainData, mapSize, parantObject);

		CustomSingleStructurePlacer(negativeS, workingNegative, mapResolution, terrainData, mapSize, parantObject);

		CustomSingleStructurePlacer(straightArtyCover, straightArtyCoverPosition, mapResolution, terrainData, mapSize, parantObject);
		//zone 3 done

		PlaceZone4(terrainData, mapResolution, mapSize, parantObject, spawnToSpawnVector, workingPositive, workingNegative);

		PlaceZone5(terrainData, mapResolution, mapSize, parantObject, spawnToSpawnVector,positiveAngle,artyDistance);

		PlaceZone6(terrainData, mapResolution, mapSize, parantObject, spawnToSpawnVector, artyDistance);


	}

	private void PlaceZone6(TerrainData terrainData, int mapResolution, int mapSize, GameObject parantObject, Vector2 spawnToSpawnVector, float artyDistance)
	{
		//for every corner
		Vector2 perp = Vector2.Perpendicular(spawnToSpawnVector);

		float distance = Random.Range(artyDistance + Zone6_MinDistanceAddition, artyDistance + Zone6_MaxDistanceAddition);
		//upper left
		float angle1 = Random.Range(0, Zone6_MaxAngle);
		//lower left
		float angle2 = Random.Range(0, Zone6_MaxAngle);
		//upper right
		float angle3 = Random.Range(0, Zone6_MaxAngle);
		//lower right
		float angle4 = Random.Range(0, Zone6_MaxAngle);


		Vector2 targetPos1 = spawnLocation + distance * Rotate(perp,angle1);

		Vector2 targetPos2 = spawnLocation + distance * Rotate(perp, -angle2);

		Vector2 targetPos3 = spawnLocation - distance * Rotate(perp, angle3);

		Vector2 targetPos4 = spawnLocation - distance * Rotate(perp, -angle4);


		if (Random.Range(0f, 1f) <= Zone6SpawnChance) 
		{
			WorldStructure strcutureToSpawn = Zone6_Props[Random.Range(0, Zone6_Props.Count)];
			var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetPos1, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
			StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, targetPos1, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);
		}
		if (Random.Range(0f, 1f) <= Zone6SpawnChance)
		{
			WorldStructure strcutureToSpawn = Zone6_Props[Random.Range(0, Zone6_Props.Count)];
			var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetPos2, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
			StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, targetPos2, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);
		}
		if (Random.Range(0f, 1f) <= Zone6SpawnChance)
		{
			WorldStructure strcutureToSpawn = Zone6_Props[Random.Range(0, Zone6_Props.Count)];
			var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetPos3, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
			StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, targetPos3, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);
		}
		if (Random.Range(0f, 1f) <= Zone6SpawnChance)
		{
			WorldStructure strcutureToSpawn = Zone6_Props[Random.Range(0, Zone6_Props.Count)];
			var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetPos4, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
			StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, targetPos4, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);
		}
	}

	private void PlaceZone5(TerrainData terrainData, int mapResolution, int mapSize, GameObject parantObject, Vector2 spawnToSpawnVector, 
		float usedAngleForArty, float distanceUsedForArty)
	{
		if (Random.Range(0f, 1f) > Zone5SpawnChance) { return; }


		//split into 4 quarters.
		float randomAngle = Random.Range(usedAngleForArty+Zone5_MinAngleAddition, Zone5_MaxAngle);

		int quarterToUse = Random.Range(1, 4+1);
		float rotAngle = 0;
		//upper right
		if (quarterToUse == 1)
		{
			rotAngle = randomAngle;
		}
		//upper left
		else if (quarterToUse == 2)
		{
			rotAngle = -randomAngle;
		}
		//lower right
		else if (quarterToUse == 3)
		{
			rotAngle = randomAngle/2 + 90;
		}
		else if (quarterToUse == 4)
		{
			rotAngle = -90 - randomAngle/2;
		}
		Vector2 directionToSpawn = Rotate(spawnToSpawnVector, rotAngle);
		float distance = Random.Range(distanceUsedForArty + Zone5_MinDistanceAddition, maxArtyDistance + Zone5_MaxDistanceAddition);

		Vector2 targetLocation = spawnLocation + distance * directionToSpawn;

		WorldStructure strcutureToSpawn = Zone5_Props[Random.Range(0, Zone5_Props.Count)];

		var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetLocation, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
		StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, targetLocation, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);


		//CustomSingleStructurePlacer(strcutureToSpawn, targetLocation, mapResolution, terrainData, mapSize, parantObject);
	}

	private void PlaceZone4(TerrainData terrainData, int mapResolution, int mapSize, GameObject parantObject, 
		Vector2 spawnToSpawnVector, Vector2 rightPos, Vector2 leftPos)
	{
		//Spawn left
		Zone4Handle(Zone4_CloseMinDistance, Zone4_CloseMaxDistance, Zone4_FarMinDistance, Zone4_FarMaxDistance,
			leftPos, spawnToSpawnVector, Zone4_Props, mapResolution, terrainData, mapSize, parantObject);

		//Spawn right
		Zone4Handle(Zone4_CloseMinDistance, Zone4_CloseMaxDistance, Zone4_FarMinDistance, Zone4_FarMaxDistance,
			rightPos, spawnToSpawnVector, Zone4_Props, mapResolution, terrainData, mapSize, parantObject);
	}

	private void Zone4Handle(float closeMin, float closeMax, float farMin, float farMax, Vector2 basePoint, Vector2 spawnToSapwnVector, 
		List<WorldStructure> stuctureList, int mapResolution, TerrainData terrainData, int mapSize, GameObject parantObject)
	{
		//left object
		float distanceToUse;

		if (Random.Range(0f, 1f) > 0.5f)
		{
			//use near value
			distanceToUse = Random.Range(closeMin, closeMax);

		}
		else
		{
			//use far value
			distanceToUse = Random.Range(farMin, farMax);
		}
		Vector2 targetPos = basePoint - spawnToSapwnVector * distanceToUse;
		WorldStructure strcutureToSpawn = stuctureList[Random.Range(0, stuctureList.Count)];

		CustomSingleStructurePlacer(strcutureToSpawn, targetPos, mapResolution, terrainData, mapSize, parantObject);

		//StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, targetPos, mapResolution, terrainData, mapSize, parantObject.transform);
	}



	private void PlaceZone2(TerrainData terrainData, int mapResolution, int mapSize, GameObject parantObject, Vector2 spawnToSpawnVector)
	{
		if (Random.Range(0,1) > Zone2SpawnChance) { return; }

		float distanceMod = Random.Range(minZone2Distance, maxZone2Distance);
		Vector2 zone2Pos = spawnLocation - distanceMod * spawnToSpawnVector;

		WorldStructure strcutureToSpawn = Zone2_Props[Random.Range(0, Zone2_Props.Count)];

		var structureInfo = StructureSpawner.Instance.SpawnStructre(strcutureToSpawn, zone2Pos, mapResolution, terrainData, mapSize, parantObject.transform, ignoreSpawn);
		StructureSpawner.Instance.HandleSubStructures(strcutureToSpawn, zone2Pos, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);

	}

	private void CustomSingleStructurePlacer(WorldStructure structrureToSpawn, Vector2 workingPositive1, int mapResolution, TerrainData terrainData, int mapSize, GameObject parantObject)
	{
		WorldObject objectToSpawn = structrureToSpawn.primaryObjects[Random.Range(0, structrureToSpawn.primaryObjects.Count)];


		float spawnHeight = terrainData.GetInterpolatedHeight(workingPositive1.x / mapResolution, workingPositive1.y / mapResolution);
		Vector3 spawnPosition = new Vector3(workingPositive1.x, spawnHeight, workingPositive1.y);


		float lookAtHeight = terrainData.GetInterpolatedHeight(pointToFace.x / mapResolution, pointToFace.y / mapResolution);
		Vector3 lookAtPosition = new Vector3(pointToFace.x, lookAtHeight, pointToFace.y);

		float angle = Mathf.Atan2(pointToFace.y - spawnLocation.y, pointToFace.x - spawnLocation.x) * Mathf.Rad2Deg;

		GameObject holder = new GameObject(objectToSpawn.name);
		holder.transform.SetParent(parantObject.transform);

		objectToSpawn.CreateMeRotate(spawnPosition, holder.transform, "",terrainData,new Vector3(0,angle,0));

		StructureSpawner.CustomStructureReturnInfo structureInfo = new StructureSpawner.CustomStructureReturnInfo()
		{
			initialPoint = spawnPosition,
			typeDependantInfo = spawnPosition,
			parantObject = holder.transform
		};
		StructureSpawner.Instance.HandleSubStructures(structrureToSpawn, workingPositive1, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);

		//StructureSpawner.Instance.HandleSubStructures(structrureToSpawn, spawnLocation, mapResolution, terrainData, mapSize, structureInfo);
	}

	private void PlaceCenterStructure(TerrainData terrainData, int mapSize, int mapResolution, Transform parantTransform)
	{
		WorldStructure structure = centerStructures[Random.Range(0, centerStructures.Count)];

		StructureSpawner.CustomStructureReturnInfo structureInfo = StructureSpawner.Instance.SpawnStructre(structure, spawnLocation, mapResolution, terrainData, mapSize, parantTransform, ignoreSpawn);

		if (structureInfo.parantObject.childCount == 0)
		{
			//No strutures got placed. So lets delete the transform.
			Debug.Log("Would of destroyed: " + structureInfo.parantObject.gameObject);
			//DestroyImmediate(structureInfo.parantObject.gameObject);
			return;
		}
		StructureSpawner.Instance.HandleSubStructures(structure, spawnLocation, mapResolution, terrainData, mapSize, structureInfo, ignoreSpawn);
	}





	public static Vector2 DegreeToVector2(float degree)
	{
		return RadianToVector2(degree * Mathf.Deg2Rad);
	}

	public static Vector2 RadianToVector2(float radian)
	{
		return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
	}

	Vector2 Rotate(Vector2 aPoint, float aDegree)
	{
		return Quaternion.Euler(0, 0, aDegree) * aPoint;
	}
}
