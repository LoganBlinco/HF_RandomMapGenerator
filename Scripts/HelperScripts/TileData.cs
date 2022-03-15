using UnityEngine;
using System.Collections;
using System;

//https://gist.github.com/sdomenici009/b774cada82c03950aed2


[System.Serializable]
public class TileData
{
    [System.Serializable]
    public struct rowData
    {
        public string[] row;
    }

    public rowData[] rows = new rowData[6];

    public TileData()
    {
        int size = rows.Length;
        for (int i =0;i<size;i++)
        {
            rows[i].row = new string[size];
        }
    }

    internal void init(string[,] biomeTable)
    {
        for (int i =0;i<biomeTable.GetLength(0);i++)
        {
            for (int j = 0; j < biomeTable.GetLength(1); j++)
            {
                rows[i].row[j] = biomeTable[i, j];
            }

        }
    }
}