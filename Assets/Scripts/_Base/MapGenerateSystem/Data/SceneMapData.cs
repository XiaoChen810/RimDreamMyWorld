using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// 场景地图数据
    /// </summary>
    [System.Serializable]
    public class SceneMapData
    {
        public SceneMapData()
        {
        }

        public SceneMapData(Data_MapSave save)
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
        public Tilemap mainTilemap;
        public List<Vector3> obstaclesPositionList = new List<Vector3>();
    }
}