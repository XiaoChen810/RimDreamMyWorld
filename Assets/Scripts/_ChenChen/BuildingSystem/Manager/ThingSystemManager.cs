using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using ChenChen_UISystem;
using ChenChen_Scene;
using System.IO;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 记录全部蓝图
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [Header("包含全部物品定义的字典")]
        [SerializedDictionary("名称", "物品定义")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary;

        [Header("包含全部已经生成的物体列表")]
        [SerializeField] private List<Thing_Building> ThingBuildingGeneratedList;

        public BuildingModeTool Tool { get; private set; }

        public PanelManager BuildingPanelManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LoadAllThingDefData();
            ThingBuildingGeneratedList = new List<Thing_Building>();
            Tool = new BuildingModeTool(this);
            BuildingPanelManager = new PanelManager();
        }

        public void Update()
        {
            Tool.BuildUpdate();
            if (Input.GetKeyDown(KeyCode.U))
            {
                OpenBuildingMenuPanel();
            }
        }

        public void OpenBuildingMenuPanel()
        {
            // 定义面板OnEnter时的回调函数，设置isPanelCreated为true
            PanelBase.Callback onEnterCallback = () =>
            {

            };

            // 定义面板OnExit时的回调函数，重置isPanelCreated为false
            PanelBase.Callback onExitCallback = () =>
            {

            };

            BuildingPanelManager.TogglePanel(new BuildingMenuPanel(onEnterCallback, onExitCallback), SceneType.Main);
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
            if (obj.TryGetComponent<Thing_Building>(out var building))
            {
                ThingBuildingGeneratedList.Add(building);
            }
            else
            {
                Debug.LogWarning("检测到没有 Building 组件的物体想添加进 _BuildingList ，已返回");
                return;
            }
        }
        /// <summary>
        /// 将物体移除到已生成列表
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
                Debug.LogWarning("列表里没有这个物体");
                return;
            }
        }
        /// <summary>
        /// 获取物体从已生成列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingGenerated(string name = null, bool needFree = true)
        {
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            return null;
        }
        /// <summary>
        /// 获取物体从已生成列表
        /// </summary>
        /// <param name="lifeState"> 物体处于什么阶段 </param>
        /// <param name="name"></param>
        /// <param name="needFree"> 是否需要一个没有被使用的 </param>
        /// <returns></returns>
        public GameObject GetThingGenerated(BuildingLifeStateType lifeState, string name = null, bool needFree = true)
        {
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (building.LifeState != lifeState) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
                return building.gameObject;
            }
            return null;
        }
        /// <summary>
        /// 获取全部物体从已生成列表
        /// </summary>
        /// <typeparam name="T">物体类型</typeparam>
        /// <param name="name">指定的名字（可选）</param>
        /// <param name="needFree">是否需要一个没有被使用的物体（默认值为 true）</param>
        /// <returns>符合条件的物体列表</returns>
        public List<T> GetThingsGenerated<T>(string name = null, bool needFree = true) where T : ThingBase
        {
            List<T> list = new List<T>();
            foreach (var building in ThingBuildingGeneratedList)
            {
                if (!building.TryGetComponent<T>(out T component)) continue;
                if (name != null && building.Def.DefName != name) continue;
                if (needFree && (building.TheUsingPawn != null || building.Permission != PermissionBase.PermissionType.IsFree)) continue;
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
            GameObject thingObj = Instantiate(prefab, thingSave.ThingPos, thingSave.ThingRot, transform);
            thingObj.name = thingSave.DefName;
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
            // 生成
            GameObject thingObj = Instantiate(thingDef.Prefab, position + thingDef.Offset, routation, transform);
            ThingBase thing = thingObj.GetComponent<ThingBase>();
            thing.OnPlaced(initLifdState, mapName);
        }

        #endregion
    }
}