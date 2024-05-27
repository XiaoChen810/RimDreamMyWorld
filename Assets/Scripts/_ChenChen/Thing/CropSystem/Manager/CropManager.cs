using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public SerializedDictionary<string, CropDef> CropDefDictionary;

        protected override void Awake()
        {
            base.Awake();
            LoadAllCropDefData();
        }

        private void LoadAllCropDefData()
        {
            CropDefDictionary = new SerializedDictionary<string, CropDef>();

            // 获取指定路径下的所有CropDef文件
            string[] CropDefDataFiles = AssetDatabase.FindAssets("t:CropDef", new[] { "Assets/Resources/Prefabs/CropDef" });

            foreach (var CropDefDataFile in CropDefDataFiles)
            {
                // 根据GUID加载ThingDef
                string CropDefDataAssetPath = AssetDatabase.GUIDToAssetPath(CropDefDataFile);
                CropDef CropDefData = AssetDatabase.LoadAssetAtPath<CropDef>(CropDefDataAssetPath);

                if (CropDefData != null)
                {
                    if (!CropDefDictionary.ContainsKey(CropDefData.CropName))
                    {
                        CropDefDictionary.Add(CropDefData.CropName, CropDefData);
                    }
                    else
                    {
                        Debug.LogWarning($"BlueprintData with name '{CropDefData.CropName}' already exists. Skipping.");
                    }
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