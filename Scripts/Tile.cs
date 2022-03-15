using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	public static MoistureType DEFAULT_MOISTURE_TYPE = MoistureType.Dryest;
	public static HeatType DEFAULT_HEAT_TYPE = HeatType.Coldest;
	public static string DEFAULT_BIOME_TYPE = "Ice";


	public float HeightValue;
	public float HeatValue;
	public float MoistureValue;
	public int X;
	public int Y;

	public float steepnessValue = 0;
	public bool isWater;


	public string primaryBiomeType;
	public string SecondaryBiomeType;
	public string ThirdBiomeType;
	public float primaryBiomeDistance;
	public float secondaryBiomeDistance;
	public float thirdBiomeDistance;

	//NEW MOISTURE/HEAT SYSTEM
	public HeatType[] NewHeatTypes = new HeatType[3];
	public MoistureType[] NewMoistureTypes = new MoistureType[3];

	/// <summary>
	/// Calculates Heat and Moisture data then biome data from that. [Helper method]
	/// </summary>
	/// <param name="BiomeTable">Biome Table mapping Heat and moisture values to biome type.</param>
	public void CalculateBiomeInfomation(string[,] BiomeTable)
	{
			CalculateClosestHeatTypes();
			CalculateClosestMoistureTypes();
			CalculateBiomeTypes(BiomeTable);
	}


	public void CalculateBiomeTypes(string[,] BiomeTable)
	{
		//distance function d(x,y)
		primaryBiomeType = getBiomeFromEnums(NewMoistureTypes[0], NewHeatTypes[0], BiomeTable);
		float primaryD = CustomMaths.distance(BiomeInfomation.MoistureToValue[NewMoistureTypes[0]], BiomeInfomation.HeatToValue[NewHeatTypes[0]], MoistureValue, HeatValue);

		SecondaryBiomeType  = DEFAULT_BIOME_TYPE;
		float secondaryD = float.MaxValue;
		ThirdBiomeType = DEFAULT_BIOME_TYPE;
		float thirdD = float.MaxValue;
		foreach(HeatType h in NewHeatTypes)
		{
			foreach(MoistureType m in NewMoistureTypes)
			{
				string currentBiome = getBiomeFromEnums(m, h, BiomeTable);
				float currentD = CustomMaths.distance(BiomeInfomation.MoistureToValue[m], BiomeInfomation.HeatToValue[h], MoistureValue, HeatValue);
				//if (currentBiome != primaryBiomeType)
				if (currentD != primaryD)
				{
					//float currentD = CustomMaths.distance(BiomeInfomation.MoistureToValue[m], BiomeInfomation.HeatToValue[h], MoistureData, HeatData);
					if (currentD < secondaryD)
					{
						if (currentBiome != SecondaryBiomeType)
						{
							thirdD = secondaryD;
							ThirdBiomeType = SecondaryBiomeType;
						}
						secondaryD = currentD;
						SecondaryBiomeType = currentBiome;
					}
					else if(currentD < thirdD)
					{
						thirdD = currentD;
						ThirdBiomeType = currentBiome;
					}
				}
			}
		}
		if (secondaryD == float.MaxValue)
		{
			secondaryD = primaryD;
			SecondaryBiomeType = primaryBiomeType;
		}
		if (thirdD == float.MaxValue)
		{
			thirdD = secondaryD;
			ThirdBiomeType = SecondaryBiomeType;
		}
		List<float> temp = new List<float>() { primaryD, secondaryD, thirdD };
		temp.Sort();
		primaryD = temp[0];
		secondaryD = temp[1];
		thirdD = temp[2];

		float sum = primaryD + secondaryD + thirdD;
		primaryBiomeDistance = primaryD / sum;
		secondaryBiomeDistance = secondaryD / sum;
		thirdBiomeDistance = thirdD / sum;
	}

	public string getBiomeFromEnums(MoistureType m, HeatType h, string[,] BiomeTable)
	{
		return BiomeTable[(int)m, (int)h]; ;
	}

	#region Moisture and Heat types
	public void CalculateClosestMoistureTypes()
	{
		int correctIndex = -1;

		MoistureType[] enumArray = (MoistureType[])System.Enum.GetValues(typeof(MoistureType));
		for (int i = 0; i < enumArray.Length; i++)
		{
			MoistureType type = enumArray[i];
			if (BiomeInfomation.MoistureToValue[type] <= MoistureValue)
			{
				correctIndex = i;
			}
			else
			{
				//The start value is bigger than the value so we can end.
				break;
			}
		}
		if (correctIndex == -1)
		{
			Debug.Log("Failed to find a correct biome Value: "+MoistureValue);
			return;
		}
		NewMoistureTypes[0] = enumArray[correctIndex];
		NewMoistureTypes[1] = enumArray[correctIndex];
		NewMoistureTypes[2] = enumArray[correctIndex];
		if (correctIndex + 1 < enumArray.Length)
		{
			NewMoistureTypes[1] = enumArray[correctIndex + 1];
		}
		if (correctIndex - 1 >= 0)
		{
			NewMoistureTypes[2] = enumArray[correctIndex - 1];
		}
	}




	public void CalculateClosestHeatTypes()
	{
		int correctIndex = -1;

		HeatType[] enumArray = (HeatType[])System.Enum.GetValues(typeof(HeatType));
		for (int i = 0; i < enumArray.Length; i++)
		{
			HeatType type = enumArray[i];
			if (BiomeInfomation.HeatToValue[type] <= HeatValue)
			{
				correctIndex = i;
			}
			else
			{
				//The start value is bigger than the value so we can end.
				break;
			}
		}
		if (correctIndex == -1)
		{
			Debug.Log("Failed to find a correct biome");
			return;
		}
		NewHeatTypes[0] = enumArray[correctIndex];
		NewHeatTypes[1] = enumArray[correctIndex];
		NewHeatTypes[2] = enumArray[correctIndex];
		if (correctIndex + 1 < enumArray.Length)
		{
			NewHeatTypes[1] = enumArray[correctIndex + 1];
		}
		if (correctIndex - 1 >= 0)
		{
			NewHeatTypes[2] = enumArray[correctIndex - 1];
		}
	}
	#endregion

	public Tile()
	{

	}

	/// <summary>
	/// Adds biome height interpolation to the heightmap. Ratio is done based on how close the biome is.
	/// </summary>
	/// <param name="biomeDict">BiomeType -> Biome [used for heightMap Data]</param>
	public void NewAddBiomesToHeight(Dictionary<string, BiomeObj> biomeDict, float maxHeight)
	{
		float exponentPower = 1.1f;

		//Generate heightmaps for the biomes.
		//For every point determine the amount needed to add.
		//add it as a percentage like the blending.
		//have some ratio values
		//apply smoothing.


		float baseHeight = HeightValue;
		string primaryBiome = primaryBiomeType;
		string secondaryBiome = SecondaryBiomeType;
		string thirdBiome = ThirdBiomeType;

		float d1 = 1f/Mathf.Pow(primaryBiomeDistance,exponentPower);
		float d2 = 1f /Mathf.Pow(secondaryBiomeDistance, exponentPower);
		float d3 = 1f /Mathf.Pow(thirdBiomeDistance, exponentPower);

		var sum = d1 + d2 + d3;


		float h1 = biomeDict[primaryBiome.ToString()].getGeneratorSettings().GetDataPoint(X, Y);
		float h2 = biomeDict[secondaryBiome.ToString()].getGeneratorSettings().GetDataPoint(X, Y);
		float h3 = biomeDict[thirdBiome.ToString()].getGeneratorSettings().GetDataPoint(X, Y);
		/*
		float h1 = biomeDict[primaryBiome].heightMap[X, Y];
		float h2 = biomeDict[secondaryBiome].heightMap[X,Y];
		float h3 = biomeDict[thirdBiome].heightMap[X,Y];
		*/

		float maxHeight1 = biomeDict[primaryBiome.ToString()].getGeneratorSettings().maxHeightRep;
		float maxHeight2 = biomeDict[secondaryBiome.ToString()].getGeneratorSettings().maxHeightRep;
		float maxHeight3 = biomeDict[thirdBiome.ToString()].getGeneratorSettings().maxHeightRep;

		float biomeChange = (d1 / sum * h1 * maxHeight1 + d2 / sum * h2 * maxHeight2 + d3 / sum * h3 * maxHeight3) / Generator.TERRAIN_HEIGHT_LIMIT;

		HeightValue = baseHeight + biomeChange;

	}

}
