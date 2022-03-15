using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMaths
{
    public static int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

	public static float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}


	public static float distance(float p1_x, float p1_y, float p2_x, float p2_y)
	{
		return distance_manhatten(p1_x, p1_y, p2_x, p2_y);
		//return distance_euclidian(p1_x, p1_y, p2_x, p2_y);
		//return distance_minkowski(p1_x, p1_y, p2_x, p2_y, 5);
	}

	private static float distance_manhatten(float p1_x, float p1_y, float p2_x, float p2_y)
	{
		return Mathf.Abs(p1_x - p2_x) + Mathf.Abs(p1_y - p2_y);
	}
	private static float distance_euclidian(float p1_x, float p1_y, float p2_x, float p2_y)
	{
		return Mathf.Sqrt(Mathf.Pow(p1_x - p2_x, 2) + Mathf.Pow(p1_y - p2_y, 2));
	}
	private static float distance_minkowski(float p1_x, float p1_y, float p2_x, float p2_y, int power)
	{
		return Mathf.Pow(Mathf.Pow(Mathf.Abs(p1_x - p2_x), power) + Mathf.Pow(Mathf.Abs(p1_y - p2_y), power), 1 / (float)power);
	}
}
