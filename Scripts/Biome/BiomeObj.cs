using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace eLF_RandomMaps
{
	[CreateAssetMenuAttribute(fileName = "Biome: ", menuName = "TerrainGen/Biomes")]
	public class BiomeObj : ScriptableObject
	{
		[Header("Biome Texture/Detail Infomation")]
		public List<Block> baseBiomeBlocks;
		public List<GrassConfigFile> baseDetails;

		private GeneratorSettings biomeNoise;

		private GeneratorSettings generateSettings;

		public Dictionary<int, BiomeGrassWrapper> indexToDetail = new Dictionary<int, BiomeGrassWrapper>();

		[Space(10f)]
		[Header("Biome Based Object Placement")]
		public List<WorldObject> middleDensityObjects;
		[Range(0, 100)]
		public float middleDensityObjectRadius = 20;
		private float middleDensityTotalWeight = 0;


		public List<WorldStructure> lowDensityObjects;
		public float lowDensityObjectRadius = 30;
		private float lowDensityTotalWeight = 0;





		///REGION FOR OBJECT PLACEMENT
		#region
		private void initObjectData()
		{
			middleDensityTotalWeight = CalculateObjectWeight(middleDensityObjects);
			lowDensityTotalWeight = CalculateStructureWeight(lowDensityObjects);

		}


		private float CalculateStructureWeight(List<WorldStructure> objects)
		{
			float total = 0;
			foreach (var o in objects)
			{
				total += o.structureSpawnWeight;
			}
			return total;
		}

		private float CalculateObjectWeight(List<WorldObject> objects)
		{
			float total = 0;
			foreach (var o in objects)
			{
				total += o.spawnWeight;
			}
			return total;
		}

		public WorldStructure GetStructureToSpawn()
		{
			if (lowDensityObjects.Count == 0) { Debug.Log(this + " has no structures"); return null; }
			float totalW = lowDensityTotalWeight;
			float randomVal = Random.Range(0f, 1f);//will need to seed.
			for (int i = 0; i < lowDensityObjects.Count; i++)
			{
				if (randomVal <= lowDensityObjects[i].structureSpawnWeight / totalW)
				{
					return lowDensityObjects[i];
				}
				else
				{
					randomVal -= lowDensityObjects[i].structureSpawnWeight / totalW;
				}
			}
			Debug.Log("problem finding weighted object for val: " + randomVal);
			return lowDensityObjects[lowDensityObjects.Count - 1];
		}


		public WorldObject GetObjectToSpawn()
		{
			//return middleDensityObjects[0];//TODO IMPLEMENT

			if (middleDensityObjects.Count == 0) { Debug.Log(this + " has no objects"); return null; }
			float totalW = middleDensityTotalWeight;
			float randomVal = Random.Range(0f, 1f);//will need to seed.
			for (int i = 0; i < middleDensityObjects.Count; i++)
			{
				if (randomVal <= middleDensityObjects[i].spawnWeight / totalW)
				{
					return middleDensityObjects[i];
				}
				else
				{
					randomVal -= middleDensityObjects[i].spawnWeight / totalW;
				}
			}
			Debug.Log("problem finding weighted object for val: " + randomVal);
			return middleDensityObjects[middleDensityObjects.Count - 1];
		}

		#endregion




		public List<GrassConfigFile> getBaseDetails()
		{
			return baseDetails;
		}




		public Block GetBiomeBlock(float noise)
		{
			float size = baseBiomeBlocks.Count;
			for (int i = 0; i < size; i++)
			{
				if (noise < i * 1.0f / (float)size)
				{
					return baseBiomeBlocks[i];
				}
			}
			//gone through all without finding one
			return baseBiomeBlocks[(int)size - 1];
		}


		public string getName()
		{
			return name;
		}

		public override string ToString()
		{
			string msg = string.Format("biomeName: {0}", getName());


			return msg;
		}




		/// NEW STUFF
		/// 

		public int getTexInfoIndex()
		{
			return baseBiomeBlocks[0].getTerrainLayerIndex();
			//return texInfo.index;
		}

		public int getAltTexIndex()
		{
			if (baseBiomeBlocks.Count > 1)
			{
				return baseBiomeBlocks[1].getTerrainLayerIndex();
			}
			return -1;
		}

		public void FakeConstructor(long seed)
		{
			indexToDetail = new Dictionary<int, BiomeGrassWrapper>();
			float totalW = 0;
			foreach (GrassConfigFile grassConfig in baseDetails)
			{
				totalW += grassConfig.grassWeight;
			}
			foreach (GrassConfigFile grassConfig in baseDetails)
			{
				var spawnWeight = grassConfig.grassWeight;
				if (spawnWeight != 1)
				{
					spawnWeight = spawnWeight / totalW;
				}
				indexToDetail[grassConfig.getDetailLayerIndex()] = new BiomeGrassWrapper(grassConfig, grassConfig.noGrassChance, spawnWeight);
			}


			generateSettings = Database_BiomeGenerationSettings.Instance.GetGenerationSettings(this.getName());
			generateSettings.Initialize(seed);


			//Object spawner
			initObjectData();
		}

		public GeneratorSettings getGeneratorSettings()
		{
			return generateSettings;
		}



	}
}
