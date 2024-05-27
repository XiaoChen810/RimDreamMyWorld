using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChenChen_Thing
{
    /// <summary>
    /// ����ȫ�����ﶨ��
    /// </summary>
    public class CropManager : SingletonMono<CropManager>
    {
        [Header("����ȫ�����ﶨ����ֵ�")]
        [SerializedDictionary("����", "���ﶨ��")]
        public SerializedDictionary<string, CropDef> CropDefDictionary;

        protected override void Awake()
        {
            base.Awake();
            LoadAllCropDefData();
        }

        private void LoadAllCropDefData()
        {
            CropDefDictionary = new SerializedDictionary<string, CropDef>();

            // ��ȡָ��·���µ�����CropDef�ļ�
            string[] CropDefDataFiles = AssetDatabase.FindAssets("t:CropDef", new[] { "Assets/Resources/Prefabs/CropDef" });

            foreach (var CropDefDataFile in CropDefDataFiles)
            {
                // ����GUID����ThingDef
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
        /// ������һ��Def
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
        /// �������ַ���һ��Def
        /// </summary>
        /// <param name="cropName"></param>
        /// <returns></returns>
        public CropDef GetCropDef(string cropName)
        {
            if (CropDefDictionary.ContainsKey(cropName))
            {
                return CropDefDictionary[cropName];
            }
            Debug.LogWarning($"û��{cropName}������ﶨ��");
            return null;
        }

        #endregion
    }
}