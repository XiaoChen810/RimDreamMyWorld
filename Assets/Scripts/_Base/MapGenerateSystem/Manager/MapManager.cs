using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using ChenChen_BuildingSystem;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

namespace ChenChen_MapGenerator
{
    [RequireComponent(typeof(MapCreator))]
    public class MapManager : SingletonMono<MapManager>
    {
        /// <summary>
        ///  地图生成器
        /// </summary>
        public MapCreator MapCreator { get; private set; }

        /// <summary>
        ///  当前场景的地图名字
        /// </summary>
        public string CurrentMapName { get; private set; }

        /// <summary>
        /// 不同场景的地图数据
        /// </summary>
        public Dictionary<string, SceneMapData> SceneMapDatasDict = new();
        public SceneMapData CurrentSceneMapData;


        [Header("生成的主地图的长宽")]
        public int MapWidthOfGenerate = 100;
        public int MapHeightOfGenerate = 100;

        // 当地图障碍物发生变化
        public Action MapObstaclesChange;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Update()
        {
            if (CurrentMapName != null && SceneMapDatasDict.ContainsKey(CurrentMapName))
            {
                CurrentSceneMapData = SceneMapDatasDict[CurrentMapName];
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 初始化组件
            MapCreator = GetComponent<MapCreator>();
        }

        /// <summary>
        /// 生成并添加一份地图数据到SceneMapDatasDict。
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="mapSeed">是否使用随机种子，默认为随机</param>
        private void MapDataDictAdd(string mapName,int mapSeed = -1)
        {      
            Init();
            if (!SceneMapDatasDict.ContainsKey(mapName))
            {
                SceneMapData mapData = new();
                mapData.width = MapWidthOfGenerate;
                mapData.height = MapHeightOfGenerate;
                // 生成一个随机的地图
                mapData.seed = mapSeed == -1 ? System.DateTime.Now.GetHashCode() : mapSeed;
                mapData = MapCreator.GenerateMap(MapWidthOfGenerate, MapHeightOfGenerate, mapData);
                // 将地图数据用一个GameObject的形式保存
                mapData.mapObject = Instantiate(transform.Find("当前地图").gameObject, transform);
                mapData.mapObject.name = mapName;
                mapData.mapObject.SetActive(true);
                mapData.mainTilemap = mapData.mapObject.GetComponentInChildren<Tilemap>();
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
        public void LoadOrGenerateSceneMap(string mapName, int seed = -1)
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
                CurrentMapName = mapName;
                return;
            }

            MapDataDictAdd(mapName, seed);
            CurrentMapName = mapName;
            // 初始化寻路算法的节点
            FindAnyObjectByType<AstarPath>().Scan();
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
        /// 彻底删除场景
        /// </summary>
        /// <param name="mapName"></param>
        public void DeleteSceneMap(string mapName)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.Find(mapName).gameObject);
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                SceneMapDatasDict.Remove(mapName);
            }
#else
            Destroy(transform.Find(mapName).gameObject);
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                SceneMapDatasDict.Remove(mapName);
            }
#endif
        }

        /// <summary>
        /// 从某场景的地图数据中获取寻路路径
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endtPos"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
        [Obsolete]
        public List<Vector2> GetPath(Vector3 startPos, Vector3 endtPos, string mapName)
        {
            if (!SceneMapDatasDict.ContainsKey(mapName))
            {
                Debug.LogWarning($"未能找到{mapName}这个地图的节点数据，已退出");
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  获取当前地图的一个子物体
        /// </summary>
        /// <param name="Name"> 子物体名字 </param>
        /// <returns></returns>
        public GameObject GetChildObjectFromCurMap(string Name)
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
        /// 设置某地图某点的进入所需消耗
        /// </summary>
        /// <param name="mapName"> 地图的名字 </param>
        /// <param name="pos"> 坐标 </param>
        /// <param name="set"> </param>
        [Obsolete]
        public void SetNodeIntoCost(string mapName, Vector3Int pos,float set = FinderNode.s_MaxIntoCost)
        {
            if(SceneMapDatasDict.ContainsKey(mapName))
            {
                MapObstaclesChange?.Invoke();
            }
            else
            {
                Debug.Log("没有这个地图数据");
            }
        }

        /// <summary>
        /// 设置地图某位置有障碍
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="pos"></param>
        /// <param name="set"></param>
        public void AddToObstaclesList(string mapName, Vector3Int pos)
        {
            if (!SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos))
            {
                SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);
            }
        }

        /// <summary>
        /// 设置地图某位置有建筑物
        /// </summary>
        /// <param name="obj"></param>
        public void AddToObstaclesList(GameObject obj, string mapName = null, float set = FinderNode.s_MaxIntoCost)
        {
            mapName = mapName == null ? CurrentMapName : mapName;
            // 获取当前场景的地图数据
            SceneMapData currentMapData = SceneMapDatasDict[mapName];
            // 获取物体的世界边界
            Bounds bounds = obj.GetComponent<Collider2D>().bounds;
            // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
            Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
            Vector3 maxNum = bounds.max;
            if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
                && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
            {
                maxNum -= Vector3.one;
            }
            Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);
            // 遍历占据的格子并进行处理
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    // 在这里处理每个占据的格子
                    if (x > currentMapData.width || x < 0) continue;
                    if (y > currentMapData.height || y < 0) continue;

                    Vector3Int pos = new Vector3Int(x, y);
                    if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos)) continue;
                    SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);

                }
            }
        }

        /// <summary>
        /// 设置地图某位置已经没有障碍
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="pos"></param>
        /// <param name="set"></param>
        public void RemoveFromObstaclesList(string mapName, Vector3Int pos)
        {
            if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos))
            {
                SceneMapDatasDict[mapName].obstaclesPositionList.Remove(pos);
            }
        }

        /// <summary>
        /// 设置地图某位置已经没有障碍物
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mapName"></param>
        public void RemoveFromObstaclesList(GameObject obj, string mapName = null)
        {
            mapName = mapName == null ? CurrentMapName : mapName;
            // 获取当前场景的地图数据
            SceneMapData currentMapData = SceneMapDatasDict[mapName];
            // 获取物体的世界边界
            Bounds bounds = obj.GetComponent<Collider2D>().bounds;
            // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
            Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
            Vector3 maxNum = bounds.max;
            if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
                && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
            {
                maxNum -= Vector3.one;
            }
            Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);
            // 遍历占据的格子并进行处理
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    // 在这里处理每个占据的格子
                    if (x > currentMapData.width || x < 0) continue;
                    if (y > currentMapData.height || y < 0) continue;

                    Vector3Int pos = new Vector3Int(x, y);

                    if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos)) continue;
                    SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);
                }
            }
        }

        /// <summary>
        /// 检查物体能否放置在这个位置上
        /// </summary>
        /// <param name="obj">请确保物体上有Collider组件</param>
        /// <returns></returns>
        public bool CheckObjectWhetherCanPlaceOnHere(GameObject obj)
        {
            // 获取当前场景的地图数据
            SceneMapData currentMapData = SceneMapDatasDict[CurrentMapName];
            // 获取物体的世界边界
            Bounds bounds = obj.GetComponent<Collider2D>().bounds;
            // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
            Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
            Vector3 maxNum = bounds.max;
            if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
                && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
            {
                maxNum -= Vector3.one;
            }
            Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);

            // 遍历占据的格子并进行处理
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    // 在这里处理每个占据的格子
                    if (x > currentMapData.width || x < 0) return false;
                    if (y > currentMapData.height || y < 0) return false;
                    if (SceneMapDatasDict[CurrentMapName].obstaclesPositionList.Contains(new Vector3(x, y))) return false;
                }
            }

            return true;
        }

#endregion
    }
}