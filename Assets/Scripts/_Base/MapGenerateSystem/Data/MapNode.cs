using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// 地图节点类
    /// </summary>
    public class MapNode
    {
        public MapNode(Vector2Int postion, float noiseValue)
        {
            this.postion = postion;
            this.noiseValue = noiseValue;
        }

        // 位置
        public Vector2Int postion;
        // 噪声值
        public float noiseValue;
        // 瓦片类型
        public NodeType type = NodeType.none;
    }
}
