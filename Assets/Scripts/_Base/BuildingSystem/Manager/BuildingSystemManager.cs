using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEditor.Rendering;
using ChenChen_MapGenerator;

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

        [Header("Data_ThingSave")]
        public List<Data_ThingSave> ThingSaveList = new();

        public BuildingModeTool Tool { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LoadBlueprintData();
            Tool = new BuildingModeTool(this);
        }

        public void Update()
        {
            Tool.BuildUpdate();
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
                    if (!ThingDefDictionary.ContainsKey(ThingData.DefName))
                    {
                        ThingDefDictionary.Add(ThingData.DefName, ThingData);
                    }
                    else
                    {
                        Debug.LogWarning($"BlueprintData with name '{ThingData.DefName}' already exists. Skipping.");
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
            if (obj.TryGetComponent<Thing_Building>(out var building))
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
                if (name != null && building.Def.DefName != name) continue;
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
                if (building.LifeState != state) continue;
                if (name != null && building.Def.DefName != name) continue;
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
                    def.Prefab = Resources.Load<GameObject>($"Prefabs/ThingDef/{def.DefName}/{def.DefName}_Prefab");
                }
                if (def.Prefab == null)
                {
                    Debug.LogError($"������һ������ {name}�������Ԥ�Ƽ�Ϊ��");
                    return ThingDefDictionary[name];
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
            Tool.BuildStart(GetThingDef(name));
        }
        /// <summary>
        /// ��������һ������
        /// </summary>
        /// <param name="thingSave"> ����浵 </param>
        /// <returns></returns>
        public bool TryGenerateThing(Data_ThingSave thingSave)
        {
            ThingDef thingDef = GetThingDef(thingSave.DefName);
            GameObject prefab = thingDef.Prefab;
            if (prefab == null)
            {
                thingDef.Prefab = Resources.Load<GameObject>($"Prefabs/ThingDef/{thingDef.DefName}/{thingDef.DefName}_Prefab");
                prefab = thingDef.Prefab;
            }
            if (prefab == null)
            {
                Debug.LogError($"���� {thingDef.DefName}�����Ԥ�Ƽ�Ϊ��");
                return false;
            }
            GameObject thingObj = Instantiate(prefab, thingSave.ThingPos, thingSave.ThingRot, transform);
            thingObj.GetComponent<ThingBase>().OnPlaced(thingSave.LifeState, thingSave.MapName);
            return true;
        }
        public bool TryGenerateThing(ThingDef thingDef, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            Generate(thingDef, position, routation, initLifdState, mapName);
            return true;
        }
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            ThingDef thingDef = GetThingDef(thingName);
            Generate(thingDef, position, routation, initLifdState, mapName);
            return true;
        }
        private void Generate(ThingDef thingDef, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            // ����
            GameObject thingObj = Instantiate(thingDef.Prefab, position + thingDef.offset, routation, transform);
            ThingBase thing = thingObj.GetComponent<ThingBase>();
            thing.OnPlaced(initLifdState, mapName);
        }

        #endregion
    }
}