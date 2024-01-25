using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ����ϵͳ
{
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

            Debug.LogWarning("δ���ҵ�������" + name);
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