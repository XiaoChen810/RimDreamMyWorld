using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using UnityEditor;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// ��¼ȫ����ͼ
    /// </summary>
    public class BuildingSystemManager : SingletonMono<BuildingSystemManager>
    {
        [Header("ȫ���ذ���ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _FloorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("ȫ��ǽ����ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("ȫ��������ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _BuildingBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("ȫ���Ҿ���ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _FurnitureBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("ȫ������������ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _OtherBlueprintsDict = new SerializedDictionary<string, BlueprintData>();
        
        [Header("�����б�")]
        [SerializeField] private List<GameObject> _BuildingList = new();

        protected BuildingModeTool _tool;
        public BuildingModeTool Tool
        {
            get { return _tool; }
        }

        protected override void Awake()
        {
            base.Awake();
            LoadBlueprintData();
            _tool = GetComponent<BuildingModeTool>();
        }

        private void LoadBlueprintData()
        {
            // ��ȡָ��·���µ�����BlueprintData�ļ�
            string[] blueprintDataFiles = AssetDatabase.FindAssets("t:BlueprintData", new[] { "Assets/Resources/Prefabs/Blueprints" });

            foreach (var blueprintDataFile in blueprintDataFiles)
            {
                // ����GUID����BlueprintData
                string blueprintDataAssetPath = AssetDatabase.GUIDToAssetPath(blueprintDataFile);
                BlueprintData blueprintData = AssetDatabase.LoadAssetAtPath<BlueprintData>(blueprintDataAssetPath);

                if (blueprintData != null)
                {
                    switch (blueprintData.Type)
                    {
                        case BlueprintType.Floor:
                            Add(_FloorBlueprintsDict, blueprintData.Name, blueprintData); break;
                        case BlueprintType.Wall:
                            Add(_WallBlueprintsDict, blueprintData.Name, blueprintData); break;
                        case BlueprintType.Building:
                            Add(_BuildingBlueprintsDict, blueprintData.Name, blueprintData); break;
                        case BlueprintType.Furniture:
                            Add(_FurnitureBlueprintsDict, blueprintData.Name, blueprintData); break;
                        case BlueprintType.Other:
                            Add(_OtherBlueprintsDict, blueprintData.Name, blueprintData); break;
                        default:
                            break;
                    }

                }
            }

            void Add(SerializedDictionary<string, BlueprintData> dict, string key, BlueprintData value)
            {
                // ��BlueprintData��ӵ��ֵ���
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
                else
                {
                    Debug.LogWarning($"BlueprintData with name '{key}' already exists. Skipping.");
                }
            }
        }

        #region Public

        public void AddBuildingToList(GameObject obj)
        {
            if(obj.TryGetComponent<Thing_Building>(out var building))
            {
                _BuildingList.Add(obj);
            }
            else
            {
                Debug.LogWarning("��⵽û�� Building �������������ӽ� _BuildingList ���ѷ���");
                return;
            }
        }

        public void RemoveBuildingToList(GameObject obj)
        {
            if (obj.TryGetComponent<Thing_Building>(out var building))
            {
                _BuildingList.Remove(obj);
            }
            else
            {
                Debug.LogWarning("��⵽û�� Building �������������ӽ� _BuildingList ���ѷ���");
                return;
            }
        }

        public GameObject GetBuildingObj(string name = null, bool needFree = true)
        {
            foreach (var item in _BuildingList)
            {
                Thing_Building building = item.GetComponent<Thing_Building>();
                if (name != null && building.Data.Name != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.IsUsed)) continue;
                return item;
            }
            return null;
        }

        public GameObject GetBuildingObj(BuildingStateType state, string name = null, bool needFree = true)
        {
            foreach (var item in _BuildingList)
            {
                Thing_Building building = item.GetComponent<Thing_Building>();
                if (building.State != state) continue;
                if (name != null && building.Data.Name != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.IsUsed)) continue;
                return item;             
            }
            return null;
        }

        /// <summary>
        ///  ���η����ֵ䣬�ҶԴ��ڵ���ͼ���ݲ�����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BlueprintData GetData(string name)
        {
            if (_FloorBlueprintsDict.TryGetValue(name, out var floorData))
            {
                GetBlueprintPrefabIfNull<Thing_Building>(name, floorData);
                if (floorData.Prefab != null)
                    return _FloorBlueprintsDict[name];
            }
            if (_WallBlueprintsDict.TryGetValue(name, out var wallData))
            {
                GetBlueprintPrefabIfNull<Thing_Building>(name, wallData);
                if (wallData.Prefab != null)
                    return _WallBlueprintsDict[name];
            }
            if (_BuildingBlueprintsDict.TryGetValue(name, out var buildingData))
            {
                GetBlueprintPrefabIfNull<Thing_Building>(name, buildingData);
                if (buildingData.Prefab != null)
                    return _BuildingBlueprintsDict[name];
            }
            if (_FurnitureBlueprintsDict.TryGetValue(name, out var furnitueData))
            {
                GetBlueprintPrefabIfNull<Thing_Building>(name, furnitueData);
                if (furnitueData.Prefab != null)
                    return _FurnitureBlueprintsDict[name];
            }
            if (_OtherBlueprintsDict.TryGetValue(name, out var otherBlueprintData))
            {
                GetBlueprintPrefabIfNull<Thing_Building>(name, otherBlueprintData);
                if (otherBlueprintData.Prefab != null)
                    return _OtherBlueprintsDict[name];
            }

            Debug.LogWarning($"δ���ҵ�{name}��Ԥ�Ƽ�����");
            return null;

            //��û�ж�Ӧ��Ԥ�Ƽ�ʱ����һ��
            void GetBlueprintPrefabIfNull<T>(string name, BlueprintData data) where T : ThingBase
            {
                if (data.Prefab == null)
                {
                    string path = $"Prefabs/Blueprints/{name}/{name}_Prefab.prefab";
                    Debug.LogWarning("������Ԥ�Ƽ�Ϊ�գ����Ի�ȡԤ�Ƽ���: " + path);
                    GameObject prefab = Resources.Load<GameObject>(path);
                    if (prefab != null)
                    {
                        data.Prefab = prefab;
                    }
                    else
                    {
                        Debug.LogError("δ����Ԥ�Ƽ�����");
                    }
                }
            }
        }

        #region BuildingSystem

        /// <summary>
        ///  ʹ����ͼ��ͨ��name
        /// </summary>
        /// <param name="name"></param>
        public void UseBlueprint(string name)
        {
            _tool.ToggleBlueprint(name);
        }

        #endregion

        #endregion
    }
}