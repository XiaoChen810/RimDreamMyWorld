using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.IO;
using ChenChen_Map;
using System;
using System.Linq;

namespace ChenChen_Thing
{
    /// <summary>
    /// ��¼ȫ����ͼ
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [SerializedDictionary("����", "��Ʒ����")]
        public SerializedDictionary<string, BuildingDef> ThingDefDictionary = new SerializedDictionary<string, BuildingDef>();

        private LinkedList<Thing> things = new LinkedList<Thing>();

        private BuildingModeTool tool; // ����ģʽ����
        private Quadtree quadtree;   // ���������Ĳ���

        public BuildingModeTool Tool 
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
            // ��������ThingDef��Դ
            BuildingDef[] defs = Resources.LoadAll<BuildingDef>("Prefabs/ThingDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!ThingDefDictionary.ContainsKey(def.DefName))
                    {
                        if (def.Prefab == null)
                        {
                            Debug.LogWarning($"{def.DefName} ��Ԥ�Ƽ�Ϊ��");
                        }
                        ThingDefDictionary.Add(def.DefName, def);
                    }
                    else
                    {
                        Debug.LogWarning($"ThingDef with name '{def.DefName}' already exists. Skipping.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load ThingDef.");
                }
            }
        }

        #region Public
        /// <summary>
        /// �򿪽���ģʽ��ͨ��name������ͼ
        /// </summary>
        /// <param name="name"></param>
        public void OpenBuildingMode(string name)
        {
            Tool.BuildStart(GetThingDef(name));
        }
        /// <summary>
        /// ��������ӵ�����б�
        /// </summary>
        /// <param name="obj"></param>
        public void AddThingToList(GameObject obj, bool putInQuadtree)
        {
            if (obj.TryGetComponent<Thing>(out var thing))
            {
                try
                {
                    things.AddLast(thing);

                    if (putInQuadtree) 
                        Quadtree.Insert(obj);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                Debug.LogWarning("��⵽û�� ThingBase �������������ӽ��б� ���ѷ���");
                return;
            }
        }        
        /// <summary>
        /// �������Ƴ�
        /// </summary>
        /// <param name="obj">Ҫ�Ƴ�������</param>
        public bool RemoveThing(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "��������岻��Ϊ��");
            }

            if (!obj.TryGetComponent<Thing>(out var thing))
            {
                Debug.LogWarning($"{obj} û�� <ThingBase> ���");
                return false;
            }

            things.Remove(thing);

            return true;
        }

        #region ��ȡ����ʵ��
        /// <summary>
        /// ��ȡ����ʵ��
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetThingInstance(string name)
        {
            Thing res = things.FirstOrDefault(thing => thing.name == name && thing.IsFree);
            if (res != null)
            {
                return res.gameObject;
            }
            return null;
        }

        /// <summary>
        /// ��ȡ����ʵ��
        /// </summary>
        /// <param name="lifeState"> ���崦��ʲô�׶� </param>
        /// <returns></returns>
        public GameObject GetBuildingInstance(BuildingLifeStateType lifeState)
        {
            foreach(Thing thing in things)
            {
                if(thing == null)
                {
                    throw new ArgumentNullException(nameof(thing));
                }
                if(thing.TryGetComponent<Building>(out Building building))
                {
                    if(building.LifeState == lifeState && building.IsFree)
                    {
                        return building.gameObject;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// ��ȡ��Ӧ������Ʒʵ��
        /// </summary>
        /// <returns>���������������б�</returns>
        public List<T> GetThingsInstance<T>() where T : Thing
        {
            var res = things.Where(thing => thing is T).Cast<T>().ToList();
            return res;
        }

        /// <summary>
        /// ��ȡȫ������ʵ��
        /// </summary>
        /// <returns></returns>
        public List<Thing> GetAllThingsInstance()
        {
            return things.ToList();
        }
        /// <summary>
        /// �����ֵ䣬�ҵ����ڵ���Ʒ���岢����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BuildingDef GetThingDef(string name)
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
        #endregion

        #region ��������һ������
        // �Ӵ浵
        [Obsolete]
        public bool TryGenerateThing(Data_ThingSave thingSave)
        {
            BuildingDef thingDef = GetThingDef(thingSave.DefName);
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
            Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot, true);
            return true;
        }
        // ֱ��ʹ�ö���
        public bool TryGenerateThing(BuildingDef thingDef, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            Generate(thingDef, position + thingDef.Offset, routation, putInQuadtree);
            return true;
        }
        // ֱ��ʹ������
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            BuildingDef thingDef = GetThingDef(thingName);
            Generate(thingDef, position + thingDef.Offset, routation, putInQuadtree);
            return true;
        }
        public bool TryGenerateBuilding(BuildingDef thingDef, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            var obj = Generate(thingDef, position + thingDef.Offset, routation, putInQuadtree);
            Building building = obj.GetComponent<Building>();
            building.MarkToBuild();
            return true;
        }
        // ����
        private GameObject Generate(BuildingDef thingDef, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            GameObject obj = Instantiate(thingDef.Prefab, position, routation, transform);
            AddThingToList(obj, putInQuadtree);
            if (GameManager.Instance.GameIsStart) AudioManager.Instance.PlaySFX("Placed");
            return obj;
        }
        #endregion

        #endregion
    }
}