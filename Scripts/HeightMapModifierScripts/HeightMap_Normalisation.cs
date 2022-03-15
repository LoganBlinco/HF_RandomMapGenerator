using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMap_Normalisation
{
	public static float[,] Execute(float[,] heightMap)
	{
		int sizeX = heightMap.GetLength(0);
		int sizeY = heightMap.GetLength(1);

		float minVal = heightMap[0, 0];
		float maxVal = heightMap[0, 0];
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				float val = heightMap[x, y];
				if (val < minVal)
				{
					minVal = val;
				}
				else if (val > maxVal)
				{
					maxVal = val;
				}
			}
		}

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				heightMap[x, y] = maxVal / (maxVal - minVal) * (heightMap[x, y] - minVal);
			}
		}
		return heightMap;
	}

}
