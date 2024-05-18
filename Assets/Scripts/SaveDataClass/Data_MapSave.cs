using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_MapSave
{
    public Data_MapSave()
    {

    }

    public Data_MapSave(string mapName, int width, int height, int seed)
    {
        this.mapName = mapName;
        this.width = width;
        this.height = height;
        this.seed = seed;
    }
    // ����һ����ͼ��Ҫ������
    public string mapName;
    public int width, height;
    public int seed;
}
