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
        [Header("必须属性")]
        public string DefName;
        public ThingType Type;

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

        //----------------------------------------------------------------------
        [Header("可选属性")]

        [Tooltip("是否是障碍物")]
        public bool IsObstacle;

        [Tooltip("最后生成的瓦片")]
        public TileBase TileBase;

        [Tooltip("是否选择不生成实体")]
        public bool IsNotInstancing;

        [Tooltip("其他属性")]
        public bool CanRotation;

    }
}