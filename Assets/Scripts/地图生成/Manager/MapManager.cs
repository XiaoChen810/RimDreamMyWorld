using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using MyBuildingSystem;
using UnityEngine.Tilemaps;

namespace MyMapGenerate
{
    [RequireComponent(typeof(MapCreator))]
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        /// <summary>
        ///  地图生成器
        /// </summary>
        public MapCreator MapCreator { get; private set; }

        /// <summary>
        ///  寻路算法器
        /// </summary>
        public PathFinder PathFinder { get; private set; }

        /// <summary>
        ///  当前场景的地图名字
        /// </summary>
        public string CurrentMapName { get; private set; }

        /// <summary>
        /// 场景地图数据
        /// </summary>
        public class SceneMapData
        {
            public int width, height;
            public int numberUnitGrid;
            public int seed;
            public MapCreator.TileData[,] mapTileDatas;
            public PathFinder.Node[,] nodes;
            public GameObject currentMap;
            public Tilemap mainTilemap;
        }

        /// <summary>
        /// 不同场景的地图数据
        /// </summary>
        public Dictionary<string, SceneMapData> SceneMapDatasDict = new();

        [Header("生成的主地图的长宽")]
        public int MapWidthOfGenerate = 100;
        public int MapHeightOfGenerate = 100;

        // 寻路器的单位网格数量,有点问题 
        public int NumberUnitGrid = 1;

        // 当地图障碍物发生变化
        public Action MapObstaclesChange;

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
            PathFinder = new PathFinder(MapWidthOfGenerate, MapHeightOfGenerate, NumberUnitGrid);
        }

        /// <summary>
        /// 生成并添加一份地图数据到SceneMapDatasDict。
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="mapSeed">是否使用随机种子，默认为随机</param>
        private void MapDataDictAdd(string mapName,int mapSeed = -1)
        {                          
            if (!SceneMapDatasDict.ContainsKey(mapName))
            {
                SceneMapData mapData = new();
                mapData.width = MapWidthOfGenerate;
                mapData.height = MapHeightOfGenerate;
                // 生成一个随机的地图
                mapData.seed = mapSeed == -1 ? System.DateTime.Now.GetHashCode() : mapSeed;
                mapData.mapTileDatas = MapCreator.GenerateMap(MapWidthOfGenerate, MapHeightOfGenerate, mapData.seed);
                // 初始化寻路算法的节点
                mapData.nodes = PathFinder.InitNodes(MapWidthOfGenerate, MapHeightOfGenerate, NumberUnitGrid, mapData.mapTileDatas);
                // 将地图数据用一个GameObject的形式保存
                mapData.currentMap = Instantiate(transform.Find("当前地图").gameObject, transform);
                mapData.currentMap.name = mapName;
                mapData.currentMap.SetActive(true);
                mapData.mainTilemap = mapData.currentMap.GetComponentInChildren<Tilemap>();
                // 添加进字典
                SceneMapDatasDict.Add(mapName, mapData);
                Debug.Log("已经生成地图" + mapName);
            }
            else
            {
                Debug.Log("不会重复生成相同名字的地图");
            }
        }

        #region Public

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
            // 如果已经存在场景，则直接启用
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                transform.Find(mapName).gameObject.SetActive(true);
            }
            else
            {
                MapDataDictAdd(mapName);
            }
            CurrentMapName = mapName;
        }

        /// <summary>
        ///  关闭场景
        /// </summary>
        /// <param name="mapName"></param>
        public void CloseSceneMap(string mapName)
        {
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                transform.Find(mapName).gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("没有此场景却依然想关闭");
                return;
            }
        }

        /// <summary>
        /// 从某场景的地图数据中获取寻路路径
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endtPos"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public List<Vector2> GetPath(Vector3 startPos, Vector3 endtPos, string mapName)
        {
            if (!SceneMapDatasDict.ContainsKey(mapName))
            {
                Debug.LogWarning($"未能找到{mapName}这个地图的节点数据，已退出");
                return null;
            }
            else
            {
                return PathFinder.FindPath(startPos, endtPos, SceneMapDatasDict[mapName].nodes);
            }
        }

        /// <summary>
        ///  获取当前地图的一个子物体
        /// </summary>
        /// <param name="Name"> 子物体名字 </param>
        /// <returns></returns>
        public GameObject GetChildObject(string Name)
        {
            if (CurrentMapName != null)
            {
                // 返回当前地图的所要找的子物体
                GameObject parent = transform.Find(CurrentMapName).gameObject;
                return FindChildRecursively(parent.transform, Name).gameObject;
            }

            Debug.LogWarning("当前没有存在的地图");
            return null;

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

        /// <summary>
        /// 设置寻路节点是否可以行走
        /// </summary>
        /// <param name="mapName"> 地图的名字 </param>
        /// <param name="pos"> 坐标 </param>
        /// <param name="set"> 默认为false </param>
        public void SetNodeWalkable(string mapName, Vector3Int pos,bool set = false)
        {
            if(SceneMapDatasDict.ContainsKey(mapName))
            {
                //int unit = SceneMapDatasDict[mapName].numberUnitGrid;
                //SceneMapDatasDict[mapName].nodes[pos.x * unit,pos.y * unit].walkable = set;
                PathFinder.SetNodeWalkable(SceneMapDatasDict[mapName].nodes, pos, set);
                MapObstaclesChange?.Invoke();
            }
            else
            {
                Debug.Log("没有这个地图数据");
            }
        }

        /// <summary>
        /// 设置地图上方已经有建筑物
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="pos"></param>
        /// <param name="set"></param>
        public void SetMapDataWalkable(string mapName, Vector3Int pos, bool set = false)
        {
            SceneMapDatasDict[mapName].mapTileDatas[pos.x, pos.y].walkAble = set;
        }

        /// <summary>
        /// 检查蓝图能否放置在这个位置
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool CheckBlueprintCanPlaced(BlueprintData blueprint, Vector2 pos)
        {
            int x = blueprint.BuildingWidth;
            int y = blueprint.BuildingHeight;

            for (int i = -x / 2; i <= x / 2; i++)
            {
                for (int j = -y / 2; j <= y / 2; j++)
                {
                    SceneMapData currentMapData = SceneMapDatasDict[CurrentMapName];
                    MapCreator.TileData[,] tileDatas = currentMapData.mapTileDatas;
                    if (pos.x > currentMapData.width || pos.x < 0) return false;
                    if (pos.y > currentMapData.height || pos.y < 0) return false;
                    Vector3Int cellPos = currentMapData.mainTilemap.WorldToCell(pos);
                    if (tileDatas[cellPos.x, cellPos.y].walkAble == false) return false;
                }
            }

            return true;
        }

        #endregion
    }
}