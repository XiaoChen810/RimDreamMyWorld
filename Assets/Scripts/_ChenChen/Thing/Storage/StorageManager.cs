using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace ChenChen_Thing
{
    /// <summary>
    /// 存储系统
    /// </summary>
    public class StorageManager : SingletonMono<StorageManager>
    {
        [Header("包含全部物品定义的字典")]
        [SerializedDictionary("名称", "材料定义")]
        public SerializedDictionary<string, StuffDef> StuffDefDictionary = new();

        [Header("库存列表")]
        [SerializedDictionary("名称", "材料")]
        public SerializedDictionary<string, Stuff> StoragesDictionary = new();

        protected override void Awake()
        {
            base.Awake();
            LoadAllStuffDefData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                AddStorage("小麦", UnityEngine.Random.Range(1, 10));
                AddStorage("玉米", UnityEngine.Random.Range(1, 10));
            }
        }

        private void LoadAllStuffDefData()
        {
            StuffDefDictionary = new SerializedDictionary<string, StuffDef>();

            // 获取指定路径下的所有ThingDef文件
            string[] DefDataFiles = AssetDatabase.FindAssets("t:StuffDef", new[] { "Assets/Resources/Prefabs/StuffDef" });

            foreach (var def in DefDataFiles)
            {
                // 根据GUID加载Def
                string StuffDefDataAssetPath = AssetDatabase.GUIDToAssetPath(def);
                StuffDef stuffDef = AssetDatabase.LoadAssetAtPath<StuffDef>(StuffDefDataAssetPath);

                if (stuffDef != null)
                {
                    if (!StuffDefDictionary.ContainsKey(stuffDef.Name))
                    {
                        StuffDefDictionary.Add(stuffDef.Name, stuffDef);
                    }
                    else
                    {
                        Debug.LogWarning($"材料定义 '{stuffDef.Name}' 重复加载。 跳过。");
                    }
                }
            }
        }

        /// <summary>
        /// 添加一定数量的材料到库存
        /// </summary>
        /// <param name="stuffName"></param>
        /// <param name="num"></param>
        public void AddStorage(string stuffName, int num)
        {
            if (StuffDefDictionary.TryGetValue(stuffName, out StuffDef def))
            {
                if (StoragesDictionary.TryGetValue(stuffName, out Stuff stuff))
                {
                    stuff.Num += num;
                }
                else
                {
                    stuff = new Stuff(def);
                    stuff.Num = num;
                    StoragesDictionary.Add(stuffName, stuff);
                }
            }
            else
            {
                Debug.Log("无此材料定义");
            }
        }

        /// <summary>
        /// 从库存取一定数量的材料
        /// </summary>
        /// <param name="stuffName"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool TryGetFromStorage(string stuffName, int num)
        {
            if (StoragesDictionary.TryGetValue(stuffName, out Stuff stuff))
            {
                if (stuff.Num < num)
                {
                    //材料不够
                    return false;
                }
                stuff.Num -= num;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}