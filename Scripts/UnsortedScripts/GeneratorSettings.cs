using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Xml.Serialization;
using UnityEngine;


namespace eLF_RandomMaps
{


	[CreateAssetMenu(fileName = "Biome_", menuName = "Procedural Generation/CreateBiomeSettings", order = -1)]
	public class GeneratorSettings : ScriptableObject
	{
		private static float XSHIFT = 5.2f;
		private static float YSHIFT = 1.3f;
		private static float DOMAIN_WARP = 700;

		[Range(0, 8)]
		public int octaves = 2;
		[Range(0, 1000)]
		public float scale = 70;
		[Range(0, 1)]
		public float amplitudeMod = 0.3f;
		[Range(0, 5)]
		public float frequencyMod = 1.5f;
		[Range(0, 30)]
		public float maxHeightRep = 15;

		public NoiseType type = NoiseType.Standard;
		public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

		private float maxVal;
		private OpenSimplexNoise simplex;


		public void FakeConstructor(int o, float s, float a, float f, float mh, NoiseType t, long seed)
		{
			this.octaves = o;
			this.scale = s;
			this.amplitudeMod = a;
			this.frequencyMod = f;
			this.maxHeightRep = mh;
			this.type = t;

			Initialize(seed);
		}



		public void Initialize(long seed)
		{
			this.simplex = new OpenSimplexNoise(seed);
			this.maxVal = GetMaxVal();
		}

		private float GetMaxVal()
		{
			//Forms a geometric series, so using the sum formula.
			if (amplitudeMod == 1)
			{
				return octaves;
			}
			return (1 - Mathf.Pow(amplitudeMod, octaves)) / (1 - amplitudeMod);
		}


		public float GetDataPoint(float x, float y)
		{
			switch (type)
			{
				case NoiseType.Standard:
					return curve.Evaluate(StandardEval(x, y));
				case NoiseType.DomainWarped:
					return curve.Evaluate(DomainWarpedEval(x, y));
			}
			Debug.LogError("Did not find NoiseType: " + type);
			return 0;
		}


		private float DomainWarpedEval(float x, float y)
		{
			float xChange = StandardEvalNoNormal(x, y);
			float yChange = StandardEvalNoNormal(x + XSHIFT, y + YSHIFT);

			return StandardEval(x + DOMAIN_WARP * xChange, y + DOMAIN_WARP * yChange);
		}

		private float StandardEval(float x, float y)
		{
			return StandardEvalNoNormal(x, y) / maxVal;
		}

		private float StandardEvalNoNormal(float x, float y)
		{
			float amplitude = 1;
			float frequency = 1;
			float noiseHeight = 0;
			for (int i = 0; i < octaves; i++)
			{

				float xCord = (float)x / scale * frequency;
				float yCord = (float)y / scale * frequency;


				float perlinValue = (float)(simplex.Evaluate(xCord, yCord) + 1) / 2;
				noiseHeight += perlinValue * amplitude;

				amplitude *= amplitudeMod;
				frequency *= frequencyMod;
			}
			return noiseHeight;
		}

	}
}
