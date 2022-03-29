using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{
	[System.Serializable]
	public class HeightMap_ModifierData
	{
		[SerializeField]
		public int featureRadius;
		[SerializeField]
		public Texture2D HeightMapMask;
		[SerializeField]
		[Tooltip("This is the height which is added onto the average height to calculate the target height.")]
		public float featureAdditionalHeight = 0;

	}
}

