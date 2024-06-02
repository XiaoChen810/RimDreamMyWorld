using AYellowpaper.SerializedCollections;
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
            // 加载所有CropDef资源
            StuffDef[] defs = Resources.LoadAll<StuffDef>("Prefabs/StuffDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!StuffDefDictionary.ContainsKey(def.Name))
                    {
                        StuffDefDictionary.Add(def.Name, def);
                    }
                    else
                    {
                        Debug.LogWarning($"StuffDef with name '{def.Name}' already exists. Skipping.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load CropStuffDefDef.");
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