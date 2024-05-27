using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    /// <summary>
    /// ��ͼ�ڵ���
    /// </summary>
    public class MapNode
    {
        public MapNode(Vector2Int postion, float noiseValue)
        {
            this.position = postion;
            this.noiseValue = noiseValue;
        }

        // λ��
        public Vector2Int position;
        // ����ֵ
        public float noiseValue;
        // ��Ƭ����
        public NodeType type = NodeType.none;
    }
}
