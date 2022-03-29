using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{
    //https://github.com/GameDevEducation/AdvancedDeepDive_ProcGenTerrain/blob/Part-7-Improved-object-placement/Assets/Scripts/HeightMapModifiers/HeightMapModifier_Buildings.cs
    //Modified from that github page for Image based heightmap masking.
    public class HeightMap_BuildingSmoothing
    {
        public static float[,] Execute(HeightMap_ModifierData building, float[,] heightMap, Vector2 spawnLocation, float terrainDepth)
        {
            float averageHeight = 0f;
            int numHeightSamples = 0;

            int spawnX = (int)spawnLocation.x;
            int spawnY = (int)spawnLocation.y;

            int arraySize = heightMap.GetLength(0);

            // sum the height values under the building
            for (int y = -building.featureRadius; y <= building.featureRadius; ++y)
            {
                for (int x = -building.featureRadius; x <= building.featureRadius; ++x)
                {
                    // sum the heightmap values
                    int arrayX = x + spawnX;
                    int arrayY = y + spawnY;

                    if (ArrayIndexValid(arraySize,arrayX,arrayY))
                    {
                        averageHeight += heightMap[arrayX, arrayY];
                        ++numHeightSamples;
                    }
                }
            }

            // calculate the average height
            averageHeight /= numHeightSamples;

            float targetHeight = Mathf.Clamp01(averageHeight + (building.featureAdditionalHeight / terrainDepth));

            // apply the building heightmap
            for (int y = -building.featureRadius; y <= building.featureRadius; ++y)
            {
                int workingY = y + spawnY;
                float textureY = Mathf.Clamp01((float)(y + building.featureRadius) / (building.featureRadius * 2f));
                for (int x = -building.featureRadius; x <= building.featureRadius; ++x)
                {
                    int workingX = x + spawnX;
                    float textureX = Mathf.Clamp01((float)(x + building.featureRadius) / (building.featureRadius * 2f));

                    // sample the height map
                    var pixelColour = building.HeightMapMask.GetPixelBilinear(textureX, textureY);
                    float strength = pixelColour.r;

                    // blend based on strength

                    if (ArrayIndexValid(arraySize, workingX, workingY))
                    {
                        heightMap[workingX, workingY] = Mathf.Lerp(heightMap[workingX, workingY], targetHeight, strength);
                    }
                }
            }
            return heightMap;
        }

        public static bool ArrayIndexValid(int arraySize, int x, int y)
        {
            if (x < 0 || y < 0) { return false; }

            if (x >= arraySize || y >= arraySize) { return false; }

            return true;
        }
    }
}


