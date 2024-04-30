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
        /// 地图生成器
        /// </summary>
        public MapCreator MapCreator { get; private set; }

        /// <summary>
        /// 物品生成器
        /// </summary>
        public ItemCreator ItemCreator { get; private set; }

        /// <summary>
        /// 不同场景的地图数据
        /// </summary>
        public Dictionary<string, SceneMapData> SceneMapDatasDict = new Dictionary<string, SceneMapData>();


        private string _currentMapName;
        /// <summary>
        ///  当前场景的地图名字
        /// </summary>
        public string CurrentMapName
        {
            get
            {
                return _currentMapName;
            }
            private set
            {
                _currentMapName = value;
            }
        }

        public Tilemap CurMapMainTilemap
        {
            get
            {
                return SceneMapDatasDict[_currentMapName].mainTilemap;
            }
        }

        [Header("生成的主地图的长宽")]
        public int MapWidthOfGenerate = 100;
        public int MapHeightOfGenerate = 100;

        protected override void Awake()
        {
            base.Awake();
            Init();
            ItemCreator = new ItemCreator();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 初始化组件
            MapCreator = GetComponent<MapCreator>();
            SceneMapDatasDict = new Dictionary<string, SceneMapData>();
        }

        /// <summary>
        /// 生成并添加一份地图数据到SceneMapDatasDict。
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="mapSeed">是否使用随机种子，默认为随机</param>
        private void MapDataDictAdd(Data_MapSave mapSave)
        {      
            Init();
            if (!SceneMapDatasDict.ContainsKey(mapSave.mapName))
            {
                SceneMapData mapData = new(mapSave);
                mapData = MapCreator.GenerateMap(mapData);               
                // 添加进字典
                SceneMapDatasDict.Add(mapSave.mapName, mapData);
                Debug.Log("已经生成地图" + mapSave.mapName);
                // 保存
                PlayManager.Instance.SaveDate.MapSave = mapSave;
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
            if (_currentMapName != mapName && _currentMapName != null)
            {
                CloseSceneMap(_currentMapName);
            }

            // 如果已经存在场景，则直接启用
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                transform.Find(mapName).gameObject.SetActive(true);
                Debug.Log($"场景已经存在{mapName},直接启用");
            }
            else
            {
                Data_MapSave mapSave = new Data_MapSave(mapName,
                                        MapWidthOfGenerate,
                                        MapHeightOfGenerate,
                                        seed == -1 ? System.DateTime.Now.GetHashCode() : seed);
                MapDataDictAdd(mapSave);
                for (int i = 0; i < 100; i++)
                {
                    Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, MapWidthOfGenerate), UnityEngine.Random.Range(0, MapHeightOfGenerate));
                    if (SceneMapDatasDict[mapName].mapNodes[pos.x, pos.y].type == NodeType.grass)
                        ItemCreator.GenerateItem("常青树", pos, mapName, false);
                }
            }
            AstarPath.active.Scan();
            _currentMapName = mapName;
        }

        /// <summary>
        /// 加载地图场景从存档中
        /// </summary>
        /// <param name="mapSave"></param>
        public void LoadSceneMapFromSave(Data_GameSave gameSave)
        {
            Data_MapSave mapSave = gameSave.MapSave;
            MapDataDictAdd(mapSave);
            _currentMapName = mapSave.mapName;
            SceneMapDatasDict[_currentMapName].mapObject.SetActive(false);
            foreach(var thing in gameSave.Things)
            {
                ItemCreator.GenerateItem("常青树", thing.ThingPos, thing.MapName, true);                   
            }
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
        ///  获取当前地图的一个瓦片地图子物体
        /// </summary>
        /// <param name="Name"> 子物体名字 </param>
        /// <returns></returns>
        public GameObject GetChildObjectFromCurMap(string Name)
        {
            if (_currentMapName != null)
            {
                // 返回当前地图的所要找的子物体
                GameObject parent = transform.Find(_currentMapName).gameObject;
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

        ///// <summary>
        ///// 设置某地图某位置有障碍
        ///// </summary>
        ///// <param name="mapName"></param>
        ///// <param name="pos"></param>
        ///// <param name="set"></param>
        //public void AddToObstaclesList(Vector3Int pos, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    if (!SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos))
        //    {
        //        SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);
        //    }
        //}

        ///// <summary>
        ///// 设置某地图某位置有建筑物
        ///// </summary>
        ///// <param name="obj"></param>
        //public void AddToObstaclesList(GameObject obj, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    // 获取当前场景的地图数据
        //    SceneMapData currentMapData = SceneMapDatasDict[mapName];
        //    // 获取物体的世界边界
        //    Bounds bounds = obj.GetComponent<Collider2D>().bounds;
        //    // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
        //    Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
        //    Vector3 maxNum = bounds.max;
        //    if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
        //        && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
        //    {
        //        maxNum -= Vector3.one;
        //    }
        //    Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);
        //    // 遍历占据的格子并进行处理
        //    for (int x = minCell.x; x <= maxCell.x; x++)
        //    {
        //        for (int y = minCell.y; y <= maxCell.y; y++)
        //        {
        //            // 在这里处理每个占据的格子
        //            if (x > currentMapData.width || x < 0) continue;
        //            if (y > currentMapData.height || y < 0) continue;

        //            Vector3Int pos = new Vector3Int(x, y);
        //            if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos)) continue;
        //            SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 设置某地图某位置已经没有障碍
        ///// </summary>
        ///// <param name="mapName"></param>
        ///// <param name="pos"></param>
        ///// <param name="set"></param>
        //public void RemoveFromObstaclesList(Vector3Int pos, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos))
        //    {
        //        SceneMapDatasDict[mapName].obstaclesPositionList.Remove(pos);
        //    }
        //}

        ///// <summary>
        ///// 设置某地图某位置已经没有障碍物
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="mapName"></param>
        //public void RemoveFromObstaclesList(GameObject obj, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    // 获取当前场景的地图数据
        //    SceneMapData currentMapData = SceneMapDatasDict[mapName];
        //    // 获取物体的世界边界
        //    Bounds bounds = obj.GetComponent<Collider2D>().bounds;
        //    // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
        //    Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
        //    Vector3 maxNum = bounds.max;
        //    if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
        //        && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
        //    {
        //        maxNum -= Vector3.one;
        //    }
        //    Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);
        //    // 遍历占据的格子并进行处理
        //    for (int x = minCell.x; x <= maxCell.x; x++)
        //    {
        //        for (int y = minCell.y; y <= maxCell.y; y++)
        //        {
        //            // 在这里处理每个占据的格子
        //            if (x > currentMapData.width || x < 0) continue;
        //            if (y > currentMapData.height || y < 0) continue;

        //            Vector3Int pos = new Vector3Int(x, y);

        //            if (SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos)) continue;
        //            SceneMapDatasDict[mapName].obstaclesPositionList.Add(pos);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 检查某地图这个位置上是否有障碍物
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <param name="mapName"></param>
        ///// <returns></returns>
        //public bool ContainsObstaclesList(Vector3Int pos, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    return SceneMapDatasDict[mapName].obstaclesPositionList.Contains(pos);
        //}

        ///// <summary>
        ///// 检查某地图这个物体要放的位置上是否有障碍物
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="mapName"></param>
        ///// <returns></returns>
        //public bool ContainsObstaclesList(GameObject obj, string mapName = null)
        //{
        //    mapName = mapName == null ? _currentMapName : mapName;
        //    // 获取当前场景的地图数据
        //    SceneMapData currentMapData = SceneMapDatasDict[_currentMapName];
        //    // 获取物体的世界边界
        //    Bounds bounds = obj.GetComponent<Collider2D>().bounds;
        //    // 将物体的包围盒范围转换为Tilemap上的格子坐标范围
        //    Vector3Int minCell = currentMapData.mainTilemap.WorldToCell(bounds.min);
        //    Vector3 maxNum = bounds.max;
        //    if (Mathf.Approximately(maxNum.x, Mathf.Round(maxNum.x))
        //        && Mathf.Approximately(maxNum.y, Mathf.Round(maxNum.y)))
        //    {
        //        maxNum -= Vector3.one;
        //    }
        //    Vector3Int maxCell = currentMapData.mainTilemap.WorldToCell(maxNum);

        //    // 遍历占据的格子并进行处理
        //    for (int x = minCell.x; x <= maxCell.x; x++)
        //    {
        //        for (int y = minCell.y; y <= maxCell.y; y++)
        //        {
        //            // 在这里处理每个占据的格子
        //            if (x > currentMapData.width || x < 0) return false;
        //            if (y > currentMapData.height || y < 0) return false;
        //            if (SceneMapDatasDict[_currentMapName].obstaclesPositionList.Contains(new Vector3(x, y))) return false;
        //        }
        //    }

        //    return true;
        //}

        public Tilemap GetTilemap(string name)
        {
            return MapCreator.GetTileamp(name, SceneMapDatasDict[CurrentMapName].mapObject.transform.Find("Grid").gameObject);
        }

#endregion
    }
}