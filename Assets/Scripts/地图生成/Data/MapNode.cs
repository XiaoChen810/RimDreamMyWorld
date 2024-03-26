using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// 地图节点类
    /// </summary>
    public class MapNode
    {
        public MapNode(int x, int y, float noiseValue)
        {
            this.x = x; this.y = y;
            this.noiseValue = noiseValue;
        }

        // 位置
        public int x;
        public int y;
        // 噪声值
        public float noiseValue;
        // 瓦片类型
        public enum Type
        {
            none, grass, water, ground, mountain
        }
        public Type type = Type.none;
        // 附属的瓦片地图
        public Tilemap loadingTilemap;
        // 是否有障碍物
        // public bool noObstacles;
    }
}
