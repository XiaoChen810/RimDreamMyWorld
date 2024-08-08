using ChenChen_Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    /// <summary>
    /// 物品定义
    /// </summary>
    [CreateAssetMenu(menuName = "定义/物体", fileName = "ThingDef", order = 0)]
    public class BuildingDef : ScriptableObject
    {
        [Header("必须属性")]
        public string DefName;
        public BuildingType Type;
        public string Description;

        [Tooltip("建造所需工作量")]
        public int Workload;

        [Tooltip("耐久度")]
        public int Durability;

        [Tooltip("预览图")]
        public Sprite PreviewSprite;

        [Tooltip("最后生成的建筑的预制体")]
        public GameObject Prefab;

        [Tooltip("生成位置偏移量")]
        public Vector2 Offset = new Vector2(0.5f, 0.5f);

        [Tooltip("建造所需材料")]
        public List<Need> RequiredMaterials = new List<Need>();

        [Tooltip("建筑尺寸")]
        public Vector2Int Size = Vector2Int.one;

        //----------------------------------------------------------------------
        [Header("可选属性")]

        [Tooltip("最后生成的瓦片")]
        public TileBase TileBase;

        [Tooltip("是否是障碍物")]
        public bool IsObstacle;

        [Tooltip("是否影响建造")]
        public bool IsEffectBuild = false;

    }
}