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
    /// 记录全部蓝图
    /// </summary>
    public class BuildingSystemManager : SingletonMono<BuildingSystemManager>
    {
        [Header("包含全部物品定义的字典")]
        [SerializedDictionary("名称", "物品定义")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

        [Header("包含全部已经生成的物体列表")]
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
                        Debug.LogWarning($"BlueprintData with name '{ThingData.DefName}' already exists. Skipping.");
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
                ThingGeneratedList.Add(obj);
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
            if (obj.TryGetComponent<Thing_Building>(out var building))
            {
                ThingGeneratedList.Remove(obj);
            }
            else
            {
                Debug.LogWarning("检测到没有 Building 组件的物体想添加进 _BuildingList ，已返回");
                return;
            }
        }
        /// <summary>
        /// 获取物体从已生成列表
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
        /// 获取物体从已生成列表
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
                    Debug.LogError($"返回了一个定义 {name}，但其的预制件为空");
                    return ThingDefDictionary[name];
                }
                return ThingDefDictionary[name];
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
            GameObject thingObj = Instantiate(thingDef.Prefab, position + thingDef.offset, routation, transform);
            ThingBase thing = thingObj.GetComponent<ThingBase>();
            thing.OnPlaced(initLifdState, mapName);
        }

        #endregion
    }
}