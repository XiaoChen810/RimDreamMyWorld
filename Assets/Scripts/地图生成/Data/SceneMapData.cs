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
            this.obstaclesPositionList = new List<Vector3>();
        }

        public int width, height;
        public int seed;
        public MapNode[,] mapNodes;
        public FinderNode[,] finderNodes;
        public GameObject mapObject;
        public Tilemap mainTilemap;
        public List<Vector3> obstaclesPositionList;
    }
}