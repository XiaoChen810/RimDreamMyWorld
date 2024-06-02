using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    /// <summary>
    /// ��ͼ���ݣ�������ͼ���֣���С�����ӣ��ڵ��б���Ӧʵ��
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