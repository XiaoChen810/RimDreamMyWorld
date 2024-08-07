using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.IO;
using ChenChen_Map;
using System;
using System.Linq;
using ChenChen_Core;

namespace ChenChen_Thing
{
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [SerializedDictionary("����", "��������")]
        public SerializedDictionary<string, BuildingDef> ThingDefDictionary = new SerializedDictionary<string, BuildingDef>();

        private LinkedList<Thing> things = new LinkedList<Thing>();
        private HashSet<Vector2Int> thingSet;
        private BuildingModeTool tool; 
        private Quadtree quadtree;   
        private XmlLoader xmlLoader;    

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
            xmlLoader = XmlLoader.Instance;
            LoadAllBuildingDefData();
        }

        public void Update()
        {
            Tool.BuildUpdate();
        }

        private void LoadAllBuildingDefData()
        {
            // ��������BuildingDef��Դ
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
                        Debug.LogWarning($"BuildingDef with name '{def.DefName}' already exists. Skipping.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load BuildingDef.");
                }
            }

            Resources.UnloadUnusedAssets();
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

            if(thing.DestroyOutputs.Count > 0)
            {
                foreach(var output in thing.DestroyOutputs)
                {
                    Def def = xmlLoader.GetDef(output.Item1);
                    TryGenerateItem(def, obj.transform.position, output.Item2);
                }
            }

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
            Thing res = things.FirstOrDefault(thing => thing.name == name && thing.UnLock);
            if (res != null)
            {
                return res.gameObject;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ��Ӧ������Ʒʵ��
        /// </summary>
        /// <returns>���������������б�</returns>
        public IReadOnlyList<T> GetThingsInstance<T>() where T : Thing
        {
            var res = things.Where(thing => thing is T && thing.UnLock).Cast<T>().ToList();
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
                    string folderPath = "Assets/Resources/Prefabs/ThingDef"; 
                    string fileName = $"{def.DefName}_Prefab.prefab";

                    string filePath = FindFileInFolder(folderPath, fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string resourcePath = filePath.Substring(filePath.IndexOf("Resources/") + 10); 
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
                    foreach (string file in Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file) == fileName)
                        {
                            return file;
                        }
                    }
                    return null; 
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
        /// <summary>
        /// ���ɽ���
        /// </summary>
        /// <param name="thingDef"></param>
        /// <param name="position"></param>
        /// <param name="routation"></param>
        /// <param name="putInQuadtree"></param>
        /// <returns></returns>
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
        /// <summary>
        /// ������Ʒ
        /// </summary>
        /// <param name="def"></param>
        /// <param name="position"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Item TryGenerateItem(Def def, Vector2 position, int num)
        {
            var obj = new GameObject(def.label);
            obj.transform.position = new Vector2((int)position.x + 0.5f, (int)position.y + 0.5f);

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = def.sprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = Vector2.one;
            sr.color = def.color;

            var item = obj.AddComponent<Item>();
            item.Init(def, num);
            item.GetComponent<BoxCollider2D>().size = sr.size;

            AddThingToList(obj, false);
            return item;
        }
        #endregion

        #endregion
    }
}