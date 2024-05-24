using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using ChenChen_UISystem;
using ChenChen_Scene;
using System.IO;
using ChenChen_MapGenerator;
using Unity.Burst.Intrinsics;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// ��¼ȫ����ͼ
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [Header("����ȫ����Ʒ������ֵ�")]
        [SerializedDictionary("����", "��Ʒ����")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

        [Header("����ȫ���Ѿ����ɵ������б�")]
        /// <summary>
        /// �����������б�
        /// </summary>
        public List<ThingBase> ThingBuildingGeneratedList_NotCommonlyUsed = new List<ThingBase>();
        /// <summary>
        /// ���������б�
        /// </summary>
        public List<ThingBase> ThingBuildingGeneratedList = new List<ThingBase>();

        private BuildingModeTool tool;
        public BuildingModeTool Tool // ������Ʒ�Ľ���
        {
            get
            {
                if (tool == null)
                {
                    tool = new BuildingModeTool(this);
                }
                return tool;
            }
        }

        private Quadtree quadtree;   // ���������Ĳ���
        public Quadtree Quadtree
        {
            get
            {
                if (quadtree == null)
                {
                    Rect worldBounds = new Rect(0, 0, MapManager.Instance.CurMapWidth, MapManager.Instance.CurMapHeight);
                    quadtree = new Quadtree(0, worldBounds, this.gameObject, "root");
                }
                return quadtree;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            LoadAllThingDefData();
        }

        public void Update()
        {
            Tool.BuildUpdate();
        }

        private void LoadAllThingDefData()
        {
            ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

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
                        Debug.LogWarning($"ThingDef with name '{ThingData.DefName}' already exists. Skipping.");
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
            if (obj.TryGetComponent<ThingBase>(out var thing))
            {
                if (thing.Def.Type == ThingType.Tree)
                {
                    ThingBuildingGeneratedList_NotCommonlyUsed.Add(thing);
                }
                else
                {
                    ThingBuildingGeneratedList.Add(thing);
                }

                Quadtree.Insert(obj);
            }
            else
            {
                Debug.LogWarning("��⵽û�� ThingBase �������������ӽ� _BuildingList ���ѷ���");
                return;
            }
        }
        /// <summary>
        /// �������Ƴ����������б�
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveThingToList(GameObject obj)
        {
            if (obj.TryGetComponent<Thing_Building>(out var building) && ThingBuildingGeneratedList.Contains(building))
            {
                ThingBuildingGeneratedList.Remove(building);
            }
            else
            {
                if (ThingBuildingGeneratedList_NotCommonlyUsed.Contains(building))
                {
                    ThingBuildingGeneratedList_NotCommonlyUsed.Remove(building);
                    return;
                }
                else
                {
                    Debug.Log("�б���û���������");
                    return;
                }
            }
        }
        /// <summary>
        /// ��ȡ������������б�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"> �Ƿ���Ҫһ��û�б�ʹ�õ� </param>
        /// <param name="isNotCommonlyUsed"> �Ƿ�Ӳ������б���� </param>
        /// <returns></returns>
        public GameObject GetThingGenerated(string name = null, bool needFree = true, bool isNotCommonlyUsed = false)
        {
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            if (isNotCommonlyUsed)
            {
                foreach (var building in ThingBuildingGeneratedList_NotCommonlyUsed)
                {
                    if (name != null && building.Def.DefName != name) continue;
                    if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return building.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������������б�
        /// </summary>
        /// <param name="lifeState"> ���崦��ʲô�׶� </param>
        /// <param name="name"></param>
        /// <param name="needFree"> �Ƿ���Ҫһ��û�б�ʹ�õ� </param>
        /// <param name="isNotCommonlyUsed"> �Ƿ�Ӳ������б���� </param>
        /// <returns></returns>
        public GameObject GetThingGenerated(BuildingLifeStateType lifeState, string name = null, bool needFree = true, bool isNotCommonlyUsed = false)
        {
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (building.LifeState != lifeState) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            if (isNotCommonlyUsed)
            {
                foreach (var building in ThingBuildingGeneratedList)
                {
                    if (building.LifeState != lifeState) continue;
                    if (name != null && building.Def.DefName != name) continue;
                    if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return building.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// ��ȡȫ��������������б�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="name">ָ�������֣���ѡ��</param>
        /// <param name="needFree">�Ƿ���Ҫһ��û�б�ʹ�õ����壨Ĭ��ֵΪ true��</param>
        /// <param name="isNotCommonlyUsed"> �Ƿ�Ӳ������б���� </param>
        /// <returns>���������������б�</returns>
        public List<T> GetThingsGenerated<T>(string name = null, bool needFree = true, bool isNotCommonlyUsed = false) where T : ThingBase
        {
            List<T> list = new List<T>();
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (!building.TryGetComponent<T>(out T component)) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                list.Add(component);
            }
            if (isNotCommonlyUsed)
            {
                foreach (var building in ThingBuildingGeneratedList)
                {
                    if (!building.TryGetComponent<T>(out T component)) continue;
                    if (name != null && building.Def.DefName != name) continue;
                    if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    list.Add(component);
                }
            }
            return list;
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
                    string folderPath = "Assets/Resources/Prefabs/ThingDef"; // �����ļ���·��
                    string fileName = $"{def.DefName}_Prefab.prefab"; // �ļ���

                    string filePath = FindFileInFolder(folderPath, fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // ���ļ�·������ȡ���·���Ա��� Resources.Load
                        string resourcePath = filePath.Substring(filePath.IndexOf("Resources/") + 10); // ȥ�� "Resources/" ����չ�� ".prefab"
                        resourcePath = resourcePath.Replace(".prefab", "");

                        def.Prefab = Resources.Load<GameObject>(resourcePath);

                        if (def.Prefab == null)
                        {
                            Debug.LogError($"Prefab not found at: {resourcePath}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Prefab file not found in folder: {folderPath}");
                    }
                }
                if (def.Prefab == null)
                {
                    Debug.LogError($"������һ������ {name}�������Ԥ�Ƽ�Ϊ��");
                    return ThingDefDictionary[name];
                }
                return ThingDefDictionary[name];

                string FindFileInFolder(string folderPath, string fileName)
                {
                    // �ݹ������ļ���
                    foreach (string file in Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file) == fileName)
                        {
                            return file;
                        }
                    }
                    return null; // δ�ҵ��ļ�������null
                }
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
            Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot, thingSave.LifeState, thingSave.MapName);
            return true;
        }
        // ֱ��ʹ�ö���
        public bool TryGenerateThing(ThingDef thingDef, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            Generate(thingDef, position + thingDef.Offset, routation, initLifdState, mapName);
            return true;
        }
        // ֱ��ʹ������
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            ThingDef thingDef = GetThingDef(thingName);
            Generate(thingDef, position + thingDef.Offset, routation, initLifdState, mapName);
            return true;
        }
        // ����
        private void Generate(ThingDef thingDef, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            GameObject thingObj = Instantiate(thingDef.Prefab, position, routation, transform);
            thingObj.name = thingDef.DefName;
            ThingBase thing = thingObj.GetComponent<ThingBase>();
            thing.OnPlaced(initLifdState, mapName);
            AddThingToList(thingObj);
        }

        #endregion
    }
}