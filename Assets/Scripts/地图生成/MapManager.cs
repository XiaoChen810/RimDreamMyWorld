using System.Collections.Generic;
using UnityEngine;

namespace 地图生成
{
    [RequireComponent(typeof(MapCreator))]
    [RequireComponent(typeof(PathFinder))]
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        // 地图生成器
        public MapCreator MapCreator { get; private set; }

        // 寻路算法器
        public PathFinder PathFinder { get; private set; }

        // 当前场景地图数据
        public class CurrentSceneMapData
        {
            public int width, height;
            public int seed;
            public MapCreator.TileData[,] mapTileDatas;
            public PathFinder.Node[,] nodes;
            public GameObject currentMap;

            public CurrentSceneMapData(int width, int height, MapCreator mapCreator, PathFinder pathFinder, Transform transform, string Name)
            {
                this.width = width;
                this.height = height;

                // 生成一个随机的地图，并初始化寻路算法的节点，并把数据保存起来
                seed = System.DateTime.Now.GetHashCode();
                mapTileDatas = mapCreator.GenerateMap(width, height, seed);
                nodes = pathFinder.InitNodes(width, height, mapTileDatas);
                currentMap = Instantiate(transform.Find("当前地图").gameObject, transform);
                currentMap.name = Name;
                currentMap.SetActive(true);
            }
        }

        // 不同场景的地图数据
        public Dictionary<string, CurrentSceneMapData> sceneMapDatasDict = new();

        // 游戏开始时生成的主地图的长宽
        public int mainMapWidth = 100;
        public int mainMapHeight = 100;

        public string CurrentMapName { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                Init();
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 初始化组件
            MapCreator = GetComponent<MapCreator>();
            PathFinder = GetComponent<PathFinder>();
        }

        /// <summary>
        /// 加载场景地图，若没有则会创建一个
        /// </summary>
        /// <param name="mapName"></param>
        public void LoadSceneMap(string mapName)
        {

            // 如果加载的是新场景要先把旧场景关了
            if (CurrentMapName != mapName && CurrentMapName != null)
            {
                CloseSceneMap(CurrentMapName);
            }

            if (sceneMapDatasDict.ContainsKey(mapName))
            {
                transform.Find(mapName).gameObject.SetActive(true);
            }
            else
            {
                MapDataDictAdd(mapName, mainMapWidth, mainMapHeight);
            }
            CurrentMapName = mapName;
        }

        /// <summary>
        ///  关闭场景
        /// </summary>
        /// <param name="mapName"></param>
        public void CloseSceneMap()
        {
            if (CurrentMapName != null)
            {
                transform.Find(CurrentMapName).gameObject.SetActive(false);
                CurrentMapName = null;
            }
        }
        public void CloseSceneMap(string mapName)
        {
            if (sceneMapDatasDict.ContainsKey(mapName))
            {
                transform.Find(mapName).gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("没有此场景");
                return;
            }
        }


        /// <summary>
        /// 生成并添加一份地图数据到sceneMapDatasDict。
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void MapDataDictAdd(string mapName, int width, int height)
        {
            CurrentSceneMapData mapData = new CurrentSceneMapData(width, height, MapCreator, PathFinder, transform, mapName);

            if (!sceneMapDatasDict.ContainsKey(mapName))
            {
                sceneMapDatasDict.Add(mapName, mapData);
                Debug.Log("已经生成地图" + mapName);
            }
        }

        /// <summary>
        /// 从某场景的地图数据中获取寻路路径
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public List<Vector2> GetPath(Vector3 startPos, Vector3 targetPos, string mapName)
        {
            if (!sceneMapDatasDict.ContainsKey(mapName))
            {
                Debug.LogWarning($"未能找到{mapName}的节点数据，已退出");
                return null;
            }
            else
            {
                return PathFinder.FindPath(startPos, targetPos, mapName);
            }
        }

        public GameObject GetChildObject(string Name)
        {
            if (CurrentMapName != null)
            {
                GameObject parent = transform.Find(CurrentMapName).gameObject;
                return FindChildRecursively(parent.transform, Name).gameObject;
            }

            Debug.LogWarning("当前没有存在的地图");
            return null;
        }


        Transform FindChildRecursively(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                return child;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform foundChild = FindChildRecursively(parent.GetChild(i), childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }
    }
}