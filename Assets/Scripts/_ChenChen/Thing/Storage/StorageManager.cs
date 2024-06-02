using AYellowpaper.SerializedCollections;
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
            // ��������CropDef��Դ
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