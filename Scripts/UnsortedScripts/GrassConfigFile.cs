using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{
    [CreateAssetMenu(fileName = "GrassConfig", menuName = "Procedural Generation/CreateGrass", order = -1)]


    public class GrassConfigFile : ScriptableObject
    {
        public float DETAIL_MIN_WIDTH = 1;
        public float DETAIL_MAX_WIDTH = 1.2f;
        public float DETAIL_MIN_HEIGHT = 0.4f;
        public float DETAIL_MAX_HEIGHT = 0.5f;
        public float DETAIL_NOISE_SPREAD = 10;
        public Color DETAIL_HEALTHY_COLOR = new Color(59f / 255, 115f / 255, 27f / 255);
        public Color DETAIL_DRY_COLOR = new Color(107f / 255, 209f / 255, 76f / 255);
        //public DetailRenderMode DETAIl_RENDER_MODE = DetailRenderMode.Grass;

        public Texture2D GRASS_TEXTURE;

        [Range(0, 1)]
        public float grassWeight;
        [Range(0, 1)]
        public float noGrassChance;
        [Range(0, 90)]
        public float maxGrassSpawnAngle = 13;
        public bool onlyUseSteepness = false;

        private int detailIndex;

        public int getDetailLayerIndex()
        {
            return detailIndex;
        }
        public void setDetailLayerIndex(int index)
        {
            detailIndex = index;
        }
    }
}
