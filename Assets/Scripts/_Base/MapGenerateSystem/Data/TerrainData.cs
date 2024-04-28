using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// ����������ݵĽṹ��
    /// </summary>
    [System.Serializable]
    public struct TerrainData
    {
        [Range(0, 1f)]
        public float probability;

        public string tilemapName;

        public int layerSort;

        public bool isObstacle;

        public TileBase tile;

        public NodeType type;
    }
}
