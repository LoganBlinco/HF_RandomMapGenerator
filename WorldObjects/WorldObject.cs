using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenuAttribute(fileName = "WorldObject:", menuName = "TerrainGen/CreateObject")]
public class WorldObject : ScriptableObject
{
	[Range(0, 1)]
	[SerializeField]
	private float spawnProbability = 1;
	[Range(0, 1)]
	public float spawnWeight = 1;

	[SerializeField]
	private Vector3 maxAngle = new Vector3(0, 25, 0);
	[SerializeField]
	private Vector3 heightOffset = new Vector3(0, -0.05f, 0);

	[SerializeField]
	private GameObject objectPrefab;

	[SerializeField]
	private bool snapYRotation = true;

	[Tooltip("Random amount subtracted and added to object for scale placement")]
	[SerializeField]
	private Vector3 randomScaleVariation = new Vector3(0, 0, 0);


	public Vector3 approxSize = new Vector3(0, 0, 0);
	[SerializeField]
	private Vector3 pivotFix = new Vector3(0, 0, 0);

	[Range(0,179)]
	[SerializeField]
	private float yAxisRotationRandomness = 179;

	//METHODS

	public void CreateMeSnapInternal(Vector3 targetPosition, Transform parantObject, string biomeName, TerrainData terrainData)
	{
		float angle = StructureSpawner.GetSteepnessValue(terrainData, targetPosition);
		if (!SpawnCheck(angle)) { return; }

		var spawnedObj = CreateObject(targetPosition, Quaternion.identity, parantObject);
		spawnedObj.name = biomeName + ": " + spawnedObj.name;
		if (snapYRotation)
		{
			spawnedObj.transform.up = StructureSpawner.GetNormalValue(terrainData, targetPosition);
		}
		spawnedObj.transform.Rotate(pivotFix);
		float randomAngle = Random.Range(-yAxisRotationRandomness, yAxisRotationRandomness);
		spawnedObj.transform.Rotate(new Vector3(0, randomAngle, 0));
	}

	public void CreateMeRotate(Vector3 targetPosition, Transform parantObject, string biomeName, TerrainData terrainData, Vector3 direction)
	{
		float angle = StructureSpawner.GetSteepnessValue(terrainData, targetPosition);
		if (!SpawnCheck(angle)) { return; }

		var spawnedObj = CreateObject(targetPosition, Quaternion.identity, parantObject);
		spawnedObj.name = biomeName + ": " + spawnedObj.name;
		if (snapYRotation)
		{
			spawnedObj.transform.up = StructureSpawner.GetNormalValue(terrainData, targetPosition);
		}
		spawnedObj.transform.Rotate(pivotFix);
		spawnedObj.transform.Rotate(direction);
		float randomAngle = Random.Range(-yAxisRotationRandomness, yAxisRotationRandomness);
		spawnedObj.transform.Rotate(new Vector3(0, randomAngle, 0));
	}

	public void CreateMeLookAt(Vector3 targetPos, Vector3 lookAngle, Transform parant, float angle)
	{
		if (!SpawnCheck(angle)) { return; }


		GameObject spawnedObj = CreateObject(targetPos, Quaternion.identity, parant);
		spawnedObj.transform.LookAt(lookAngle);
		spawnedObj.transform.Rotate(pivotFix);
	}

	private GameObject CreateObject(Vector3 targetPos, Quaternion rotation, Transform parant)
	{
		targetPos += heightOffset;
		GameObject spawnedObj = Instantiate(objectPrefab, targetPos, rotation, parant);
		return spawnedObj;
	}


	private bool SpawnCheck(float angle)
	{
		if (angle > maxAngle.y) { return false; }
		if (Random.Range(0f, 1f) > spawnProbability) { return false; }

		return true;
	}
}