using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance;

    [Header("ȫ���ذ���ͼ���ֵ�")]
    [SerializedDictionary("����", "��ͼ����")]
    public SerializedDictionary<string, BlueprintData> _floorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

    [Header("ȫ��ǽ����ͼ���ֵ�")]
    [SerializedDictionary("����", "��ͼ����")]
    public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

    // ��ǰ���轨����ͼ���б�
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
