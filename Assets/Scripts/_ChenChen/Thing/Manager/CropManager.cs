using AYellowpaper.SerializedCollections;
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
        public SerializedDictionary<string, CropDef> CropDefDictionary = new();

        protected override void Awake()
        {
            base.Awake();
            LoadAllCropDefData();
        }

        private void LoadAllCropDefData()
        {
            // ��������CropDef��Դ
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