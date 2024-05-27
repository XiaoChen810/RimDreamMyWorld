using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

namespace ChenChen_Thing
{
    /// <summary>
    /// �洢ϵͳ
    /// </summary>
    public class StorageManager : SingletonMono<StorageManager>
    {
        [Header("����ȫ����Ʒ������ֵ�")]
        [SerializedDictionary("����", "���϶���")]
        public SerializedDictionary<string, StuffDef> StuffDefDictionary = new();

        [Header("����б�")]
        [SerializedDictionary("����", "����")]
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
                AddStorage("С��", UnityEngine.Random.Range(1, 10));
                AddStorage("����", UnityEngine.Random.Range(1, 10));
            }
        }

        private void LoadAllStuffDefData()
        {
            StuffDefDictionary = new SerializedDictionary<string, StuffDef>();

            // ��ȡָ��·���µ�����ThingDef�ļ�
            string[] DefDataFiles = AssetDatabase.FindAssets("t:StuffDef", new[] { "Assets/Resources/Prefabs/StuffDef" });

            foreach (var def in DefDataFiles)
            {
                // ����GUID����Def
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
                        Debug.LogWarning($"���϶��� '{stuffDef.Name}' �ظ����ء� ������");
                    }
                }
            }
        }

        /// <summary>
        /// ���һ�������Ĳ��ϵ����
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
                Debug.Log("�޴˲��϶���");
            }
        }

        /// <summary>
        /// �ӿ��ȡһ�������Ĳ���
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
                    //���ϲ���
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