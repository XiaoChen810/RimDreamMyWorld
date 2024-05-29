using System;
using UnityEngine;

namespace ChenChen_Map
{
    /// <summary>
    /// 需要程序生成的物品数据
    /// </summary>
    [Serializable]
    public struct ThingData
    {
        public string name;

        public NodeType type;

        public int num;
    }
}
