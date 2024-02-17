using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using UnityEditor;

namespace MyBuildingSystem
{
    /// <summary>
    ///  记录全部蓝图
    ///  记录当前建造队列
    /// </summary>
    public class BuildingSystemManager : MonoBehaviour
    {
        public static BuildingSystemManager Instance;

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

        public BlueprintTool BlueprintTool { get; private set; }

        [Header("鼠标指示器")]
        public GameObject MouseIndicator;

        // 当前待分配的列表
        private Queue<BlueprintBase> _currentTaskQueue = new();
        // 已经分配好的列表
        private Queue<BlueprintBase> _alreadyTaskQueue = new();

        public Action OnTaskQueueAdded;

        public HashSet<Vector3> FloorHashSet = new HashSet<Vector3>();
        public HashSet<Vector3> WallHashSet = new HashSet<Vector3>();
        public HashSet<GameObject> BuildingHashSet = new HashSet<GameObject>();
        public HashSet<GameObject> FurnitureHashSet = new HashSet<GameObject>();


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                BlueprintTool = new BlueprintTool(this);
                LoadBlueprintData();
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            BlueprintTool.BuildUpdate();

            // 当任务队列里有任务时，通知全部建筑小人可以分配任务
            if (_currentTaskQueue.Count > 0)
            {
                OnTaskQueueAdded?.Invoke();
            }

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
                CreateBlueprintPrefab<FloorBlueprints>(name, floorData);
                return _FloorBlueprintsDict[name];
            }
            if (_WallBlueprintsDict.TryGetValue(name, out var wallData))
            {
                CreateBlueprintPrefab<WallBlueprints>(name, wallData);
                return _WallBlueprintsDict[name];
            }
            if (_BuildingBlueprintsDict.TryGetValue(name, out var buildingData))
            {
                CreateBlueprintPrefab<DefaultBlueprint>(name, buildingData);
                return _BuildingBlueprintsDict[name];
            }
            if (_FurnitureBlueprintsDict.TryGetValue(name, out var furnitueData))
            {
                CreateBlueprintPrefab<DefaultBlueprint>(name, furnitueData);
                return _FurnitureBlueprintsDict[name];
            }
            if(_OtherBlueprintsDict.TryGetValue(name,out var otherBlueprintData))
            {
                CreateBlueprintPrefab<DefaultBlueprint>(name, otherBlueprintData);
                return _OtherBlueprintsDict[name];
            }

            Debug.LogWarning($"未能找到{name}数据");
            return null;

            // 当没有对应的预制件时生成一个
            void CreateBlueprintPrefab<T>(string name, BlueprintData data) where T : BlueprintBase
            {
                if (data.Prefab == null)
                {
                    Debug.Log("此数据预制件为空，已经生成临时预制件");

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
                    blueprintBase._BlueprintData = data;
                    // 设置路径，保存为预制件
                    string path = "Assets/Resources/Prefabs/Blueprints/" + name + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    // 销毁临时预制件对象
                    Destroy(prefab);
                    // 存
                    GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Blueprints/" + name + ".prefab");
                    data.Prefab = savedPrefab;
                }
            }
        }

        /// <summary>
        ///  添加任务到待分配队列中
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(BlueprintBase task)
        {
            _currentTaskQueue.Enqueue(task);
        }

        /// <summary>
        ///  将该任务从已分配队列中移除
        /// </summary>
        /// <param name="task"></param>
        public void CompleteTask(BlueprintBase task)
        {
            for (int i = 0; i < _alreadyTaskQueue.Count; i++)
            {
                if (task != _alreadyTaskQueue.Peek())
                {
                    _alreadyTaskQueue.Enqueue(_alreadyTaskQueue.Dequeue());
                }
                else
                {
                    _alreadyTaskQueue.Dequeue();
                    break;
                }
            }
        }

        /// <summary>
        ///  重新将该任务放回待分配队列
        /// </summary>
        /// <param name="task"></param>
        public void CanelTask(BlueprintBase task)
        {
            for (int i = 0; i < _alreadyTaskQueue.Count; i++)
            {
                if (task != _alreadyTaskQueue.Peek())
                {
                    _alreadyTaskQueue.Enqueue(_alreadyTaskQueue.Dequeue());
                }
                else
                {
                    _currentTaskQueue.Enqueue(_alreadyTaskQueue.Dequeue());
                    break;
                }
            }
        }

        /// <summary>
        ///  从待分配队列中获取一个任务，并放到已分配队列中去
        /// </summary>
        /// <returns></returns>
        public BlueprintBase GetTask()
        {
            if (_currentTaskQueue.Count > 0)
            {
                BlueprintBase sult = _currentTaskQueue.Dequeue();
                _alreadyTaskQueue.Enqueue(sult);
                return sult;
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
            BlueprintTool.ToggleBlueprint(name);
        }

        #endregion
    }
}