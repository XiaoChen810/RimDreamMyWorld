using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace ChenChen_Thing
{
    /// <summary>
    /// 管理全部作物定义
    /// </summary>
    public class CropManager : SingletonMono<CropManager>
    {
        [Header("包含全部作物定义的字典")]
        [SerializedDictionary("名称", "作物定义")]
        public SerializedDictionary<string, CropDef> CropDefDictionary = new();

        protected override void Awake()
        {
            base.Awake();
            LoadAllCropDefData();
        }

        private void LoadAllCropDefData()
        {
            // 加载所有CropDef资源
            CropDef[] defs = Resources.LoadAll<CropDef>("Prefabs/CropDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!CropDefDictionary.ContainsKey(def.CropName))
                    {
                        CropDefDictionary.Add(def.CropName, def);
                    }
                    else
                    {
                        Debug.LogWarning($"CropDef with name '{def.CropName}' already exists. Skipping.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load CropDef.");
                }
            }
        }

        #region Public

        /// <summary>
        /// 随机获得一个Def
        /// </summary>
        /// <returns></returns>
        public CropDef GetCropDef()
        {
            int index = Random.Range(0, CropDefDictionary.Count);
            if (index < 0) return null;
            foreach (var CropDef in CropDefDictionary)
            {
                index--;
                if (index < 0) return CropDef.Value;
            }
            return null;

        }

        /// <summary>
        /// 根据名字返回一个Def
        /// </summary>
        /// <param name="cropName"></param>
        /// <returns></returns>
        public CropDef GetCropDef(string cropName)
        {
            if (CropDefDictionary.ContainsKey(cropName))
            {
                return CropDefDictionary[cropName];
            }
            Debug.LogWarning($"没有{cropName}这个作物定义");
            return null;
        }

        #endregion
    }
}