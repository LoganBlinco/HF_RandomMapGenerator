using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    private static BiomeManager _instance;
    public static BiomeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BiomeManager>();
                if (_instance == null) { Debug.LogError("Scene does not contain a BiomeManager"); }
            }
            return _instance;
        }
    }

    public Dictionary<string, BiomeObj> biomeMapping;



    public List<BiomeObj> biomes;

    public List<Block> blocks;

    public List<GrassConfigFile> details;


    public void init(TerrainData data)
    {
        blocks = new List<Block>();

        List<TerrainLayer> layers = new List<TerrainLayer>();

        int currentIndex = 0;

        foreach (BiomeObj b in biomes)
        {
            //add all blocks.
            foreach (Block block in b.baseBiomeBlocks)
            {
                if (!blocks.Contains(block))
                {
                    blocks.Add(block);
                    layers.Add(block.layer);
                    block.setTerrainLayerIndex(currentIndex);
                    currentIndex++;
                    BiomeType biometype = (BiomeType)System.Enum.Parse(typeof(BiomeType), b.getName());
                    //Generator.Instance.biomeDict[biometype].addBlock(block);
                }
            }
        }

        data.terrainLayers = layers.ToArray();
    }

    internal void GenerateBiomeDict(long seed, TerrainData terrainData)
    {
        details = new List<GrassConfigFile>();

        biomeMapping = new Dictionary<string, BiomeObj>();

        List<DetailPrototype> detailObjects = new List<DetailPrototype>();
        int currentDetailIndex = 0;


        foreach (BiomeObj bObj in biomes)
        {
            //Load details

            //Generate detail mapping
            foreach(GrassConfigFile grass in bObj.getBaseDetails())
            {
                if (details.Contains(grass)){ continue; }

                //not created this detail yet.
                DetailPrototype detailLayer = new DetailPrototype();
                detailLayer.prototypeTexture = grass.GRASS_TEXTURE;
                detailLayer.minWidth = grass.DETAIL_MIN_WIDTH;
                detailLayer.maxWidth = grass.DETAIL_MAX_WIDTH;
                detailLayer.minHeight = grass.DETAIL_MIN_HEIGHT;
                detailLayer.maxHeight = grass.DETAIL_MAX_HEIGHT;
                detailLayer.noiseSpread = grass.DETAIL_NOISE_SPREAD;
                detailLayer.healthyColor = grass.DETAIL_HEALTHY_COLOR;
                detailLayer.dryColor = grass.DETAIL_DRY_COLOR;
                detailLayer.renderMode = grass.DETAIl_RENDER_MODE;


                detailObjects.Add(detailLayer);
                grass.setDetailLayerIndex(currentDetailIndex);
                details.Add(grass);
                currentDetailIndex++;
            }


            //Initialize Biome Stuff
            bObj.FakeConstructor(seed);


            //Asign biome to mapping
            if (biomeMapping.ContainsKey(bObj.getName()))
            {
                Debug.LogError("Biome mapping already contains a biome of type: " + bObj.getName());
                continue;
            }
            biomeMapping[bObj.getName()] = bObj;
        }

        terrainData.detailPrototypes = detailObjects.ToArray();
    }
}
