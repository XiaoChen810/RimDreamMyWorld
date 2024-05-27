using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Map
{
    /// <summary>
    /// 储存植物数据的结构体
    /// </summary>
    [System.Serializable]
    public struct FlowerData
    {
        [Range(0, 1f)]
        public float probability;

        public string tilemapName;

        public int layerSort;

        public List<TileBase> tile;

        public bool farFormOtherTile;

        public NodeType type;
    }
}