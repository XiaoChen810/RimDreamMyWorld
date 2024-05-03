using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// ������ͼ����
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        public MapData()
        {
        }

        public MapData(Data_MapSave save)
        {
            this.mapName = save.mapName;
            this.width = save.width;
            this.height = save.height;
            this.seed = save.seed;
        }
        public string mapName;
        public int width, height;
        public int seed;
        public MapNode[,] mapNodes;
        public GameObject mapObject;
    }
}