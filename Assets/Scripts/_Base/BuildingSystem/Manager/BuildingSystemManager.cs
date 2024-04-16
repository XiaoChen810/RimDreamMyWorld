using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using UnityEditor;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 记录全部蓝图
    /// </summary>
    public class BuildingSystemManager : SingletonMono<BuildingSystemManager>
    {
        [Header("全部地板蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _FloorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("全部墙体蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("全部建筑蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _BuildingBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("全部家具蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _FurnitureBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("全部关于其他蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _OtherBlueprintsDict = new SerializedDictionary<string, BlueprintData>();
        
        [Header("建筑列表")]
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
            // 获取指定路径下的所有BlueprintData文件
            string[] blueprintDataFiles = AssetDatabase.FindAssets("t:BlueprintData", new[] { "Assets/Resources/Prefabs/Blueprints" });

            foreach (var blueprintDataFile in blueprintDataFiles)
            {
                // 根据GUID加载BlueprintData
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
                // 将BlueprintData添加到字典中
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
                Debug.LogWarning("检测到没有 Building 组件的物体想添加进 _BuildingList ，已返回");
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
                Debug.LogWarning("检测到没有 Building 组件的物体想添加进 _BuildingList ，已返回");
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
        ///  依次访问字典，找对存在的蓝图数据并返回
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

            Debug.LogWarning($"未能找到{name}的预制件数据");
            return null;

            //当没有对应的预制件时生成一个
            void GetBlueprintPrefabIfNull<T>(string name, BlueprintData data) where T : ThingBase
            {
                if (data.Prefab == null)
                {
                    string path = $"Prefabs/Blueprints/{name}/{name}_Prefab.prefab";
                    Debug.LogWarning("此数据预制件为空，尝试获取预制件从: " + path);
                    GameObject prefab = Resources.Load<GameObject>(path);
                    if (prefab != null)
                    {
                        data.Prefab = prefab;
                    }
                    else
                    {
                        Debug.LogError("未发现预制件数据");
                    }
                }
            }
        }

        #region BuildingSystem

        /// <summary>
        ///  使用蓝图，通过name
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