using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Generator : MonoBehaviour
{
	private static Generator _instance;
	public static Generator Instance
	{
		get
		{
			if (_instance == null)
			{ 
				_instance = FindObjectOfType<Generator>();
			}
			return _instance;
		}
	}


	public Tile[,] Tiles;
	private MeshRenderer HeightMapRenderer;
	private MeshRenderer HeatMapRenderer;
	private MeshRenderer MoistureMapRenderer;
	private MeshRenderer BiomeMapRenderer;
	private MeshRenderer FinalMapRenderer;
	private MeshRenderer TestMapRenderer;
	private MeshRenderer DetailMap1Renderer;
	private MeshRenderer DetailMap2Renderer;

	public int MapSize = 500;
	public MapSizes heightMapResolution = MapSizes.ExtraLarge_513;
	private int mapResolution;

	public static float OceanHeight = 0.3f;

	[Header("Heightmap data")]
	[Range(0, 40)]
	public static float TERRAIN_HEIGHT_LIMIT = 25;
	public float baseHeight = 10;
	
	[Space(10f)]
	[Header("Detail data -- flutuate loads")]
	[Range(0, 10)]
	//must be 1 octave for normalisation
	public int detailOctaves = 1;
	[Range(0, 10)]
	public float detailFrequency = 2f;
	[Range(0, 1000)]
	public float detailScale = 5;
	[Range(-2, 2)]
	public float detailAmplitudeMod = 0.5f;
	[Range(0, 5)]
	public float detailFrequencyMod = 2f;

	[Space(10f)]
	[Header("No Grass Detail")]
	[Range(0, 100)]
	public int noDetailScale = 5;
	private readonly int noDetailOctaves = 1;
	private readonly int noDetailAmplitudeMod = 1;
	private readonly int noDetailFreqMod = 1;


	[Header("Seed")]
	public long seed;
	private long seedOffset = 0;

	//details
	public int detailMaxDensity = 5;
	public int detailMinDensity = 3;

	public int debugX = 0;
	public int debugY = 0;

	[Header("Noise Settings for Generator")]
	public GeneratorSettings baseTerrainNoise;
	public GeneratorSettings moistureNoise;
	public GeneratorSettings heatNoise;
	public GeneratorSettings detailNoise;
	public GeneratorSettings detailNoise2;
	public GeneratorSettings detailRemoverNoise;

	private TerrainData terrainData;

	public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);

	public TileData userBiomeTable = new TileData();
	public static string[,] BiomeTable = new string[6,6];

	[Header("Biome Map Settings")]
	public bool loadSettings = false;
	public BiomeMapDefaults biomeImportSettings = BiomeMapDefaults.Default;





	public long GetSeedOffset()
	{
		seedOffset += 1;
		return seedOffset;
	}

	/*
	public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,            BiomeType.Grassland,             BiomeType.Savanna },             //WET
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.Rainforest,  BiomeType.Rainforest },  //WETTER
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.SeasonalForest, BiomeType.Rainforest,  BiomeType.Rainforest }   //WETTEST
};
*/


		/*

	//Winter settings
	public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Woodland,    BiomeType.Tundra,              BiomeType.Woodland,              BiomeType.Woodland },              //DRYEST
    { BiomeType.Tundra, BiomeType.Ice, BiomeType.Ice,    BiomeType.Woodland,              BiomeType.Ice,              BiomeType.Woodland },              //DRYER
    { BiomeType.Ice, BiomeType.Woodland, BiomeType.Ice,     BiomeType.Ice,            BiomeType.Woodland,             BiomeType.Tundra },             //DRY
    { BiomeType.Tundra, BiomeType.Tundra, BiomeType.Woodland, BiomeType.Woodland,            BiomeType.Tundra,             BiomeType.Woodland },             //WET
    { BiomeType.Ice, BiomeType.Ice, BiomeType.Woodland, BiomeType.Woodland,      BiomeType.Woodland,  BiomeType.Ice },  //WETTER
    { BiomeType.Tundra, BiomeType.Ice, BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,  BiomeType.Woodland }   //WETTEST
};
*/

	//public static Dictionary<BiomeType, int> biomeToIndex = new Dictionary<BiomeType, int>();
	
	//public Dictionary<BiomeType, BiomeObj> biomeDict;

	public void LoadDefaultValues()
	{

		HeatValues heat = HeatMoistureDefault.get_heat();
		BiomeInfomation.GenerateHeatDictionary(heat);


		MoistureValues moisture = HeatMoistureDefault.get_moisture();
		BiomeInfomation.GenerateMoistureDictionary(moisture);

		//biomeDict = new Dictionary<BiomeType, BiomeObj>();
		BiomeManager.Instance.GenerateBiomeDict(seed, terrainData);


		if (BiomeObjectSpawner.Instance == null)
		{
			Debug.Log("creating biomeobjectspawner");
			gameObject.AddComponent<BiomeObjectSpawner>();
		}

		if (StructureSpawner.Instance == null)
		{
			Debug.Log("creating strcuturespawner");
			gameObject.AddComponent<StructureSpawner>();
		}

		StructureSpawner.Instance.initValues(MapSize);

	}
	public void Run_RandomSeed()
	{
		seed = (long)UnityEngine.Random.Range(0, long.MaxValue);
		seedOffset = 0;
		Execute();
	}
	public void Run_SetSeed()
	{
		seedOffset = 0;
		Execute();
	}


	public void Execute()
	{
		if (loadSettings)
		{
			userBiomeTable.init(StaticValues.GetValue(biomeImportSettings));
			loadSettings = false;
		}


		for (int i =0;i<userBiomeTable.rows.Length;i++)
		{
			for (int j = 0; j < userBiomeTable.rows[0].row.Length; j++)
			{
				BiomeTable[i, j] = userBiomeTable.rows[i].row[j];
			}
		}

		Debug.Log("Main has been ran");
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();


		mapResolution = GetMapSize(heightMapResolution);

		Terrain currentTerrain = Terrain.activeTerrain;
		terrainData = currentTerrain.terrainData;
		//TerrainData terrainData = FindObjectOfType<Terrain>().terrainData;
		if (terrainData == null) { Debug.LogError("Terrain not found");return; }

		//GenerateDetailLayers(terrainData);

		//Set variables and instatiate -- uses mapsize and seed so must be after.
		LoadDefaultValues();


		float[,] finalH = new float[mapResolution, mapResolution];

		baseTerrainNoise.Initialize(seed);
		heatNoise.Initialize(seed + GetSeedOffset());
		moistureNoise.Initialize(seed + GetSeedOffset());
		Tiles = new Tile[mapResolution, mapResolution];
		for (int x = 0;x<mapResolution;x++)
		{
			for (int y = 0;y<mapResolution;y++)
			{
				Tile t = new Tile
				{
					X = x,
					Y = y,
					//HeightValue = baseHeight / TERRAIN_HEIGHT_LIMIT * GetDataPoint(x, y, scale, octaves, amplitudeMod, frequencyMod, heightSimplex, heightMax),
					HeightValue = baseHeight / TERRAIN_HEIGHT_LIMIT * baseTerrainNoise.GetDataPoint(x, y),
					HeatValue = heatNoise.GetDataPoint(x, y),
					MoistureValue = moistureNoise.GetDataPoint(x, y),
				};
				t.CalculateBiomeInfomation(BiomeTable);
				t.NewAddBiomesToHeight(BiomeManager.Instance.biomeMapping, TERRAIN_HEIGHT_LIMIT);
				t.HeightValue = animationCurve.Evaluate(t.HeightValue);
				finalH[x, y] = t.HeightValue;
				Tiles[x, y] = t;
			}
		}


		// Build our final objects based on our data
		finalH = HeightMap_Smoothing.Execute(Tiles, mapResolution, 0.9f);
		//finalH = HeightMap_Normalisation.Execute(finalH);
		//place objects for each biome.
		//place structures for each biome.



		terrainData.heightmapResolution = mapResolution;
		terrainData.alphamapResolution = mapResolution;
		terrainData.SetDetailResolution(mapResolution, 8);
		terrainData.size = new Vector3(MapSize, TERRAIN_HEIGHT_LIMIT, MapSize);
		terrainData.SetHeights(0,0, finalH);



		//Generate the Main objects
		BiomeObjectSpawner.Instance.SpawnObjectsForBiomes(mapResolution, Tiles, terrainData);

		StructureSpawner.Instance.SpawnStructuresForBiomes(mapResolution, Tiles, terrainData, MapSize);

		StructureSpawner.Instance.RunSpawnCreator(terrainData, MapSize, mapResolution);


		BiomeManager.Instance.init(terrainData);
		//GenerateLayers(terrainData);
		PaintMap(mapResolution,Tiles, terrainData);


		detailNoise.Initialize(seed + GetSeedOffset());
		detailNoise2.Initialize(seed + GetSeedOffset());

		// Get the mesh we are rendering our output to

		//HandleOutputTextures(finalH, detailMap, detailMap2);

		Dictionary<string, GeneratorSettings> altGrassNoDetailNoise = AltGenerateGrassNoDetailNoise(noDetailOctaves, noDetailScale, noDetailFreqMod, noDetailAmplitudeMod, seed);
		AltDetailMap(terrainData, Tiles, detailNoise, detailNoise2, altGrassNoDetailNoise);
		//DetailMap(terrainData, Tiles, detailNoise,detailNoise2,grassNoDetailNoise);



		currentTerrain.Flush();
		Debug.Log("finished");

		stopWatch.Stop();
		// Get the elapsed time as a TimeSpan value.
		TimeSpan ts = stopWatch.Elapsed;

		// Format and display the TimeSpan value.
		string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			ts.Hours, ts.Minutes, ts.Seconds,
			ts.Milliseconds / 10);
		Debug.Log("RunTime " + elapsedTime);

	}


	private Dictionary<string, GeneratorSettings> AltGenerateGrassNoDetailNoise(int noDetailOctaves, int noDetailScale, int noDetailFreqMod, int noDetailAmplitudeMod, long seed)
	{
		Dictionary<string, GeneratorSettings> final = new Dictionary<string, GeneratorSettings>();
		List<GrassConfigFile> types = BiomeManager.Instance.details;
		for (int i = 0; i < types.Count; i++)
		{
			GeneratorSettings temp = ScriptableObject.CreateInstance<GeneratorSettings>();
			temp.FakeConstructor(noDetailOctaves, noDetailScale, noDetailAmplitudeMod, noDetailFreqMod, 0, NoiseType.Standard, seed + GetSeedOffset());
			final[types[i].name] = temp;
		}
		return final;
	}
	#region Detail Section

	public void AltDetailMap(TerrainData terrainData, Tile[,] tiles, GeneratorSettings detail, GeneratorSettings detail2, Dictionary<string, GeneratorSettings> noGrassNoise)
	{
		//DETAIL MAPS ARE RETARDED. X AND Y POS ARE SWAPPED. @UNITTY WHYYYYYY????????

		float zoneGap = 1 / (float)detailMaxDensity;
		int numDetails = terrainData.detailPrototypes.Length;
		for (int i = 0; i < numDetails; i++)
		{
			int[,] map = new int[terrainData.detailWidth, terrainData.detailHeight];
			// For each pixel in the detail map...
			for (int y = 0; y < terrainData.detailHeight; y++)
			{
				for (int x = 0; x < terrainData.detailWidth; x++)
				{
					Tile tile = tiles[x, y];
					string close = tile.primaryBiomeType;
					string secondaryBiome = tile.SecondaryBiomeType;

					//List<grassInfomation> grassinfo = biomeDict[close].grassList;

					//as a biome becomes closer to another biome, it should become possible for other type of grass to appear?
					var grassDict = BiomeManager.Instance.biomeMapping[close].indexToDetail;
					BiomeGrassWrapper layerDetail;

					if (grassDict.TryGetValue(i, out layerDetail) && layerDetail.spawnWeight >= detail.GetDataPoint(x, y)
												&& (layerDetail.noGrassChance < noGrassNoise[layerDetail.config.name].GetDataPoint(x, y) || layerDetail.config.onlyUseSteepness)) 
					{
						if (tile.steepnessValue > layerDetail.config.maxGrassSpawnAngle) { continue; }

							//DetailMap2 then determines the density.

						for (int count = 1; count <= detailMaxDensity; count++)
						{
							if (detail2.GetDataPoint(x, y) <= count * zoneGap)
							{
								//this is where we use the flip
								map[x, y] = Mathf.Max(count, detailMinDensity);
								break;
							}
						}
					}
				}
			}
			terrainData.SetDetailLayer(0, 0, i, map);
		}
	}
	#endregion
	private void HandleOutputTextures(float[,] finalH, float[,] detailMap, float[,] detailMap2)
	{
		FindSceneTextures();
		ApplyTextures(finalH, detailMap, detailMap2);
	}

	
	private void ApplyTextures(float[,] finalH, float[,] detailMap, float[,] detailMap2)
	{
		// Render a texture representation of our map
		HeightMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(mapResolution, mapResolution, Tiles, TextureGenerator.TextureTypes.HeightMap);
		HeatMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(mapResolution, mapResolution, Tiles, TextureGenerator.TextureTypes.HeatMap);
		MoistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(mapResolution, mapResolution, Tiles, TextureGenerator.TextureTypes.MoistureMap);
		BiomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(mapResolution, mapResolution, Tiles);
		FinalMapRenderer.materials[0].mainTexture = TextureGenerator.GetFinalHMap(mapResolution, mapResolution, finalH);
		TestMapRenderer.materials[0].mainTexture = TextureGenerator.GetTextureColor(mapResolution, mapResolution, finalH);
		DetailMap1Renderer.materials[0].mainTexture = TextureGenerator.GetTextureColor(mapResolution, mapResolution, detailMap);
		DetailMap2Renderer.materials[0].mainTexture = TextureGenerator.GetTextureColor(mapResolution, mapResolution, detailMap2);
	}
	
	
	
	
	/// <summary>
	/// Finds the Quads that are used to display the textures eg biome/height map.
	/// </summary>
	private void FindSceneTextures()
	{
		string[] textureRenderers = new string[] { "HeightTexture", "HeatTexture", "MoistureTexture", "BiomeTexture", "FinalMapRenderer", "test","DetailMap1Renderer","DetailMap2Renderer" };
		foreach(string name in textureRenderers)
		{
			var tempHeight = transform.Find(name);
			if (tempHeight == null)
			{
				GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
				temp.name = name;
				temp.transform.SetParent(transform);
			}
		}

		HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
		HeatMapRenderer = transform.Find("HeatTexture").GetComponent<MeshRenderer>();
		MoistureMapRenderer = transform.Find("MoistureTexture").GetComponent<MeshRenderer>();
		BiomeMapRenderer = transform.Find("BiomeTexture").GetComponent<MeshRenderer>();
		FinalMapRenderer = transform.Find("FinalMapRenderer").GetComponent<MeshRenderer>();
		TestMapRenderer = transform.Find("test").GetComponent<MeshRenderer>();
		DetailMap1Renderer = transform.Find("DetailMap1Renderer").GetComponent<MeshRenderer>();
		DetailMap2Renderer = transform.Find("DetailMap2Renderer").GetComponent<MeshRenderer>();
	}


	public long GetNextSeed()
	{
		return seed + GetSeedOffset();
	}


	/// <summary>
	/// Returns map size from the enum.
	/// </summary>
	/// <param name="mapSize"></param>
	/// <returns></returns>
	private int GetMapSize(MapSizes mapSize)
	{
		return (int)mapSize;
	}


	#region Splat map region

	/// <summary>
	/// Paints the splat map of the terrain based on biome data
	/// Using inverse distance weighting for the values: https://en.wikipedia.org/wiki/Inverse_distance_weighting
	/// </summary>
	/// <param name="tempSize"></param>
	/// <param name="biomeMap"></param>
	/// <param name="terrainData"></param>
	private void PaintMap(int tempSize, Tile[,] biomeMap, TerrainData terrainData)
	{
		int counter = 25;
		float exponentPower = 1.3f;

		float[,,] splatMap = new float[tempSize, tempSize, terrainData.alphamapLayers];
		for (int x = 0; x < tempSize; x++)
		{
			for (int y = 0; y < tempSize; y++)
			{
				var d1 = biomeMap[x, y].primaryBiomeDistance;
				var d2 = biomeMap[x, y].secondaryBiomeDistance;
				var d3 = biomeMap[x, y].thirdBiomeDistance;
				d1 = 1f/Mathf.Pow(d1, exponentPower);
				d2 = 1f/Mathf.Pow(d2, exponentPower);
				d3 = 1f/Mathf.Pow(d3, exponentPower);

				var sum = d1 + d2 + d3;
				
				var index1 = BiomeManager.Instance.biomeMapping[biomeMap[x, y].primaryBiomeType].getTexInfoIndex();
				var index2 = BiomeManager.Instance.biomeMapping[biomeMap[x, y].SecondaryBiomeType].getTexInfoIndex();
				var index3 = BiomeManager.Instance.biomeMapping[biomeMap[x, y].ThirdBiomeType].getTexInfoIndex();




				int alternativeIndex = BiomeManager.Instance.biomeMapping[biomeMap[x, y].primaryBiomeType].getAltTexIndex();

				bool doSteepNessTest = false;
				if (alternativeIndex != -1 || doSteepNessTest)
				{
					int primaryIndex = index1;
					int secondaryIndex = alternativeIndex; //temp/
					float primaryWeight = d1 / (d1+d2+d3);
					//float primaryWeight = 1;
					SteepNessMapTest(x, y, splatMap, terrainData, primaryIndex, secondaryIndex,primaryWeight);
					splatMap[x, y, index2] += d2 / (d1 + d2+d3);
					splatMap[x, y, index3] += d3 / (d1 + d2 + d3);

					if (counter > 0)
					{
						/*
						Debug.Log("Valuess :" +
							"primary: " + splatMap[x, y, primaryIndex] + " " +
							"Alt: " + splatMap[x, y, secondaryIndex] + " " +
							"Secondary Biome: " + splatMap[x, y, index2]);
							*/
						counter--;
					}
				}
				else
				{
					splatMap[x, y, index1] += d1 / sum;
					splatMap[x, y, index2] += d2 / sum;
					splatMap[x, y, index3] += d3 / sum;
				}

			}
		}
		terrainData.SetAlphamaps(0, 0, splatMap);
	}
	#endregion

	int _counter = 15;
	private void SteepNessMapTest(int x,int y, float[,,] splatMap, TerrainData terrainData, int pIndex, int sIndex, float primaryW)
	{
		int mainIndex = pIndex;
		int rockIndex = sIndex;

		// Normalise x/y coordinates to range 0-1 
		//float y_01 = (float)y / (float)terrainData.alphamapHeight;
		//float x_01 = (float)x / (float)terrainData.alphamapWidth;

		float y_01 = (float)y / (float)terrainData.alphamapResolution;
		float x_01 = (float)x / (float)terrainData.alphamapResolution;

		// Calculate the steepness of the terrain
		float steepness = terrainData.GetSteepness(y_01, x_01);
		Tiles[x, y].steepnessValue = steepness;


		float fullSlopeAngle = 45f;
		float moderateSlopAngle = 35f; //https://geographyfieldwork.com/SlopeSteepnessIndex.htm
		float gentleScopeAngle = 15f;
		float noBlend = 0;

		float gradient = (1 - gentleScopeAngle * gentleScopeAngle / 90.0f) / (moderateSlopAngle - gentleScopeAngle);
		float blendMod = 0.5f;
		if (steepness < noBlend)
		{
			return;
		}
		else if (steepness < gentleScopeAngle)
		{
			float blend = steepness * gentleScopeAngle / 90.0f;
			blend *= blendMod;
			var value = (float)Math.Round(primaryW * blend, 2);
			splatMap[x, y, rockIndex] += value;
			splatMap[x, y, mainIndex] += primaryW - value;
		}
		else if (steepness >= gentleScopeAngle && steepness <= moderateSlopAngle)
		{
			float blend = gradient * (steepness - moderateSlopAngle) + 1;
			blend *= blendMod;
			var value = (float)Math.Round(primaryW * blend, 2);
			splatMap[x, y, rockIndex] += value;
			splatMap[x, y, mainIndex] += primaryW - value;
		}
		else if (steepness > moderateSlopAngle)
		{
			float delta = (1 - blendMod) / (fullSlopeAngle - moderateSlopAngle);
			float blend = delta * (steepness - fullSlopeAngle) + 1;
			blend = Mathf.Clamp01(blend);
			var value = (float)Math.Round(primaryW * blend, 2);
			splatMap[x, y, rockIndex] += value;
			splatMap[x, y, mainIndex] += primaryW - value;
		}

		if (mainIndex == rockIndex)
		{
			Debug.Log("same index");
		}

		if (splatMap[x, y, mainIndex] + splatMap[x, y, rockIndex] > 1 || _counter > 0)
		{
			Debug.Log("Weight: "+primaryW+" biger. Main: "+ splatMap[x, y, mainIndex]+", alternative: "+ splatMap[x, y, rockIndex]);
			_counter--;
		}

		//return splatMap;
	}



	/// <summary>
	/// Returns the value of X under the linear map:
	/// [domainMin,domainMax] -> [rangeMin,rangeMax]
	/// </summary>
	/// <param name="domainMin">Minimum input value</param>
	/// <param name="domainMax">Maximum input value</param>
	/// <param name="rangeMin">Minimum output value</param>
	/// <param name="rangeMax">Maximum output value</param>
	/// <param name="x">Value to be mapped</param>
	/// <returns>value x mapped from [domainMin,domainMax] -> [rangeMin,rangeMax]</returns>
	public static float GeneralLinearMap(float domainMin, float domainMax, float rangeMin, float rangeMax, float x)
	{
		float quotient = (rangeMax - rangeMin) / (domainMax - domainMin);
		float temp = rangeMin + (x - domainMin) * quotient;
		return temp;
	}


	public static float GetMaxVal(int octaves, float amplitudeMod)
	{
		//Forms a geometric series, so using the sum formula.
		if (amplitudeMod == 1)
		{
			return octaves;
		}
		return (1 - Mathf.Pow(amplitudeMod, octaves)) / (1 - amplitudeMod);
	}

}
