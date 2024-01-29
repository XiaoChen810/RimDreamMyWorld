using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System;

namespace ����ϵͳ
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
        public SerializedDictionary<string, BlueprintData> _floorBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("ȫ��ǽ����ͼ���ֵ�")]
        [SerializedDictionary("����", "��ͼ����")]
        public SerializedDictionary<string, BlueprintData> _WallBlueprintsDict = new SerializedDictionary<string, BlueprintData>();

        [Header("BuildingSystem")]
        public BuildingSystem _BuildingSystem;


        // ��ǰ��������б�
        private Queue<BuildingBlueprintBase> _currentTaskQueue = new();
        // �Ѿ�����õ��б�
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
        ///  ���η����ֵ䣬�ҶԴ��ڵ���ͼ���ݲ�����
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

            Debug.LogWarning("δ���ҵ�������" + name);
            return null;
        }

        public void AddTask(BuildingBlueprintBase task)
        {
            _currentTaskQueue.Enqueue(task);
            // ����������ﱻ�������ʱ��֪ͨȫ������С�˿��Է�������
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