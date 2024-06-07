using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.IO;
using ChenChen_Map;
using System;

namespace ChenChen_Thing
{
    /// <summary>
    /// ��¼ȫ����ͼ
    /// </summary>
    public class ThingSystemManager : SingletonMono<ThingSystemManager>
    {
        [Header("����ȫ����Ʒ������ֵ�")]
        [SerializedDictionary("����", "��Ʒ����")]
        public SerializedDictionary<string, ThingDef> ThingDefDictionary = new SerializedDictionary<string, ThingDef>();

        private Dictionary<Vector2, Thing_Tree> ThingDict_Tree = new();
        private Dictionary<string, LinkedList<ThingBase>> ThingDict = new();


        private BuildingModeTool tool;
        public BuildingModeTool Tool // ������Ʒ�Ľ���
        {
            get
            {
                if (tool == null)
                {
                    tool = new BuildingModeTool(this);
                }
                return tool;
            }
        }

        private Quadtree quadtree;   // ���������Ĳ���
        public Quadtree Quadtree
        {
            get
            {
                if (quadtree == null)
                {
                    Rect worldBounds = new Rect(0, 0, MapManager.Instance.CurMapWidth, MapManager.Instance.CurMapHeight);
                    quadtree = new Quadtree(0, worldBounds, this.gameObject, "root");
                }
                return quadtree;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            LoadAllThingDefData();
        }

        public void Update()
        {
            Tool.BuildUpdate();
        }

        private void LoadAllThingDefData()
        {
            // ��������ThingDef��Դ
            ThingDef[] defs = Resources.LoadAll<ThingDef>("Prefabs/ThingDef");

            foreach (var def in defs)
            {
                if (def != null)
                {
                    if (!ThingDefDictionary.ContainsKey(def.DefName))
                    {
                        ThingDefDictionary.Add(def.DefName, def);
                    }
                    else
                    {
                        Debug.LogWarning($"ThingDef with name '{def.DefName}' already exists. Skipping.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load ThingDef.");
                }
            }
        }

        #region Public
        /// <summary>
        /// �򿪽���ģʽ��ͨ��name������ͼ
        /// </summary>
        /// <param name="name"></param>
        public void OpenBuildingMode(string name)
        {
            Tool.BuildStart(GetThingDef(name));
        }
        /// <summary>
        /// ��������ӵ�����б�
        /// </summary>
        /// <param name="obj"></param>
        public void AddThingToList(GameObject obj)
        {
            if (obj.TryGetComponent<ThingBase>(out var thing))
            {
                switch (thing.Def.Type)
                {
                    case ThingType.Tree:
                        if (!ThingDict_Tree.ContainsKey(obj.transform.position))
                        {
                            ThingDict_Tree.Add(obj.transform.position, thing.GetComponent<Thing_Tree>());
                        }
                        else
                        {
                            Debug.LogWarning("��ͬ�ļ������");
                        }

                        break;
                    default:
                        if (ThingDict.ContainsKey(thing.Def.DefName))
                        {
                            ThingDict[thing.Def.DefName].AddFirst(thing);
                        }
                        else
                        {
                            ThingDict.Add(thing.Def.DefName, new LinkedList<ThingBase>());
                            ThingDict[thing.Def.DefName].AddFirst(thing);
                        }
                        break;
                }

                if(Tool.OnBuildMode && Tool._mouseIndicator == obj)
                {
                    Debug.Log("����ӽ���Ԥ��ָʾ��");
                }
                else
                {
                    Quadtree.Insert(obj);
                }
            }
            else
            {
                Debug.LogWarning("��⵽û�� ThingBase �������������ӽ��б� ���ѷ���");
                return;
            }
        }        
        /// <summary>
        /// �������Ƴ�
        /// </summary>
        /// <param name="obj">Ҫ�Ƴ�������</param>
        public void RemoveThing(GameObject obj)
        {
            // ��鴫��Ķ����Ƿ�Ϊnull
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "��������岻��Ϊ��");
            }

            // ���Ի�ȡ ThingBase ���
            if (!obj.TryGetComponent<ThingBase>(out var thing))
            {
                Debug.LogWarning($"{obj} û�� <ThingBase> ���");
                return;
            }

            // ���� ThingBase �������Ƴ���Ӧ������
            switch (thing.Def.Type)
            {
                case ThingType.Tree:
                    // ����Ƿ�ɹ��Ƴ�
                    if (!ThingDict_Tree.Remove(obj.transform.position))
                    {
                        Debug.LogWarning($"�Ƴ�ʧ�ܣ��� ThingDict_Tree ���Ҳ���λ��Ϊ {obj.transform.position} ������");
                        return;
                    }
                    break;
                default:
                    // ����ֵ����Ƿ������������
                    if (!ThingDict.ContainsKey(obj.name))
                    {
                        Debug.LogWarning($"�Ƴ�ʧ�ܣ��� ThingDict ���Ҳ�����Ϊ {obj.name} ����Ŀ");
                        return;
                    }

                    // ����Ƿ�ɹ��Ƴ�
                    if (!ThingDict[obj.name].Remove(thing))
                    {
                        Debug.LogWarning($"�Ƴ�ʧ�ܣ��� ThingDict ���Ҳ�������Ϊ {obj.name} ������");
                        return;
                    }
                    break;
            }
        }

