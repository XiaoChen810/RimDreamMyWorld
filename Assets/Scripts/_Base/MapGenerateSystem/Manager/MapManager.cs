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
                PlayManager.Instance.SaveData.SaveMap = mapSave;
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
                        ItemCreator.GenerateItem("常青树", pos, mapName);
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
            Data_MapSave mapSave = gameSave.SaveMap;
            MapDataDictAdd(mapSave);
            _currentMapName = mapSave.mapName;
            SceneMapDatasDict[_currentMapName].mapObject.SetActive(false);
            foreach(var thingSave in gameSave.SaveThings)
            {
                ItemCreator.GenerateItem(thingSave);                   
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

        public bool TryGetTilemap(string name,out Tilemap result)
        {
            result = MapCreator.GetTileamp(name, SceneMapDatasDict[CurrentMapName].mapObject.transform.Find("Grid").gameObject);
            return result != null;
        }

        public bool TryGetMapMain(string mapName, out Tilemap result)
        {
            result = null;
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                result = SceneMapDatasDict[mapName].mainTilemap;
            }
            return result != null;
        }

        #endregion
    }
}