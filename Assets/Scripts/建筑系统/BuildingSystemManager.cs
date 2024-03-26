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

        private BuildingModeTool BuildingModeTool;

        [Header("�������б�")]
        [SerializeField] private List<GameObject> _ToDoList = new();
        [Header("�ѷ����б�")]
        [SerializeField] private List<GameObject> _DoingList = new();
        [Header("����ɽ������б�")]
        [SerializeField] private List<GameObject> _DoneList = new();

        protected override void Awake()
        {
            base.Awake();
            LoadBlueprintData();
            BuildingModeTool = GetComponent<BuildingModeTool>();
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

        #region ListCompleted

        /// <summary>
        /// ͨ�����ֻ�ȡ����һ��������GameObject
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
        /// ��ӽ�����ɵĽ����������б�Ҫȷ��������BlueprintBase���
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
                Debug.LogWarning("��⵽û�� BlueprintBase ����������ӽ� BuildingHashSet ���ѷ���");
            }
        }

        public void AddBuildingListDone(GameObject building)
        {
            _DoneList.Add(building);
        }

        #endregion

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

            Debug.LogWarning($"δ���ҵ�{name}��Ԥ�Ƽ�����");
            return null;

            //��û�ж�Ӧ��Ԥ�Ƽ�ʱ����һ��
            void CreateBlueprintPrefabIfNull<T>(string name, BlueprintData data) where T : BlueprintBase
            {
                if (data.Prefab == null)
                {
                    Debug.LogWarning("������Ԥ�Ƽ�Ϊ�գ��Ѿ�������ʱԤ�Ƽ�");

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
                    blueprintBase.Data = data;
                    //// ����·��������ΪԤ�Ƽ�
                    string path = "Assets/Prefabs/Buffer/" + name + ".prefab";
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    // ������ʱԤ�Ƽ�����
                    Destroy(prefab);
                    // ��
                    GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Buffer/" + name + ".prefab");
                    data.Prefab = savedPrefab;
                }
            }
        }

        /// <summary>
        /// ��һ����δ����Ľ�������ӵ������������
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(GameObject task)
        {
            _ToDoList.Add(task);
            Debug.Log("Add task :" + task.name);
        }

        /// <summary>
        /// ����������ѷ���������Ƴ�����
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
        /// ȡ�����񣬽�����Ӵ������б���ɾ��
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
        /// �ж��������ѷ����б����ҵ��Żش������б�
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
        ///  �Ӵ���������л�ȡһ�����񣬲��ŵ��ѷ��������ȥ,����һ��������ͼ������
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
        ///  ʹ����ͼ��ͨ��name
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