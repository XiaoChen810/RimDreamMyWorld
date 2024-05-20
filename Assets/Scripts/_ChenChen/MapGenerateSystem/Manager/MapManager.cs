using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using ChenChen_BuildingSystem;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using UnityEditor.U2D.Aseprite;
using Pathfinding.RVO;

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
        private Dictionary<string, MapData> _mapDatasDict = new Dictionary<string, MapData>();

        private string _currentMapName;
        /// <summary>
        /// 当前场景的地图名字
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

        /// <summary>
        /// 当前生成地图的参数
        /// </summary>
        public Data_MapSave CurMapSave { get; private set; }

        [Header("生成的主地图的长宽")]
        public int MapWidthOfGenerate = 100;
        public int MapHeightOfGenerate = 100;
        public int TreeNum;

        public int CurMapWidth
        {
            get 
            {
                if (_mapDatasDict.ContainsKey(CurrentMapName))
                   return _mapDatasDict[CurrentMapName].width;
                return MapWidthOfGenerate;
            }
        }
        public int CurMapHeight
        {
            get
            {
                if (_mapDatasDict.ContainsKey(CurrentMapName))
                    return _mapDatasDict[CurrentMapName].height;
                return MapHeightOfGenerate;
            }
        }
        public MapNode[,] CurMapNodes => _mapDatasDict[CurrentMapName].mapNodes;

        protected override void Awake()
        {
            base.Awake();
            MapCreator = GetComponent<MapCreator>();
            ItemCreator = new ItemCreator();
            _mapDatasDict = new Dictionary<string, MapData>();
        }

        /// <summary>
        /// 生成一份地图数据。数据会把保存进字典
        /// </summary>
        /// <param name="mapSave"> 生成地图用的数据 </param>
        /// <param name="mapObjectActive"> 生成的地图是否立即作为当前场景 </param>
        private void GenerateMap(Data_MapSave mapSave, bool mapObjectActive)
        {    
            if (!_mapDatasDict.ContainsKey(mapSave.mapName))
            {
                MapData mapData = new(mapSave);
                if (MapCreator == null) MapCreator = GetComponent<MapCreator>();
                mapData = MapCreator.GenerateMap(mapData);
                if (mapObjectActive)
                {
                    CurrentMapName = mapData.mapName;
                    mapData.mapObject.SetActive(true);
                }
                else
                {
                    mapData.mapObject.SetActive(false);
                }
                // 添加进字典
                _mapDatasDict.Add(mapSave.mapName, mapData);
                Debug.Log("已经生成地图" + mapSave.mapName);
            }
            else
            {
                Debug.Log("不会重复生成相同名字的地图");
            }
            CurMapSave = mapSave;
            CurrentMapName = mapSave.mapName;
        }

        #region Public

        /// <summary>
        /// 加载现有的场景地图，若没有则会创建一个
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
            if (_mapDatasDict.ContainsKey(mapName))
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
                //if(ItemCreator == null) ItemCreator = new ItemCreator();
                //// 生成树
                //for (int i = 0; i < TreeNum; i++)
                //{
                //    Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, MapWidthOfGenerate), UnityEngine.Random.Range(0, MapHeightOfGenerate));
                //    if (_mapDatasDict[mapName].mapNodes[pos.x, pos.y].type == NodeType.grass)
                //        ItemCreator.GenerateItem("常青树", pos, mapName);
                //}
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
            if (_mapDatasDict.ContainsKey(mapName))
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
            if (_mapDatasDict.ContainsKey(mapName))
            {
                _mapDatasDict.Remove(mapName);
            }
#else
            Destroy(transform.Find(mapName).gameObject);
            if (SceneMapDatasDict.ContainsKey(mapName))
            {
                SceneMapDatasDict.Remove(mapName);
            }
#endif
        }

        public bool TryGetTilemap(string name, bool isObstacle,out Tilemap result)
        {
            Transform grid = _mapDatasDict[CurrentMapName].mapObject.transform.Find("Grid");
            Transform tilemapTransform = grid.Find(name);
            if(tilemapTransform != null)
            {
                if(!tilemapTransform.TryGetComponent<Tilemap>(out result))
                {
                    Debug.LogError($"缺失Tilemap组件");
                }
            }
            else
            {
                Debug.Log($"未能找到对应的Tilemap，已重新生成了一个 : {name}");
                GameObject newObj = new GameObject(name);
                Tilemap tilemap = newObj.AddComponent<Tilemap>();
                newObj.AddComponent<TilemapRenderer>().sortingLayerName = "Above";
                if (isObstacle)
                {
                    newObj.AddComponent<TilemapCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
                    newObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    newObj.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
                    newObj.layer = 8; //Obstacle Layer
                }
                newObj.transform.parent = grid.transform;
                result = tilemap;
            }
            return result != null;
        }

        public MapNode GetMapNodeHere(Vector2 position)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            x = x < 0 ? 0 : x;
            x = x > MapWidthOfGenerate ? MapWidthOfGenerate : x;
            y = y < 0 ? 0 : y;
            y = y > MapHeightOfGenerate ? MapHeightOfGenerate : y;
            return _mapDatasDict[CurrentMapName].mapNodes[x, y];
        }

        #endregion
    }
}