using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace 建筑系统
{
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
        private List<BuildingBlueprintBase> _currentTaskList = new();


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            if(_currentTaskList.Count > 0)
            {
                BuildingBlueprintBase task = _currentTaskList[0];
                task.Complete();
            }
        }

        public BlueprintData GetData(string name)
        {
            if (_floorBlueprintsDict.ContainsKey(name))
            {
                return _floorBlueprintsDict[name];
            }
            if (_WallBlueprintsDict.ContainsKey(name))
            {
                return _WallBlueprintsDict[name];
            }

            Debug.LogWarning("未能找到该数据" + name);
            return null;
        }

        public void AddTask(BuildingBlueprintBase task)
        {
            _currentTaskList.Add(task);
        }

        public BuildingBlueprintBase GetTask()
        {
            return null;
        }

        public void RemoveTask(BuildingBlueprintBase task)
        {
            _currentTaskList?.Remove(task);
        }
    }
}