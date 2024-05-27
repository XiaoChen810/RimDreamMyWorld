using System;
using UnityEngine;

namespace ChenChen_Map
{
    /// <summary>
    /// 需要程序生成的预制体数据
    /// </summary>
    [Serializable]
    public struct PrefabData
    {
        public string name;

        public NodeType type;

        public int num;
    }
}
