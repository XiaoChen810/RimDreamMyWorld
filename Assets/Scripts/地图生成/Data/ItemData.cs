using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// ��������,��ľ,���ӵ�
    /// </summary>
    [System.Serializable]
    public struct ItemData
    {
        public string name;
        [Range(0, 1f)] public float probability;
        public int number;
        public bool useNumber;
        public GameObject prefab;
        public MapNode.Type environment;
        public Vector3 offset;
        public int height;
        public int width;
        public int priority;
    }
}
