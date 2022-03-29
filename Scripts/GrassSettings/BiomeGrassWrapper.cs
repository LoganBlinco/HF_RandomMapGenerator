using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{
	public struct BiomeGrassWrapper
	{
		public GrassConfigFile config;
		public float noGrassChance;
		public float spawnWeight;

		public BiomeGrassWrapper(GrassConfigFile c, float noGrass, float w)
		{
			this.config = c;
			this.noGrassChance = noGrass;
			this.spawnWeight = w;
		}
	}

}