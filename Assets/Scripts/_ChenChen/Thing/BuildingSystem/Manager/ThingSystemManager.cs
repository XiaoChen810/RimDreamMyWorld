using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using System.IO;
using ChenChen_Map;
using UnityEngine.UIElements;

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

        /// <summary>
        /// Trees列表
        /// </summary>
        [SerializeField] private List<Thing_Tree> ThingList_Trees = new List<Thing_Tree>();
        /// <summary>
        /// Building列表
        /// </summary>
        [SerializeField] private List<Thing_Building> ThingList_Building = new List<Thing_Building>();

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
            ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

            // 获取指定路径下的所有ThingDef文件
            string[] ThingDataFiles = AssetDatabase.FindAssets("t:ThingDef", new[] { "Assets/Resources/Prefabs/ThingDef" });

            foreach (var ThingDataFile in ThingDataFiles)
            {
                // 根据GUID加载ThingDef
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
        /// 将物体添加到已生成列表
        /// </summary>
        /// <param name="obj"></param>
        public void AddThingToList(GameObject obj)
        {
            if (obj.TryGetComponent<ThingBase>(out var thing))
            {
                if (thing.Def.Type == ThingType.Tree)
                {
                    ThingList_Trees.Add(thing.GetComponent<Thing_Tree>());
                }
                else
                {
                    ThingList_Building.Add(thing.GetComponent<Thing_Building>());
                }

                Quadtree.Insert(obj);
            }
            else
            {
                Debug.LogWarning("检测到没有 ThingBase 组件的物体想添加进列表 ，已返回");
                return;
            }
        }
        /// <summary>
        /// 将物体移除已生成列表
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveThing(GameObject obj)
        {
            if (!obj.TryGetComponent<ThingBase>(out var thing))
            {
                Debug.LogWarning($"{obj} 没有 <ThingBase> 组件");
                return;
            }
            else
            {
                if (thing.TryGetComponent<Thing_Building>(out Thing_Building building) && ThingList_Building.Contains(building))
                {
                    ThingList_Building.Remove(building);
                    return;
                }
                if (thing.TryGetComponent<Thing_Tree>(out Thing_Tree tree) && ThingList_Trees.Contains(tree))
                {
                    ThingList_Trees.Remove(tree);
                    return;
                }
                Debug.Log("列表里没有这个物体");
                return;
            }
        }
        /// <summary>
        /// 获取物体实例从已生成列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingInstance(string name = null, bool needFree = true)
        {
            foreach (var building in ThingList_Building)
            {
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            foreach (var tree in ThingList_Trees)
            {
                if (name != null && tree.Def.DefName != name) continue;
                if (needFree && (tree.TheUsingPawn != null || tree.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return tree.gameObject;
            }
            return null;
        }
        /// <summary>
        /// 获取物体实例从已生成列表
        /// </summary>
        /// <param name="lifeState"> 物体处于什么阶段 </param>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingInstance(BuildingLifeStateType lifeState, string name = null, bool needFree = true)
        {
            foreach (var building in ThingList_Building)
            {
                if (building.LifeState != lifeState) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            foreach (var tree in ThingList_Trees)
            {
                if (tree.LifeState != lifeState) continue;
                if (name != null && tree.Def.DefName != name) continue;
                if (needFree && (tree.TheUsingPawn != null || tree.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return tree.gameObject;
            }
            return null;
        }
        /// <summary>
        /// 获取全部物体实例从已生成列表
        /// </summary>
        /// <typeparam name="T">物体类型</typeparam>
        /// <param name="name">指定的名字（可选）</param>
        /// <param name="needFree">是否需要一个没有被使用的物体（默认值为 true）</param>
        /// <returns>符合条件的物体列表</returns>
        public List<T> GetThingsInstance<T>(string name = null, bool needFree = true) where T : ThingBase
        {
            List<T> list = new List<T>();
            foreach (var building in ThingList_Building)
            {
                if (!building.TryGetComponent<T>(out T component)) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                list.Add(component);
            }
            foreach (var tree in ThingList_Trees)
            {
                if (!tree.TryGetComponent<T>(out T component)) continue;
                if (name != null && tree.Def.DefName != name) continue;
                if (needFree && (tree.TheUsingPawn != null || tree.Permission != PermissionBase.PermissionType.IsFree)) continue;
                list.Add(component);
            }

            return list;
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
        /// <summary>
        /// 打开建造模式，通过name放置蓝图
        /// </summary>
        /// <param name="name"></param>
        public void OpenBuildingMode(string name)
        {
            Tool.BuildStart(GetThingDef(name));
        }
        /// <summary>
        /// 尝试生成一个物体
        /// </summary>
        /// <param name="thingSave"> 物体存档 </param>
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
                Debug.LogError($"定义 {thingDef.DefName}定义的预制件为空");
                return false;
            }
            Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot, thingSave.LifeState, thingSave.MapName);
            return true;
        }
        // 直接使用定义
        public bool TryGenerateThing(ThingDef thingDef, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            Generate(thingDef, position + thingDef.Offset, routation, initLifdState, mapName);
            return true;
        }
        // 直接使用名字
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation, BuildingLifeStateType initLifdState, string mapName)
        {
            ThingDef thingDef = GetThingDef(thingName);
            Generate(thingDef, position + thingDef.Offset, routation, initLifdState, mapName);
            return true;
        }
        // 生成
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