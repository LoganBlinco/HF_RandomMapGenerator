using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From: https://github.com/GameDevEducation/AdvancedDeepDive_ProcGenTerrain/blob/master/Assets/Scripts/HeightMapModifiers/HeightMapModifier_Smooth.cs
public class HeightMap_Smoothing
{
    private static readonly int SmoothingKernelSize = 5;


	public static float[,] Execute(Tile[,] Tiles, int mapResolution,float Strength)
	{
        float[,] smoothedHeights = new float[mapResolution, mapResolution];

        for (int y = 0; y < mapResolution; ++y)
        {
            for (int x = 0; x < mapResolution; ++x)
            {
                float heightSum = 0f;
                int numValues = 0;

                // sum the neighbouring values
                for (int yDelta = -SmoothingKernelSize; yDelta <= SmoothingKernelSize; ++yDelta)
                {
                    int workingY = y + yDelta;
                    if (workingY < 0 || workingY >= mapResolution)
                        continue;

                    for (int xDelta = -SmoothingKernelSize; xDelta <= SmoothingKernelSize; ++xDelta)
                    {
                        int workingX = x + xDelta;
                        if (workingX < 0 || workingX >= mapResolution)
                            continue;

                        heightSum += Tiles[workingX, workingY].HeightValue;
                        ++numValues;
                    }
                }

                // store the smoothed (aka average) height
                smoothedHeights[x, y] = heightSum / numValues;
            }
        }

        float[,] final = new float[mapResolution, mapResolution];

        for (int y = 0; y < mapResolution; ++y)
        {
            for (int x = 0; x < mapResolution; ++x)
            {
                // blend based on strength
                Tiles[x, y].HeightValue = Mathf.Lerp(Tiles[x, y].HeightValue, smoothedHeights[x, y], Strength);
                final[x, y] = Tiles[x, y].HeightValue;
            }
        }
        return final;
    }

}
