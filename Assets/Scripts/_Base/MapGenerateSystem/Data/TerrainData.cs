using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// 储存地形数据的结构体
    /// </summary>
    [System.Serializable]
    public struct TerrainData
    {
        [Range(0, 1f)]
        public float probability;

        public TileBase tile;

        public MapNode.Type type;

        public Tilemap loadingTilemap;
    }
}
