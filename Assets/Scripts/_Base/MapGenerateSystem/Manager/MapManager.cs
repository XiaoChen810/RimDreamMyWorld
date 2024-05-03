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
        public Dictionary<string, MapData> MapDatasDict = new Dictionary<string, MapData>();

        private string _currentMapName;
        /// <summary>
        ///  当前场景的地图名字
        /// </summary>
        public string CurrentMapName
        {
            get
            {
                if (_currentMapName == null)
                {
                    return StaticDef.Map_Default_Name;
                }
                return _currentMapName;
            }
            private set
            {
                _currentMapName = value;
            }
        }

        [Header("生成的主地图的长宽")]
        public int MapWidthOfGenerate = 100;
        public int MapHeightOfGenerate = 100;

        protected override void Awake()
        {
            base.Awake();
            MapCreator = GetComponent<MapCreator>();
            ItemCreator = new ItemCreator();
            MapDatasDict = new Dictionary<string, MapData>();
        }

        /// <summary>
        /// 生成一份地图数据。数据会把保存进字典
        /// </summary>
        /// <param name="mapSave"> 生成地图用的数据 </param>
        /// <param name="mapObjectActive"> 生成的地图是否立即作为当前场景 </param>
        private void GenerateMap(Data_MapSave mapSave, bool mapObjectActive)
        {      
            if (!MapDatasDict.ContainsKey(mapSave.mapName))
            {
                MapData mapData = new(mapSave);
                mapData = MapCreator.GenerateMap(mapData);
                if (mapObjectActive)
                {
                    _currentMapName = mapData.mapName;
                    mapData.mapObject.SetActive(true);
                }
                else
                {
                    mapData.mapObject.SetActive(false);
                }
                // 添加进字典
                MapDatasDict.Add(mapSave.mapName, mapData);
                Debug.Log("已经生成地图" + mapSave.mapName);
            }
            else
            {
                Debug.Log("不会重复生成相同名字的地图");
            }
        }

        #region Public

        /// <summary>
        /// 加载现有的场景地图，若没有则会创建一个
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
            if (MapDatasDict.ContainsKey(mapName))
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
                GenerateMap(mapSave, true);
                // 生成环境
                for (int i = 0; i < 100; i++)
                {
                    Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, MapWidthOfGenerate), UnityEngine.Random.Range(0, MapHeightOfGenerate));
                    if (MapDatasDict[mapName].mapNodes[pos.x, pos.y].type == NodeType.grass)
                        ItemCreator.GenerateItem("常青树", pos, mapName);
                }
                // 保存
                PlayManager.Instance.MapSaveThisPlay = mapSave;
            }
            AstarPath.active.Scan();
        }

        /// <summary>
        /// 加载地图场景从存档中
        /// </summary>
        /// <param name="mapSave"></param>
        public void LoadSceneMapFromSave(Data_GameSave gameSave)
        {
            Data_MapSave mapSave = gameSave.SaveMap;
            GenerateMap(mapSave, false);
            CurrentMapName = gameSave.SaveMap.mapName;
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
            if (MapDatasDict.ContainsKey(mapName))
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
            if (MapDatasDict.ContainsKey(mapName))
            {
                MapDatasDict.Remove(mapName);
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
            result = MapCreator.GetTileamp(name, MapDatasDict[CurrentMapName].mapObject.transform.Find("Grid").gameObject);
            return result != null;
        }

        #endregion
    }
}