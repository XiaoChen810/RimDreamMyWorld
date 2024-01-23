using UnityEngine;

[CreateAssetMenu(menuName = "蓝图数据", fileName = "蓝图数据", order = 0)]
public class BlueprintData : ScriptableObject
{
    [Header("占据格子的长和宽")]
    public int BuildingWidth;
    public int BuildingHeight;

    [Header("建造所需工作量")]
    public int Workload;

    [Header("预览图")]
    public Sprite PreviewSprite;

    [Header("最后生成的建筑的预制体")]
    public GameObject Prefab;
}
