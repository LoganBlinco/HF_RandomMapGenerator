using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{

	[CreateAssetMenu(fileName = "BiomeSettings_", menuName = "Procedural Generation/CreateBiomeSettingHolder", order = -1)]
	public class BiomeSettingHolder : ScriptableObject
	{
		//[Header("Important Base Terrain")]
		//public GeneratorSettings BaseTerrainSettings;
		[Space(10f)]
		[Header("Biome settings")]
		public GeneratorSettings BorealForest;
		public GeneratorSettings Desert;
		public GeneratorSettings GrassLand;
		public GeneratorSettings Ice;
		public GeneratorSettings RainForest;
		public GeneratorSettings Savanna;
		public GeneratorSettings SeasonalForest;
		public GeneratorSettings Tundra;
		public GeneratorSettings Woodland;
	}
}