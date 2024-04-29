using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEditor;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// ��¼ȫ����ͼ
    /// </summary>
    public class BuildingSystemManager : SingletonMono<BuildingSystemManager>
    {
        [Header("����ȫ����Ʒ������ֵ�")]
        [SerializedDictionary("����", "��Ʒ����")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary = new SerializedDictionary<string, ThingDef>();
        
        [Header("����ȫ���Ѿ����ɵ������б�")]
        [SerializeField] private List<GameObject> ThingGeneratedList = new();

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
            // ��ȡָ��·���µ�����ThingDef�ļ�
            string[] ThingDataFiles = AssetDatabase.FindAssets("t:ThingDef", new[] { "Assets/Resources/Prefabs/ThingDef" });

            foreach (var ThingDataFile in ThingDataFiles)
            {
                // ����GUID����ThingDef
                string ThingDataAssetPath = AssetDatabase.GUIDToAssetPath(ThingDataFile);
                ThingDef ThingData = AssetDatabase.LoadAssetAtPath<ThingDef>(ThingDataAssetPath);

                if (ThingData != null)
                {
                    if (!ThingDefDictionary.ContainsKey(ThingData.Name))
                    {
                        ThingDefDictionary.Add(ThingData.Name, ThingData);
                    }
                    else
                    {
                        Debug.LogWarning($"BlueprintData with name '{ThingData.Name}' already exists. Skipping.");
                    }
                }
            }
        }

        #region Public

        /// <summary>
        /// ��������ӵ��������б�
        /// </summary>
        /// <param name="obj"></param>
        public void AddThingToList(GameObject obj)
        {
            if(obj.TryGetComponent<Thing_Building>(out var building))
            {
                ThingGeneratedList.Add(obj);
            }
            else
            {
                Debug.LogWarning("��⵽û�� Building �������������ӽ� _BuildingList ���ѷ���");
                return;
            }
        }
        /// <summary>
        /// �������Ƴ����������б�
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveThingToList(GameObject obj)
        {
            if (obj.TryGetComponent<Thing_Building>(out var building))
            {
                ThingGeneratedList.Remove(obj);
            }
            else
            {
                Debug.LogWarning("��⵽û�� Building �������������ӽ� _BuildingList ���ѷ���");
                return;
            }
        }
        /// <summary>
        /// ��ȡ������������б�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"></param>
        /// <returns></returns>
        public GameObject GetThingGenerated(string name = null, bool needFree = true)
        {
            foreach (var item in ThingGeneratedList)
            {
                Thing_Building building = item.GetComponent<Thing_Building>();
                if (name != null && building.Def.Name != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.IsUsed)) continue;
                return item;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������������б�
        /// </summary>
        /// <param name="state"></param>
        /// <param name="name"></param>
        /// <param name="needFree"></param>
        /// <returns></returns>
        public GameObject GetThingGenerated(BuildingLifeStateType state, string name = null, bool needFree = true)
        {
            foreach (var item in ThingGeneratedList)
            {
                Thing_Building building = item.GetComponent<Thing_Building>();
                if (building.State != state) continue;
                if (name != null && building.Def.Name != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.IsUsed)) continue;
                return item;             
            }
            return null;
        }
        /// <summary>
        /// �����ֵ䣬�ҵ����ڵ���Ʒ���岢����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ThingDef GetThingDef(string name)
        {
            if (ThingDefDictionary.TryGetValue(name, out var def))
            {
                if (def.Prefab == null)
                {
                    def.Prefab = Resources.Load<GameObject>($"Prefabs/ThingDef/{def.Name}/{def.Name}_Prefab");
                }
                if(def.Prefab == null)
                {
                    Debug.LogError($"{name}�Ķ����Ԥ�Ƽ�Ϊ��");
                }
                return ThingDefDictionary[name];
            }

            Debug.LogWarning($"δ���ҵ�{name}�Ķ���");
            return null;
        }
        /// <summary>
        /// �򿪽���ģʽ��ͨ��name������ͼ
        /// </summary>
        /// <param name="name"></param>
        public void OpenBuildingMode(string name)
        {
            _tool.BuildStart(GetThingDef(name));
        }

        #endregion
    }
}