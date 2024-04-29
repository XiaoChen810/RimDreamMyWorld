using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public enum ThingType
    {
        Floor,Wall,Building,Furniture,Other,Tree
    }

    /// <summary>
    /// 物品定义
    /// </summary>
    [CreateAssetMenu(menuName = "蓝图数据", fileName = "蓝图数据", order = 0)]
    public class ThingDef : ScriptableObject
    {
        public string Name;

        [Header("类型")]
        public ThingType Type;

        [Header("建造所需工作量")]
        public int Workload;

        [Header("预览图")]
        public Sprite PreviewSprite;

        [Header("是否是障碍物")]
        public bool IsObstacle;

        [Header("最后生成的建筑的预制体")]
        public GameObject Prefab;

        [Header("最后生成的瓦片")]
        public TileBase TileBase;

        [Header("耐久度")]
        public int Durability;

        [Header("其他属性")]
        public Vector3 offset;

    }
}