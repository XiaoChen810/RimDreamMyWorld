using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class Def
    {
        public string label;
        public string name;
        public string description;
        public Sprite sprite; // 图标贴图
    }

    // 基础类：StuffDef
    public class StuffDef : Def
    {
        public int worth;   // 市场价值
        public int workload;    // 制作所需工作量
    }

    // 需求材料类
    [System.Serializable]
    public class Need
    {
        public string label; // 后续根据标签找到对应的材料
        public int numbers;

        public override string ToString()
        {
            return label + " " + numbers;
        }
    }

    #region - Body Def -

    public class BodyDef : Def
    {

    }

    public class HairDef : Def
    {

    }

    public class HeadDef : Def
    {

    }

    #endregion

    #region - Stuff Def -

    // 材料类
    public class MaterialDef : StuffDef
    {
        public float armorFactor = 1;  // 装甲系数
        public float warmthFactor = 1; // 保暖系数
        public float damageFactor = 1; // 伤害系数
    }

    // 衣物类
    public class ApparelDef : StuffDef
    {
        public List<Need> requiredMaterials; // 制作所需材料
    }

    // 武器类
    public class WeaponDef : StuffDef
    {
        public float meleeDamage;
        public float rangeDamage;
        public List<Need> requiredMaterials; // 制作所需材料
    }

    // 食物类
    public class FoodDef : StuffDef
    {
        public float nutritional;   // 提供的营养值
        public List<Need> requiredMaterials; // 制作所需材料
    }

    // 药品类
    public class MedicineDef : StuffDef
    {
        public float therapeutic;   // 疗效
        public List<Need> requiredMaterials; // 制作所需材料
    }

    #endregion
}
