using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    /// <summary>
    /// 地图数据，包括地图名字，大小，种子，节点列表，对应实例
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
            this.mapNodes = new MapNode[width,height];
        }
        public string mapName;
        public int width, height;
        public int seed;
        public MapNode[,] mapNodes;
        public GameObject mapObject;
    }
}