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
        public Sprite sprite = null; // 图标贴图
        public Color color = Color.white;
    }

    // 基础类：StuffDef
    public class StuffDef : Def
    {
        public int worth = 0;   // 市场价值
        public int workload = 0;    // 制作所需工作量
        public Vector2Int size = Vector2Int.one;

        public bool canMade = true;
        public List<Need> requiredMaterials = new List<Need>(); // 制作所需材料
    }

    // 需求材料类
    [System.Serializable]
    public class Need
    {
        public string label; // 后续根据标签找到对应的材料
        public string name => XmlLoader.Instance.GetDef(label).name;

        public int numbers;

        public override string ToString()
        {
            return name + " " + numbers;
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
        public string stuffCategories; // 类别
        public float armorFactor = 1;  // 装甲系数
        public float warmthFactor = 1; // 保暖系数
        public float damageFactor = 1; // 伤害系数
    }

    // 衣物类
    public class ApparelDef : StuffDef
    {
        public List<string> bodyPartGroups;  // 装备在身体那个部位
    }

    // 武器类
    public class WeaponDef : StuffDef
    {
        public bool isMelee;

        public float meleeDamage;           // 近距离伤害
        public float rangeDamage;           // 远距离伤害

        public float equippedAngleOffset;   // 装备时偏移量

        public float range;                 // 射击范围
        public float warmupTime;            // 准备时间
        public float rangedWeaponCooldown;  // 冷却时间

        public float accuracy = 1;              // 射击精度

        public static readonly WeaponDef Fist = new WeaponDef
        {
            label = "Fist",
            name = "拳头",
            isMelee = true,
            meleeDamage = 1.0f,     // 定义拳头的近距离伤害
            rangeDamage = 0.0f,     // 拳头没有远距离伤害
            equippedAngleOffset = 0.0f,
            range = 1.0f,           // 拳头的攻击范围很近
            warmupTime = 0.5f,      // 拳头的准备时间
            rangedWeaponCooldown = 0.5f,
            accuracy = 1.0f
        };
    }

    // 食物类
    public class FoodDef : StuffDef
    {
        public float nutritional;   // 提供的营养值
    }

    // 药品类
    public class MedicineDef : StuffDef
    {
        public float therapeutic;   // 疗效
    }

    #endregion
}
