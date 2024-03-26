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

        public TileBase tile;

        public MapNode.Type type;

        public Tilemap loadingTilemap;
    }
}
