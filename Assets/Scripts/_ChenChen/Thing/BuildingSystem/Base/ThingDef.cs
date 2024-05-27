using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    /// <summary>
    /// 物品定义
    /// </summary>
    [CreateAssetMenu(menuName = "定义/物体", fileName = "ThingDef", order = 0)]
    public class ThingDef : ScriptableObject
    {
        public string DefName;

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
        public Vector2 Offset;
        public bool CanRotation;

    }
}