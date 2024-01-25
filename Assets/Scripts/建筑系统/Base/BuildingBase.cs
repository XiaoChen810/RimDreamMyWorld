using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 建筑系统
{
    [System.Serializable]
    public abstract class BuildingBase
    {
        // 建筑物名字
        public string Name;

        // 建筑物耐久
        public float Durability;
    }
}