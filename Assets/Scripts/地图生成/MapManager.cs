using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapCreator))]
[RequireComponent(typeof(PathFinder))]
public class MapManager : MonoBehaviour 
{
    public static MapManager Instance;

    // 地图生成器
    private MapCreator mapCreator;

    // 寻路算法器
    private PathFinder pathFinder;

    // 当前场景地图数据
    public class CurrentSceneMapData
    {
        public int width, height;
        public int seed;     
        public MapCreator.MapTileData[,] mapTileDatas;
        public PathFinder.Node[,] nodes;

        public CurrentSceneMapData(int width, int height, MapCreator mapCreator, PathFinder pathFinder)
        {
            this.width = width;
            this.height = height;

            // 生成一个随机的地图，并初始化寻路算法的节点，并把数据保存起来
            this.seed = System.DateTime.Now.GetHashCode();
            this.mapTileDatas = mapCreator.GenerateMap(width, height, this.seed);
            this.nodes = pathFinder.InitNodes(width, height, this.mapTileDatas);
        }
    }

    // 不同场景的地图数据
    public Dictionary<string, CurrentSceneMapData> sceneMapDatasDict = new();

    // 游戏开始时生成的主地图的长宽
    public int mainMapWidth = 100;
    public int mainMapHeight = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 初始化组件
        mapCreator = GetComponent<MapCreator>();
        pathFinder = GetComponent<PathFinder>();

        MapDataDictAdd("mainMap", mainMapWidth, mainMapHeight);
    }

    /// <summary>
    /// 生成并添加一份地图数据到sceneMapDatasDict。
    /// </summary>
    /// <param name="mapName"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void MapDataDictAdd(string mapName,int width, int height)
    {
        CurrentSceneMapData mapData = new CurrentSceneMapData(width, height, mapCreator, pathFinder);

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
            return pathFinder.FindPath(startPos, targetPos, mapName);
        }
    }
}
