using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyBuildingSystem
{
    public enum BlueprintType
    {
        Floor,Wall,Building,Furniture,Other
    }

    [CreateAssetMenu(menuName = "蓝图数据", fileName = "蓝图数据", order = 0)]
    public class BlueprintData : ScriptableObject
    {
        public string Name;

        [Header("类型")]
        public BlueprintType Type;

        [Header("占据格子的长和宽")]
        public int BuildingWidth;
        public int BuildingHeight;

        [Header("建造所需工作量")]
        public int Workload;

        [Header("预览图")]
        public Sprite PreviewSprite;

        [Header("是否可以在上方继续放东西")]
        public bool Stackable;

        [Header("最后生成的建筑的预制体")]
        public GameObject Prefab;

        [Header("最后生成的瓦片")]
        public TileBase TileBase;
    }
}