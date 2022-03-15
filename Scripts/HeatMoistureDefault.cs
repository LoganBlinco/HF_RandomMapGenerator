using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMoistureDefault
{
    public static MoistureValues get_moisture()
    {
        MoistureValues temp = new MoistureValues();
        temp.Dryest = 0f;
        temp.Dryer = 0.3f;
        temp.Dry = 0.4f;
        temp.Wet = 0.6f;
        temp.Wetter = 0.8f;
        temp.Wettest = 0.9f;
        return temp;
    }
    public static HeatValues get_heat()
    {
        HeatValues temp = new HeatValues();
        temp.Coldest = 0f;
        temp.Colder = 0.2f;
        temp.Cold = 0.35f;
        temp.Hot = 0.5f;
        temp.Hotter = 0.65f;
        temp.Hottest = 0.8f;
        return temp;
    }
}
