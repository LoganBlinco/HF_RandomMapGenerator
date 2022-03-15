using UnityEngine;

public enum BiomeMapDefaults
{
    Default,
    GrassBased,
    DesertBased,
    SnowBased
}


public static class StaticValues
{
    public static string[,] GetValue(BiomeMapDefaults e)
    {
        switch(e)
        {
            case BiomeMapDefaults.Default:
                return Default;
            case BiomeMapDefaults.GrassBased:
                return GrassBased;
            case BiomeMapDefaults.DesertBased:
                return DesertBased;
            case BiomeMapDefaults.SnowBased:
                return SnowBased;
            default:
                Debug.LogError("Unable to find variable for enum: " + e.ToString());
                return Default;
        }
    }



    private static string[,] Default = new string[6, 6] {   
        //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { "Ice", "Tundra", "BorealForest",    "Savanna",              "Desert",              "Desert" },              //DRYEST
        { "Ice", "Tundra", "Woodland",    "Grassland",              "Savanna",              "Desert" },              //DRYER
        { "Ice", "Tundra", "Woodland",     "Grassland",            "Savanna",             "Savanna"},             //DRY
        { "Ice", "Tundra", "Woodland", "Grassland",            "Grassland",             "Savanna" },             //WET
        { "Tundra", "BorealForest", "BorealForest", "SeasonalForest",      "SeasonalForest",  "Rainforest" },  //WETTER
        { "Tundra", "Tundra", "BorealForest", "SeasonalForest", "Rainforest",  "Rainforest" }   //WETTEST
    };

    //grass settings

    private static string[,] GrassBased = new string[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { "Woodland", "Woodland", "Grassland",    "Grassland",              "Grassland",              "SeasonalForest" },              //DRYEST
        { "Woodland", "Woodland", "Grassland",    "Grassland",              "Grassland",              "SeasonalForest" },              //DRYER
        { "Woodland", "Grassland", "Woodland",     "SeasonalForest",            "SeasonalForest",             "SeasonalForest" },             //DRY
        { "Woodland", "Grassland", "Woodland", "Grassland",            "SeasonalForest",             "Rainforest" },             //WET
        { "Woodland", "Woodland", "Grassland", "Grassland",      "SeasonalForest",  "Rainforest" },  //WETTER
        { "Woodland", "Woodland", "Grassland", "Grassland", "Rainforest",  "Rainforest" }   //WETTEST
    };
    //deserrt settings

    public static string[,] DesertBased = new string[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { "Woodland", "Savanna", "Savanna",    "Desert",              "Desert",              "Desert" },              //DRYEST
        { "Woodland", "Desert", "Savanna",    "Desert",              "Savanna",              "Desert" },              //DRYER
        { "Woodland", "Savanna", "Desert",     "Savanna",            "Savanna",             "Desert" },             //DRY
        { "Savanna", "Savanna", "Desert", "Desert",            "Desert",             "Savanna" },             //WET
        { "Woodland", "Woodland", "Savanna", "Desert",      "Desert",  "Savanna" },  //WETTER
        { "Woodland", "Woodland", "Savanna", "Desert", "Desert",  "Desert" }   //WETTEST
    };


//Winter settings
    public static string[,] SnowBased = new string[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
        { "Ice", "Ice", "Ice",    "Woodland",              "Woodland",              "Woodland" },              //DRYEST
        { "Ice", "Ice", "Woodland",    "Woodland",              "Woodland",              "Woodland" },              //DRYER
        { "Ice", "Woodland", "Woodland",     "Woodland",            "Woodland",             "Tundra" },             //DRY
        { "Ice", "Ice", "Tundra", "Tundra",            "Woodland",             "Tundra" },             //WET
        { "Ice", "Ice", "Tundra", "Tundra",      "Tundra",  "Tundra" },  //WETTER
        { "Ice", "Ice", "Ice", "Tundra", "Tundra",  "Tundra" }   //WETTEST
    };

    //grass settings

    /*
	public static string[,] BiomeTable = new string[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { "Woodland", "Woodland", "Grassland",    "Grassland",              "Grassland",              "SeasonalForest" },              //DRYEST
    { "Woodland", "Woodland", "Grassland",    "Grassland",              "Grassland",              "SeasonalForest" },              //DRYER
    { "Woodland", "Grassland", "Woodland",     "SeasonalForest",            "SeasonalForest",             "SeasonalForest" },             //DRY
    { "Woodland", "Grassland", "Woodland", "Grassland",            "SeasonalForest",             "Rainforest" },             //WET
    { "Woodland", "Woodland", "Grassland", "Grassland",      "SeasonalForest",  "Rainforest" },  //WETTER
    { "Woodland", "Woodland", "Grassland", "Grassland", "Rainforest",  "Rainforest" }   //WETTEST
};
*/
    //deserrt settings

    /*
public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
{ BiomeType.Woodland, BiomeType.Savanna, BiomeType.Savanna,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
{ BiomeType.Woodland, BiomeType.Desert, BiomeType.Savanna,    BiomeType.Desert,              BiomeType.Savanna,              BiomeType.Desert },              //DRYER
{ BiomeType.Woodland, BiomeType.Savanna, BiomeType.Desert,     BiomeType.Savanna,            BiomeType.Savanna,             BiomeType.Desert },             //DRY
{ BiomeType.Savanna, BiomeType.Savanna, BiomeType.Desert, BiomeType.Desert,            BiomeType.Desert,             BiomeType.Savanna },             //WET
{ BiomeType.Woodland, BiomeType.Woodland, BiomeType.Savanna, BiomeType.Desert,      BiomeType.Desert,  BiomeType.Savanna },  //WETTER
{ BiomeType.Woodland, BiomeType.Woodland, BiomeType.Savanna, BiomeType.Desert, BiomeType.Desert,  BiomeType.Desert }   //WETTEST
};
*/


}
