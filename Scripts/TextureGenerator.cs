using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public enum TextureTypes
    {
        HeightMap,
        HeatMap,
        MoistureMap
    }

    private static Color DeepColor = new Color(0, 0, 0.5f, 1);
    private static Color ShallowColor = new Color(25 / 255f, 25 / 255f, 150 / 255f, 1);
    private static Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
    private static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
    private static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
    private static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
    private static Color SnowColor = new Color(1, 1, 1, 1);

    private static Color Ice = Color.white;
    private static Color Desert = new Color(200 / 255f, 170 / 255f, 41 / 255f, 1); //sand
    private static Color Savanna = new Color(209 / 255f, 163 / 255f, 110 / 255f, 1); //savanna
    private static Color Rainforest = new Color(0 / 255f, 117 / 255f, 94 / 255f, 1); //tropical rainforest
    private static Color Tundra = new Color(80 / 255f, 32 / 255f, 100 / 255f, 1); //tundra
    private static Color Grassland = new Color(0 / 255f, 154 / 255f, 23 / 255f, 1); //grassland
    private static Color SeasonalForest = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1); //seasonal forrest
    private static Color BorealForest = new Color(84 / 255f, 96 / 255f, 79 / 255f, 1); //boreal forest
    private static Color Woodland = new Color(102 / 255f, 107 / 255f, 59 / 255f, 1); //woodland


    public static Texture2D GetTexture(int width, int height, Tile[,] tiles, TextureTypes texType)
    {

        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        switch(texType)
        {
            case TextureTypes.HeightMap:
                pixels = usingHeightMap(width, height, tiles, pixels);
                break;
            case TextureTypes.HeatMap:
                pixels = usingHeatMap(width, height, tiles, pixels);
                break;
            case TextureTypes.MoistureMap:
                pixels = usingMoistureMap(width, height, tiles, pixels);
                break;
            default:
                Debug.Log("Invalid texType");
                break;
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
    private static Color[] usingHeatMap(int width, int height, Tile[,] tiles, Color[] pixels)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = Color.Lerp(Color.blue, Color.red, tiles[x, y].HeatValue);
            }
        }
        return pixels;
    }
    private static Color[] usingMoistureMap(int width, int height, Tile[,] tiles, Color[] pixels)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = Color.Lerp(Color.green, Color.magenta, tiles[x, y].MoistureValue);
            }
        }
        return pixels;
    }


    private static Color[] usingHeightMap(int width, int height, Tile[,] tiles, Color[] pixels)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                float value = tiles[x, y].HeightValue;

                if (value < 0.2f)
                {
                    pixels[x + y * width] = DeepColor;
                }
                else if (value < 0.4f)
                {
                    pixels[x + y * width] = ShallowColor;
                }
                else if (value < 0.5f)
                {
                    pixels[x + y * width] = SandColor;
                }
                else if (value < 0.7f)
                {
                    pixels[x + y * width] = GrassColor;
                }
                else if (value < 0.8f)
                {
                    pixels[x + y * width] = ForestColor;
                }
                else if (value < 0.9f)
                {
                    pixels[x + y * width] = RockColor;
                }
                else
                {
                    //Set color range, 0 = black, 1 = white
                    pixels[x + y * width] = SnowColor;
                }
            }
        }
        return pixels;
    }

    public static Texture2D GetBiomeMapTexture(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                string value = tiles[x, y].primaryBiomeType;
                pixels[x + y * width] = tiles[x, y].primaryBiomeDistance * GetBiomeColor(tiles[x, y].primaryBiomeType);
                pixels[x + y * width] += tiles[x, y].secondaryBiomeDistance * GetBiomeColor(tiles[x, y].SecondaryBiomeType);
                pixels[x+y*width] += tiles[x,y].thirdBiomeDistance * GetBiomeColor(tiles[x, y].ThirdBiomeType);
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Color GetBiomeColor(string biome)
    {
        switch (biome)
        {
            case "Ice":
                return Ice;
            case "BorealForest":
                return BorealForest;
            case "Desert":
                return Desert;
            case "Grassland":
                return Grassland;
            case "SeasonalForest":
                return SeasonalForest;
            case "Tundra":
                return Tundra;
            case "Savanna":
                return Savanna;
            case "Rainforest":
                return Rainforest;
            case "Woodland":
                return Woodland;
            default:
                Debug.Log("error");
                return Ice;
        }
    }

    internal static Texture GetFinalHMap(int width, int height, float[,] finalH)
    {

        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                float value = finalH[x, y];

                if (value < 0.2f)
                {
                    pixels[x + y * width] = DeepColor;
                }
                else if (value < 0.4f)
                {
                    pixels[x + y * width] = ShallowColor;
                }
                else if (value < 0.5f)
                {
                    pixels[x + y * width] = SandColor;
                }
                else if (value < 0.7f)
                {
                    pixels[x + y * width] = GrassColor;
                }
                else if (value < 0.8f)
                {
                    pixels[x + y * width] = ForestColor;
                }
                else if (value < 0.9f)
                {
                    pixels[x + y * width] = RockColor;
                }
                else
                {
                    //Set color range, 0 = black, 1 = white
                    pixels[x + y * width] = SnowColor;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

	internal static Texture GetTextureColor(int width, int height, float[,] finalH)
	{
        int sizeRemoved = 96;

        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = Color.Lerp(Color.blue, Color.red, finalH[x, y]);
                if (x == sizeRemoved || x == 512- sizeRemoved || y == sizeRemoved || y == 512- sizeRemoved)
                {
                    pixels[x + y * width] = Color.white;
                }
            }
        }
        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
