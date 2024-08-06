using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public enum BuildingType : byte
    {
        Architectural,  // 建筑构造
        Furniture,  // 家具
        Other,  // 其他
        Tree,   // 树
        Tool,  // 工具
        Light,  // 光
        Defend, // 防卫
    }
}