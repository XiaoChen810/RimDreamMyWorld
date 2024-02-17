using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;
using UnityEditor;

namespace MyBuildingSystem
{
    /// <summary>
    ///  ��¼ȫ����ͼ
    ///  ��¼��ǰ�������
    /// </summary>
    public class BuildingSystemManager : MonoBehaviour
    {
        public static BuildingSystemManager Instance;

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

        public BlueprintTool BlueprintTool { get; private set; }

        [Header("���ָʾ��")]
        public GameObject MouseIndicator;

        // ��ǰ��������б�
        private Queue<BlueprintBase> _currentTaskQueue = new();
        // �Ѿ�����õ��б�
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

            // �����������������ʱ��֪ͨȫ������С�˿��Է�������
            if (_currentTaskQueue.Count > 0)
            {
                OnTaskQueueAdded?.Invoke();
            }

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

        #region  Task

        /// <summary>
        ///  ���η����ֵ䣬�ҶԴ��ڵ���ͼ���ݲ�����
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

            Debug.LogWarning($"δ���ҵ�{name}����");
            return null;

            // ��û�ж�Ӧ��Ԥ�Ƽ�ʱ����һ��
            void CreateBlueprintPrefab<T>(string name, BlueprintData data) where T : BlueprintBase
            {
                if (data.Prefab == null)
                {
                    Debug.Log("������Ԥ�Ƽ�Ϊ�գ��Ѿ�������ʱԤ�Ƽ�");

                    // ���������SpriteRenderer��������ʾ��
                    GameObject prefab = new GameObject(name);
                    SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
                    if (data.PreviewSprite != null) spriteRenderer.sprite = data.PreviewSprite;
                    else
                    {
                        spriteRenderer.sprite = null;
                        Debug.LogWarning("���ɵ�Ԥ��ͼΪ��");
                    }
                    spriteRenderer.sortingLayerName = "Above";
                    // ʹ�� Instantiate ����ʵ����
                    prefab = Instantiate(prefab);
                    // ��Ӵ�����
                    BoxCollider2D boxCollider = prefab.AddComponent<BoxCollider2D>();
                    boxCollider.isTrigger = true;
                    // ��Ӷ�Ӧ����ͼ����
                    BlueprintBase blueprintBase = prefab.AddComponent<T>();
                    blueprintBase._BlueprintData = data;
                    // ����·��������ΪԤ�Ƽ�
                    string path = "Assets/Resources/Prefabs/Blueprints/" + name + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    // ������ʱԤ�Ƽ�����
                    Destroy(prefab);
                    // ��
                    GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/Blueprints/" + name + ".prefab");
                    data.Prefab = savedPrefab;
                }
            }
        }

        /// <summary>
        ///  ������񵽴����������
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(BlueprintBase task)
        {
            _currentTaskQueue.Enqueue(task);
        }

        /// <summary>
        ///  ����������ѷ���������Ƴ�
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
        ///  ���½�������Żش��������
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
        ///  �Ӵ���������л�ȡһ�����񣬲��ŵ��ѷ��������ȥ
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
        ///  ʹ����ͼ��ͨ��name
        /// </summary>
        /// <param name="name"></param>
        public void UseBlueprint(string name)
        {
            BlueprintTool.ToggleBlueprint(name);
        }

        #endregion
    }
}