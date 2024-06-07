using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.IO;
using ChenChen_Map;
using System;

namespace ChenChen_Thing
{
    /// <summary>
    /// 记录全部蓝图
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [Header("包含全部物品定义的字典")]
        [SerializedDictionary("名称", "物品定义")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

        private Dictionary<Vector2, Thing_Tree> ThingDict_Tree = new();
        private Dictionary<string, LinkedList<ThingBase>> ThingDict = new();


        private BuildingModeTool tool;
        public BuildingModeTool Tool // 处理物品的建造
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

        private Quadtree quadtree;   // 存放物体的四叉树
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
            ThingDef[] defs = Resources.LoadAll<ThingDef>("Prefabs/ThingDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!ThingDefDictionary.ContainsKey(def.DefName))
                    {
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
        public void AddThingToList(GameObject obj)
        {
            if (obj.TryGetComponent<ThingBase>(out var thing))
            {
                switch (thing.Def.Type)
                {
                    case ThingType.Tree:
                        if (!ThingDict_Tree.ContainsKey(obj.transform.position))
                        {
                            ThingDict_Tree.Add(obj.transform.position, thing.GetComponent<Thing_Tree>());
                        }
                        else
                        {
                            Debug.LogWarning("相同的键被添加");
                        }

                        break;
                    default:
                        if (ThingDict.ContainsKey(thing.Def.DefName))
                        {
                            ThingDict[thing.Def.DefName].AddFirst(thing);
                        }
                        else
                        {
                            ThingDict.Add(thing.Def.DefName, new LinkedList<ThingBase>());
                            ThingDict[thing.Def.DefName].AddFirst(thing);
                        }
                        break;
                }

                if(Tool.OnBuildMode && Tool._mouseIndicator == obj)
                {
                    Debug.Log("不添加建造预览指示器");
                }
                else
                {
                    Quadtree.Insert(obj);
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
        public void RemoveThing(GameObject obj)
        {
            // 检查传入的对象是否为null
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "传入的物体不能为空");
            }

            // 尝试获取 ThingBase 组件
            if (!obj.TryGetComponent<ThingBase>(out var thing))
            {
                Debug.LogWarning($"{obj} 没有 <ThingBase> 组件");
                return;
            }

            // 根据 ThingBase 的类型移除相应的物体
            switch (thing.Def.Type)
            {
                case ThingType.Tree:
                    // 检查是否成功移除
                    if (!ThingDict_Tree.Remove(obj.transform.position))
                    {
                        Debug.LogWarning($"移除失败：在 ThingDict_Tree 中找不到位置为 {obj.transform.position} 的物体");
                        return;
                    }
                    break;
                default:
                    // 检查字典中是否包含物体名称
                    if (!ThingDict.ContainsKey(obj.name))
                    {
                        Debug.LogWarning($"移除失败：在 ThingDict 中找不到键为 {obj.name} 的条目");
                        return;
                    }

                    // 检查是否成功移除
                    if (!ThingDict[obj.name].Remove(thing))
                    {
                        Debug.LogWarning($"移除失败：在 ThingDict 中找不到名称为 {obj.name} 的物体");
                        return;
                    }
                    break;
            }
        }

        #region 获取物体实例
        /// <summary>
        /// 获取物体实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingInstance(string name, bool needFree = true)
        {
            if (!ThingDict.ContainsKey(name))
            {
                Debug.LogWarning($"ThingDict 中不存在键为 {name} 的条目");
            }
            else
            {
                foreach (var thing in ThingDict[name])
                {
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取物体实例
        /// </summary>
        /// <param name="lifeState"> 物体处于什么阶段 </param>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingInstance(BuildingLifeStateType lifeState, string name = null, bool needFree = true)
        {
            if (name != null && ThingDict.ContainsKey(name))
            {
                Debug.LogWarning($"ThingDict 中不存在键为 {name} 的条目");
                foreach (var thing in ThingDict[name])
                {
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }           
            foreach (var thinglist in ThingDict)
            {
                foreach (var thing in thinglist.Value)
                {
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取全部物体实例
        /// </summary>
        /// <typeparam name="T">物体类型</typeparam>
        /// <param name="needFree">是否需要一个没有被使用的物体（默认值为 true）</param>
        /// <returns>符合条件的物体列表</returns>
        public List<T> GetThingsInstance<T>(BuildingLifeStateType lifeState = BuildingLifeStateType.None, bool needFree = true) where T : ThingBase
        {
            List<T> list = new List<T>();
            foreach (var thinglist in ThingDict)
            {
                foreach (var thing in thinglist.Value)
                {
                    if (thing is not T) continue;
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    list.Add(thing as T);
                }
            }
            return list;
        }
        /// <summary>
        /// 获取全部物体实例
        /// </summary>
        /// <returns></returns>
        public List<ThingBase> GetAllThingsInstance()
        {
            List<ThingBase> list = new();
            foreach (var thingList in ThingDict)
            {
                foreach(var thing in thingList.Value)
                {
                    list.Add(thing as ThingBase);
                }
            }
            foreach (var thing in ThingDict_Tree)
            {
                list.Add(thing.Value as ThingBase);
            }
            return list;
        }
        /// <summary>
        /// 获取一颗要砍的树
        /// </summary>
        /// <returns></returns>
        public GameObject GetTreeToCut()
        {
            foreach (var tree in ThingDict_Tree)
            {
                if (tree.Value.IsMarkCut)
                {
                    return tree.Value.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// 访问字典，找到存在的物品定义并返回
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
                Debug.LogError($"定义 {thingDef.DefName}定义的预制件为空");
                return false;
            }
            GameObject obj = Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot);
            obj.GetComponent<ThingBase>().ChangeLifeState(thingSave.LifeState);
            return true;
        }
        // 直接使用定义
        public bool TryGenerateThing(ThingDef thingDef, Vector2 position, Quaternion routation)
        {
            GameObject obj = Generate(thingDef, position + thingDef.Offset, routation);
            obj.GetComponent<ThingBase>().Building();
            return true;
        }
        // 直接使用名字
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation)
        {
            ThingDef thingDef = GetThingDef(thingName);
            GameObject obj = Generate(thingDef, position + thingDef.Offset, routation);
            obj.GetComponent<ThingBase>().Building();
            return true;
        }
        // 生成
        private GameObject Generate(ThingDef thingDef, Vector2 position, Quaternion routation)
        {
            GameObject obj = Instantiate(thingDef.Prefab, position, routation, transform);
            AddThingToList(obj);
            if (GameManager.Instance.GameIsStart) AudioManager.Instance.PlaySFX("Placed");
            return obj;
        }
        #endregion

        #endregion
    }
}