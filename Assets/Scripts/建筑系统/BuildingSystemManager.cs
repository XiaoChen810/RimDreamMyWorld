using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance;

    [Header("全部地板蓝图的字典")]
    [SerializedDictionary("名称", "蓝图数据")]
    public SerializedDictionary<string, BlueprintData> _floorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

    [Header("全部墙体蓝图的字典")]
    [SerializedDictionary("名称", "蓝图数据")]
    public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

    // 当前所需建造蓝图的列表
    private List<BuildingBlueprintBase> _currentBlueprintList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
