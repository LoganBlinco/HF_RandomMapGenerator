using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "WorldObject:", menuName = "TerrainGen/CreateObject")]
public class WorldObject : ScriptableObject
{
	[Range(0, 1)]
	public float spawnProbability = 1;
	[Range(0, 1)]
	public float spawnWeight = 1;

	public Vector3 maxAngle = new Vector3(0, 25, 0);
	public Vector3 heightOffset = new Vector3(0, -0.05f, 0);

	[SerializeField]
	public GameObject objectPrefab;

	public bool snapYRotation = true;

	[Tooltip("Random amount subtracted and added to object for scale placement")]
	public Vector3 randomScaleVariation = new Vector3(0, 0, 0);


	public Vector3 approxSize = new Vector3(0, 0, 0);
	public Vector3 pivotFix = new Vector3(0, 0, 0);

	[Range(0,359)]
	public float yAxisRotationRandomness = 359f;
}