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
        [SerializedDictionary("名称", "建筑定义")]
        public SerializedDictionary<string, BuildingDef> ThingDefDictionary = new SerializedDictionary<string, BuildingDef>();

        private LinkedList<Thing> things = new LinkedList<Thing>();
        private HashSet<Vector2Int> thingSet = new HashSet<Vector2Int>();
        private Dictionary<Vector2Int,LinkedList<Thing>> thingDict = new Dictionary<Vector2Int,LinkedList<Thing>>();

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
            // 加载所有BuildingDef资源
            BuildingDef[] defs = Resources.LoadAll<BuildingDef>("Prefabs/ThingDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!ThingDefDictionary.ContainsKey(def.DefName))
                    {
                        if (def.Prefab == null)
                        {
                            Debug.LogWarning($"{def.DefName} 的预制件为空");
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

        public void OpenBuildingMode(string name)
        {
            Tool.BuildStart(GetThingDef(name));
        }
        public bool RemoveThing(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "传入的物体不能为空");
            }

            if (!obj.TryGetComponent<Thing>(out var thing))
            {
                Debug.LogWarning($"{obj} 没有 <Thing> 组件");
                return false;
            }

            things.Remove(thing);

            if(thing.DestroyOutputs.Count > 0)
            {
                foreach(var output in thing.DestroyOutputs)
                {
                    Def def = xmlLoader.GetDef(output.Item1);
                    GenerateItem(def, obj.transform.position, output.Item2);
                }
            }

            return true;
        }
        /// <summary>
        /// 判断这个地方能否建造
        /// </summary>
        public bool CanBuildHere(Vector2Int position, Vector2Int size)
        {
            Vector2Int bottomLeftPosition = new Vector2Int(
                position.x - size.x / 2,
                position.y - size.y / 2
            );

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Vector2Int checkPosition = new Vector2Int(bottomLeftPosition.x + i, bottomLeftPosition.y + j);
                    if (thingSet.Contains(checkPosition))
                    {
                        return false; // 该位置已被占用，无法建造
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 生成建筑
        /// </summary>
        public void GenerateBuilding(BuildingDef def, Vector2Int position, bool putInQuadtree)
        {
            var offest = def.Offset;
            var spawnPosition = position + offest;
            var obj = Generate(def, spawnPosition, Quaternion.identity, putInQuadtree);

            Building building = obj.GetComponent<Building>();
            building.MarkToBuild();

            if (def.IsEffectBuild) return;

            Vector2Int size = def.Size;
            Vector2Int bottomLeftPosition = new Vector2Int(
                position.x - size.x / 2,
                position.y - size.y / 2
            );

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    thingSet.Add(new Vector2Int(bottomLeftPosition.x + i, bottomLeftPosition.y + j));
                }
            }
        }
        /// <summary>
        /// 生产物品
        /// </summary>
        public Item GenerateItem(Def def, Vector2 position, int num)
        {
            var obj = new GameObject(def.label);
            obj.transform.position = new Vector2((int)position.x + 0.5f, (int)position.y + 0.5f);

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = def.sprite;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = Vector2.one;
            sr.color = def.color;

            var coll = obj.AddComponent<BoxCollider2D>();
            coll.size = Vector2.one;

            var item = obj.AddComponent<Item>();
            item.Init(def, num);

            AddThingToList(obj, false);
            return item;
        }
        // 生成
        private GameObject Generate(BuildingDef thingDef, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            GameObject obj = Instantiate(thingDef.Prefab, position, routation, transform);
            AddThingToList(obj, putInQuadtree);
            if (GameManager.Instance.GameIsStart) AudioManager.Instance.PlaySFX("Placed");
            return obj;
        }
        private void AddThingToList(GameObject obj, bool putInQuadtree)
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
        }
        #endregion

        #region 获取物体实例
        /// <summary>
        /// 获取物体实例
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
        /// 获取对应类型物品实例
        /// </summary>
        /// <returns>符合条件的物体列表</returns>
        public IReadOnlyList<T> GetThingsInstance<T>() where T : Thing
        {
            var res = things.Where(thing => thing is T && thing.UnLock).Cast<T>().ToList();
            return res;
        }
        /// <summary>
        /// 获取全部物体实例
        /// </summary>
        /// <returns></returns>     
        public List<Thing> GetAllThingsInstance()
        {
            return things.ToList();
        }
        /// <summary>
        /// 访问字典，找到存在的物品定义并返回
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
                    Debug.LogError($"返回了一个定义 {name}，但其的预制件为空");
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

            Debug.LogWarning($"未能找到{name}的定义");
            return null;
        }
        #endregion
    }
}