        #region ��ȡ����ʵ��
        /// <summary>
        /// ��ȡ����ʵ��
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needFree"> �Ƿ���Ҫһ��û�б�ʹ�õ� </param>
        /// <returns></returns>
        public GameObject GetThingInstance(string name, bool needFree = true)
        {
            if (!ThingDict.ContainsKey(name))
            {
                Debug.LogWarning($"ThingDict �в����ڼ�Ϊ {name} ����Ŀ");
            }
            else
            {
                foreach (var thing in ThingDict[name])
                {
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// ��ȡ����ʵ��
        /// </summary>
        /// <param name="lifeState"> ���崦��ʲô�׶� </param>
        /// <param name="name"></param>
        /// <param name="needFree"> �Ƿ���Ҫһ��û�б�ʹ�õ� </param>
        /// <returns></returns>
        public GameObject GetThingInstance(BuildingLifeStateType lifeState, string name = null, bool needFree = true)
        {
            if (name != null && ThingDict.ContainsKey(name))
            {
                Debug.LogWarning($"ThingDict �в����ڼ�Ϊ {name} ����Ŀ");
                foreach (var thing in ThingDict[name])
                {
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }           
            foreach (var thinglist in ThingDict)
            {
                foreach (var thing in thinglist.Value)
                {
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    return thing.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// ��ȡȫ������ʵ��
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="needFree">�Ƿ���Ҫһ��û�б�ʹ�õ����壨Ĭ��ֵΪ true��</param>
        /// <returns>���������������б�</returns>
        public List<T> GetThingsInstance<T>(BuildingLifeStateType lifeState = BuildingLifeStateType.None, bool needFree = true) where T : ThingBase
        {
            List<T> list = new List<T>();
            foreach (var thinglist in ThingDict)
            {
                foreach (var thing in thinglist.Value)
                {
                    if (thing is not T) continue;
                    if (thing.LifeState != lifeState) continue;
                    if (needFree && (thing.TheUsingPawn != null || thing.Permission != PermissionBase.PermissionType.IsFree)) continue;
                    list.Add(thing as T);
                }
            }
            return list;
        }
        /// <summary>
        /// ��ȡȫ������ʵ��
        /// </summary>
        /// <returns></returns>
        public List<ThingBase> GetAllThingsInstance()
        {
            List<ThingBase> list = new();
            foreach (var thingList in ThingDict)
            {
                foreach(var thing in thingList.Value)
                {
                    list.Add(thing as ThingBase);
                }
            }
            foreach (var thing in ThingDict_Tree)
            {
                list.Add(thing.Value as ThingBase);
            }
            return list;
        }
        /// <summary>
        /// ��ȡһ��Ҫ������
        /// </summary>
        /// <returns></returns>
        public GameObject GetTreeToCut()
        {
            foreach (var tree in ThingDict_Tree)
            {
                if (tree.Value.IsMarkCut)
                {
                    return tree.Value.gameObject;
                }
            }
            return null;
        }
        /// <summary>
        /// �����ֵ䣬�ҵ����ڵ���Ʒ���岢����
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
                    string folderPath = "Assets/Resources/Prefabs/ThingDef"; // �����ļ���·��
                    string fileName = $"{def.DefName}_Prefab.prefab"; // �ļ���

                    string filePath = FindFileInFolder(folderPath, fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        // ���ļ�·������ȡ���·���Ա��� Resources.Load
                        string resourcePath = filePath.Substring(filePath.IndexOf("Resources/") + 10); // ȥ�� "Resources/" ����չ�� ".prefab"
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
                    Debug.LogError($"������һ������ {name}�������Ԥ�Ƽ�Ϊ��");
                    return ThingDefDictionary[name];
                }
                return ThingDefDictionary[name];

                string FindFileInFolder(string folderPath, string fileName)
                {
                    // �ݹ������ļ���
                    foreach (string file in Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file) == fileName)
                        {
                            return file;
                        }
                    }
                    return null; // δ�ҵ��ļ�������null
                }
            }

            Debug.LogWarning($"δ���ҵ�{name}�Ķ���");
            return null;
        }
        #endregion

        #region ��������һ������
        // �Ӵ浵
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
                Debug.LogError($"���� {thingDef.DefName}�����Ԥ�Ƽ�Ϊ��");
                return false;
            }
            GameObject obj = Generate(thingDef, thingSave.ThingPos, thingSave.ThingRot);
            obj.GetComponent<ThingBase>().ChangeLifeState(thingSave.LifeState);
            return true;
        }
        // ֱ��ʹ�ö���
        public bool TryGenerateThing(ThingDef thingDef, Vector2 position, Quaternion routation)
        {
            GameObject obj = Generate(thingDef, position + thingDef.Offset, routation);
            obj.GetComponent<ThingBase>().Building();
            return true;
        }
        // ֱ��ʹ������
        public bool TryGenerateThing(string thingName, Vector2 position, Quaternion routation)
        {
            ThingDef thingDef = GetThingDef(thingName);
            GameObject obj = Generate(thingDef, position + thingDef.Offset, routation);
            obj.GetComponent<ThingBase>().Building();
            return true;
        }
        // ����
        private GameObject Generate(ThingDef thingDef, Vector2 position, Quaternion routation)
        {
            GameObject obj = Instantiate(thingDef.Prefab, position, routation, transform);
            AddThingToList(obj);
            if (GameManager.Instance.GameIsStart) AudioManager.Instance.PlaySFX("Placed");
            return obj;
        }
        #endregion

        #endregion
    }
}