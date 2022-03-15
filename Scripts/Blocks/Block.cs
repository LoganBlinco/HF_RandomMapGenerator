using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New InteractableItem", menuName = "TerrainGen/Blocks")]
public class Block : ScriptableObject
{

	public TerrainLayer layer;
	//public Texture2D normal;

	private int terrainLayerIndex = -1;
	public string getName()
	{
		return layer.diffuseTexture.name;
	}

	public int getTerrainLayerIndex()
	{
		return terrainLayerIndex;
	}
	public void setTerrainLayerIndex(int index)
	{
		terrainLayerIndex = index;
	}
}
