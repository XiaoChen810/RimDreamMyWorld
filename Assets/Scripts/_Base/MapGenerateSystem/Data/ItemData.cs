using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    /// <summary>
    /// 物体数据,树木,房子等
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
