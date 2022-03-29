using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace eLF_RandomMaps
{
    public class BiomeInfomation
    {
        public static Dictionary<MoistureType, float> MoistureToValue = new Dictionary<MoistureType, float>();
        public static Dictionary<HeatType, float> HeatToValue = new Dictionary<HeatType, float>();

        public static MoistureType DEFAULT_MOISTURE = MoistureType.Wet;
        public static HeatType DEFAULT_HEAT = HeatType.Warm;


        public static void GenerateMoistureDictionary(MoistureValues moist)
        {
            MoistureToValue = new Dictionary<MoistureType, float>();
            MoistureToValue.Add(MoistureType.Dryest, moist.Dryest);
            MoistureToValue.Add(MoistureType.Dryer, moist.Dryer);
            MoistureToValue.Add(MoistureType.Dry, moist.Dry);
            MoistureToValue.Add(MoistureType.Wet, moist.Wet);
            MoistureToValue.Add(MoistureType.Wetter, moist.Wetter);
            MoistureToValue.Add(MoistureType.Wettest, moist.Wettest);
        }

        public static void GenerateHeatDictionary(HeatValues heat)
        {
            HeatToValue = new Dictionary<HeatType, float>();
            HeatToValue.Add(HeatType.Coldest, heat.Coldest);
            HeatToValue.Add(HeatType.Colder, heat.Colder);
            HeatToValue.Add(HeatType.Cold, heat.Cold);
            HeatToValue.Add(HeatType.Warm, heat.Hot);
            HeatToValue.Add(HeatType.Warmer, heat.Hotter);
            HeatToValue.Add(HeatType.Warmest, heat.Hottest);
        }

        public static string getBiome(float moisture, float heat)
        {
            HeatType heatEnum = GetHeatEnum(heat);
            MoistureType moistureEnum = GetMoistureEnum(moisture);
            return Generator.BiomeTable[(int)moistureEnum, (int)heatEnum];
        }


        public static HeatType GetHeatEnum(float value)
        {
            HeatType[] enums = (HeatType[])System.Enum.GetValues(typeof(HeatType));
            for (int i = 0; i < enums.Length; i++)
            {
                if (i == enums.Length - 1)
                {
                    //none of the previous are valid , thus must be the final one
                    return enums[i];
                }
                if (value < HeatToValue[enums[i + 1]])
                {
                    return enums[i];
                }
            }
            Debug.LogWarning("Invalid tempeture found");
            return DEFAULT_HEAT;
        }

        public static MoistureType GetMoistureEnum(float value)
        {
            MoistureType[] enums = (MoistureType[])System.Enum.GetValues(typeof(MoistureType));
            for (int i = 0; i < enums.Length; i++)
            {
                if (i == enums.Length - 1)
                {
                    //none of the previous are valid , thus must be the final one
                    return enums[i];
                }
                if (value < MoistureToValue[enums[i + 1]])
                {
                    return enums[i];
                }
            }
            Debug.LogWarning("Invalid Moisture found");
            return DEFAULT_MOISTURE;
        }
    }

}