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
    /// 记录全部蓝图
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [SerializedDictionary("名称", "物品定义")]
        public SerializedDictionary<string, BuildingDef> ThingDefDictionary = new SerializedDictionary<string, BuildingDef>();

        private LinkedList<Thing> things = new LinkedList<Thing>();

        private BuildingModeTool tool; // 建造模式工具
        private Quadtree quadtree;   // 存放物体的四叉树

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
            // 加载所有ThingDef资源
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
        /// 打开建造模式，通过name放置蓝图
        /// </summary>
        /// <param name="name"></param>
        public void OpenBuildingMode(string name)
        {
            Tool.BuildStart(GetThingDef(name));
        }
        /// <summary>
        /// 将物体添加到完成列表
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
                Debug.LogWarning("检测到没有 ThingBase 组件的物体想添加进列表 ，已返回");
                return;
            }
        }        
        /// <summary>
        /// 将物体移除
        /// </summary>
        /// <param name="obj">要移除的物体</param>
        public bool RemoveThing(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "传入的物体不能为空");
            }

            if (!obj.TryGetComponent<Thing>(out var thing))
            {
                Debug.LogWarning($"{obj} 没有 <ThingBase> 组件");
                return false;
            }

            things.Remove(thing);

            return true;
        }

        #region 获取物体实例
        /// <summary>
        /// 获取物体实例
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
        /// 获取建筑实例
        /// </summary>
        /// <param name="lifeState"> 物体处于什么阶段 </param>
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
        /// 获取对应类型物品实例
        /// </summary>
        /// <returns>符合条件的物体列表</returns>
        public List<T> GetThingsInstance<T>() where T : Thing
        {
            var res = things.Where(thing => thing is T).Cast<T>().ToList();
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
                    string folderPath = "Assets/Resources/Prefabs/ThingDef"; // 大体文件夹路径
                    string fileName = $"{def.DefName}_Prefab.prefab"; // 文件名

                    string filePath = FindFileInFolder(folderPath, fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // 从文件路径中提取相对路径以便于 Resources.Load
                        string resourcePath = filePath.Substring(filePath.IndexOf("Resources/") + 10); // 去掉 "Resources/" 和扩展名 ".prefab"
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
                    // 递归搜索文件夹
                    foreach (string file in Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file) == fileName)
                        {
                            return file;
                        }
                    }
                    return null; // 未找到文件，返回null
                }
            }

            Debug.LogWarning($"未能找到{name}的定义");
            return null;
        }
        #endregion

        #region 尝试生成一个物体
        // 从存档
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
                Debug.LogError($"定义 {thingDef.DefName}定义的预制件为空");
                return false;
            }
            Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot, true);
            return true;
        }
        // 直接使用定义
        public bool TryGenerateThing(BuildingDef thingDef, Vector2 position, Quaternion routation, bool putInQuadtree)
        {
            Generate(thingDef, position + thingDef.Offset, routation, putInQuadtree);
            return true;
        }
        // 直接使用名字
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
        // 生成
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