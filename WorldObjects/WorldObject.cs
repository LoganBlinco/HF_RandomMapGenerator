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

	[Range(0,359)]
	[SerializeField]
	private float yAxisRotationRandomness = 359f;


	public void CreateMeSnap(Vector3 targetPosition, Vector3 normal,Transform parantObject, string biomeName, float angle)
	{
		if (!spawnCheck(angle)) { return; }

		var spawnedObj = createObject(targetPosition, Quaternion.identity, parantObject);
		spawnedObj.name = biomeName + ": " + spawnedObj.name;
		if (snapYRotation)
		{
			spawnedObj.transform.up = normal;
		}
		spawnedObj.transform.Rotate(pivotFix);
		float randomAngle = Random.Range(0, yAxisRotationRandomness);
		spawnedObj.transform.Rotate(new Vector3(0, randomAngle, 0));
	}

	public void CreateMeLookAt(Vector3 targetPos, Vector3 lookAngle, Transform parant, float angle)
	{
		if (!spawnCheck(angle)) { return; }


		GameObject spawnedObj = createObject(targetPos, Quaternion.identity, parant);
		spawnedObj.transform.LookAt(lookAngle);
		spawnedObj.transform.Rotate(pivotFix);
	}

	private GameObject createObject(Vector3 targetPos, Quaternion rotation, Transform parant)
	{
		targetPos += heightOffset;
		GameObject spawnedObj = Instantiate(objectPrefab, targetPos, rotation, parant);
		return spawnedObj;
	}


	private bool spawnCheck(float angle)
	{
		if (angle > maxAngle.y) { return false; }
		if (Random.Range(0f, 1f) > spawnProbability) { return false; }

		return true;
	}
}