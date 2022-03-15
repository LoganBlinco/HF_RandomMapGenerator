using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database_BiomeGenerationSettings : MonoBehaviour
{
	
	private static Database_BiomeGenerationSettings _instance;
	public static Database_BiomeGenerationSettings Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Database_BiomeGenerationSettings>();
			}
			return _instance;
		}
	}

	public BiomeSettingHolder BiomeSettings;

	public GeneratorSettings GetGenerationSettings(string b)
	{
		switch (b)
		{
			case "BorealForest":
				return BiomeSettings.BorealForest;
			case "Desert":
				return BiomeSettings.Desert;
			case "Grassland":
				return BiomeSettings.GrassLand;
			case "Ice":
				return BiomeSettings.Ice;
			case "Rainforest":
				return BiomeSettings.RainForest;
			case "Savanna":
				return BiomeSettings.Savanna;
			case "SeasonalForest":
				return BiomeSettings.SeasonalForest;
			case "Tundra":
				return BiomeSettings.Tundra;
			case "Woodland":
				return BiomeSettings.Woodland;
			default:
				Debug.Log("Error finding Biome Generation Settings of type: " + b);
				return BiomeSettings.GrassLand;
		}
	}
}
