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
        public MapNode(int x, int y, float noiseValue)
        {
            this.x = x; this.y = y;
            this.noiseValue = noiseValue;
        }

        // λ��
        public int x;
        public int y;
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
        // �Ƿ����ϰ���
        // public bool noObstacles;
    }
}
