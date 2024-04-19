using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// ��ͼ�ڵ���
    /// </summary>
    public class MapNode
    {
        public MapNode(Vector2Int postion, float noiseValue)
        {
            this.postion = postion;
            this.noiseValue = noiseValue;
        }

        // λ��
        public Vector2Int postion;
        // ����ֵ
        public float noiseValue;
        // ��Ƭ����
        public enum Type
        {
            none, grass, water, ground, mountain
        }
        public Type type = Type.none;
        // ��������Ƭ��ͼ
        public Tilemap loadingTilemap;
    }
}
