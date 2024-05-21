﻿using System;
using UnityEngine;

namespace ChenChen_MapGenerator
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

    /// <summary>
    /// 需要程序生成的特效数据
    /// </summary>
    [Serializable]
    public struct EffectData
    {
        public GameObject prefab;

        public int num;

        public int spacing;
    }
}
