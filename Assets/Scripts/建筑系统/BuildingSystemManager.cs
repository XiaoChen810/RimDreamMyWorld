using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

namespace 建筑系统
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
        public SerializedDictionary<string, BlueprintData> _floorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("全部墙体蓝图的字典")]
        [SerializedDictionary("名称", "蓝图数据")]
        public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("BuildingSystem")]
        public BuildingSystem _BuildingSystem;


        // 当前待分配的列表
        private Queue<BuildingBlueprintBase> _currentTaskQueue = new();
        // 已经分配好的列表
        private Queue<BuildingBlueprintBase> _alreadyTaskQueue = new();

        public Action OnTaskQueueAdded;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _BuildingSystem = GetComponentInChildren<BuildingSystem>();
                DontDestroyOnLoad(gameObject);
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
            _currentTaskQueue.Enqueue(task);
            // 当任务队列里被添加任务时，通知全部建筑小人可以分配任务
            OnTaskQueueAdded?.Invoke();
        }

        public void CompleteTask(BuildingBlueprintBase task)
        {
            for(int i = 0; i < _alreadyTaskQueue.Count; i++)
            {
                if(task != _alreadyTaskQueue.Peek())
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

        public void CanelTask(BuildingBlueprintBase task)
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

        public BuildingBlueprintBase GetTask()
        {
            if (_currentTaskQueue.Count > 0)
            {
                BuildingBlueprintBase sult = _currentTaskQueue.Dequeue();
                _alreadyTaskQueue.Enqueue(sult); 
                return sult;
            }
            return null;
        }

        #endregion
    }
}