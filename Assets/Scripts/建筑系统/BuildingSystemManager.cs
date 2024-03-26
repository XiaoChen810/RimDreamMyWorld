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

        private BuildingModeTool BuildingModeTool;

        [Header("待分配列表")]
        [SerializeField] private List<GameObject> _ToDoList = new();
        [Header("已分配列表")]
        [SerializeField] private List<GameObject> _DoingList = new();
        [Header("已完成建筑的列表")]
        [SerializeField] private List<GameObject> _DoneList = new();

        protected override void Awake()
        {
            base.Awake();
            LoadBlueprintData();
            BuildingModeTool = GetComponent<BuildingModeTool>();
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

        #region ListCompleted

        /// <summary>
        /// 通过名字获取其中一个建筑的GameObject
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetBuildingObjFromListCompleted(string name)
        {
            foreach (var item in _DoneList)
            {
                Building building = item.GetComponent<Building>();
                if (building.Data.Name == name && !building.IsUsed)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加建造完成的建筑到建筑列表，要确保建筑有BlueprintBase组件
        /// </summary>
        /// <param name="building"></param>
        public void AddBuildingToListCompleted(GameObject building)
        {
            BlueprintBase blueprint = building.GetComponent<BlueprintBase>();
            if (blueprint != null)
            {
                AddBuildingListDone(building);
            }
            else
            {
                Debug.LogWarning("检测到没有 BlueprintBase 的物体想添加进 BuildingHashSet ，已返回");
            }
        }

        public void AddBuildingListDone(GameObject building)
        {
            _DoneList.Add(building);
        }

        #endregion

        #region  Task

        /// <summary>
        ///  依次访问字典，找对存在的蓝图数据并返回
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BlueprintData GetData(string name)
        {
            if (_FloorBlueprintsDict.TryGetValue(name, out var floorData))
            {
                CreateBlueprintPrefabIfNull<Building>(name, floorData);
                if (floorData.Prefab != null)
                    return _FloorBlueprintsDict[name];
            }
            if (_WallBlueprintsDict.TryGetValue(name, out var wallData))
            {
                CreateBlueprintPrefabIfNull<Building>(name, wallData);
                if (wallData.Prefab != null)
                    return _WallBlueprintsDict[name];
            }
            if (_BuildingBlueprintsDict.TryGetValue(name, out var buildingData))
            {
                CreateBlueprintPrefabIfNull<Building>(name, buildingData);
                if (buildingData.Prefab != null)
                    return _BuildingBlueprintsDict[name];
            }
            if (_FurnitureBlueprintsDict.TryGetValue(name, out var furnitueData))
            {
                CreateBlueprintPrefabIfNull<Building>(name, furnitueData);
                if (furnitueData.Prefab != null)
                    return _FurnitureBlueprintsDict[name];
            }
            if (_OtherBlueprintsDict.TryGetValue(name, out var otherBlueprintData))
            {
                CreateBlueprintPrefabIfNull<Building>(name, otherBlueprintData);
                if (otherBlueprintData.Prefab != null)
                    return _OtherBlueprintsDict[name];
            }

            Debug.LogWarning($"未能找到{name}的预制件数据");
            return null;

            //当没有对应的预制件时生成一个
            void CreateBlueprintPrefabIfNull<T>(string name, BlueprintData data) where T : BlueprintBase
            {
                if (data.Prefab == null)
                {
                    Debug.LogWarning("此数据预制件为空，已经生成临时预制件");

                    // 创建并添加SpriteRenderer，设置显示层
                    GameObject prefab = new GameObject(name);
                    SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                    if (data.PreviewSprite != null) spriteRenderer.sprite = data.PreviewSprite;
                    else
                    {
                        spriteRenderer.sprite = null;
                        Debug.LogWarning("生成的预览图为空");
                    }
                    spriteRenderer.sortingLayerName = "Above";
                    // 使用 Instantiate 进行实例化
                    prefab = Instantiate(prefab);
                    // 添加触发器
                    BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                    boxCollider.isTrigger = true;
                    // 添加对应的蓝图基类
                    BlueprintBase blueprintBase = prefab.AddComponent<T>();
                    blueprintBase.Data = data;
                    //// 设置路径，保存为预制件
                    string path = "Assets/Prefabs/Buffer/" + name + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    // 销毁临时预制件对象
                    Destroy(prefab);
                    // 存
                    GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Buffer/" + name + ".prefab");
                    data.Prefab = savedPrefab;
                }
            }
        }

        /// <summary>
        /// 把一个还未建造的建筑物添加到待分配队列中
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(GameObject task)
        {
            _ToDoList.Add(task);
            Debug.Log("Add task :" + task.name);
        }

        /// <summary>
        /// 完成任务，在已分配队列中移除任务
        /// </summary>
        /// <param name="task"></param>
        public void CompleteTask(GameObject task)
        {
            if (_DoingList.Contains(task))
            {
                _DoingList.Remove(task);
                _DoneList.Add(task);    
                Debug.Log("Complete task :" + task.name);
                return;
            }
            Debug.Log("Don't find task :" + task.name);
        }

        /// <summary>
        /// 取消任务，将任务从待分配列表里删除
        /// </summary>
        /// <param name="task"></param>
        public void CanelTask(GameObject task)
        {
            if (_ToDoList.Contains(task))
            {
                _ToDoList.Remove(task);
                Debug.Log("Cancel task :" + task.name);
                return;
            }
            Debug.Log("Don't find task :" + task.name);
        }

        /// <summary>
        /// 中断任务，在已分配列表里找到放回待分配列表
        /// </summary>
        /// <param name="task"></param>
        public void InterpretTask(GameObject task)
        {
            if (_DoingList.Contains(task))
            {
                _DoingList.Remove(task);
                _ToDoList.Add(task);
                Debug.Log("Interpret task :" + task.name);
                return;
            }
            Debug.Log("Don't find task :" + task.name);
        }

        /// <summary>
        ///  从待分配队列中获取一个任务，并放到已分配队列中去,返回一个建筑蓝图的物体
        /// </summary>
        /// <returns></returns>
        public GameObject GetTask()
        {
            if (_ToDoList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, _ToDoList.Count);
                GameObject result = _ToDoList[index];
                _ToDoList.RemoveAt(index);
                _DoingList.Add(result);
                BlueprintBase blueprint = result.GetComponent<BlueprintBase>();
                return result;
            }
            return null;
        }

        #endregion

        #region BuildingSystem

        /// <summary>
        ///  使用蓝图，通过name
        /// </summary>
        /// <param name="name"></param>
        public void UseBlueprint(string name)
        {
            BuildingModeTool.ToggleBlueprint(name);
        }

        #endregion

        #endregion
    }
}