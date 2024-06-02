using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Map
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
        /// <param name="isSave"> 是存档 </param>
        private void GenerateMap(Data_MapSave mapSave, bool mapObjectActive, bool isSave)
        {    
            if (!_mapDatasDict.ContainsKey(mapSave.mapName))
            {
                MapData mapData = new(mapSave);
                if (MapCreator == null) MapCreator = GetComponent<MapCreator>();
                mapData = MapCreator.GenerateMap(mapData, isSave);
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
                GenerateMap(mapSave, true, false);
            }
            // 更新寻路
            if (AstarPath.active != null)
            {
                AstarPath.active.Scan();
                AstarPath.active.AddWorkItem(new AstarWorkItem(() =>
                {
                    foreach (var mapNode in CurMapNodes)
                    {
                        if (mapNode.type == NodeType.water)
                        {
                            SetNodePenalty((Vector2)mapNode.position, 1000);
                            SetSurroundingNodesPenalty((Vector2)mapNode.position, 1000);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// 设置特定位置节点的代价
        /// </summary>
        /// <param name="position">节点位置</param>
        /// <param name="penalty">代价值</param>
        private void SetNodePenalty(Vector2 position, uint penalty)
        {
            var aStarNode = AstarPath.active.GetNearest(position).node;
            if (aStarNode != null)
            {
                aStarNode.Penalty = penalty;
            }
        }

        /// <summary>
        /// 设置特定位置周围8个节点的代价
        /// </summary>
        /// <param name="position">中心节点位置</param>
        /// <param name="penalty">代价值</param>
        private void SetSurroundingNodesPenalty(Vector2 position, uint penalty)
        {
            Vector2[] value = {
                new Vector2(-1, -1),
                new Vector2(0, -1),
                new Vector2(1, -1),
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(-1, 1),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            Vector2[] directions = value;

            foreach (var direction in directions)
            {
                SetNodePenalty(position + direction, penalty);
            }
        }

        /// <summary>
        /// 加载地图场景从存档中
        /// </summary>
        /// <param name="mapSave"></param>
        public void LoadSceneMapFromSave(Data_GameSave gameSave)
        {
            Data_MapSave mapSave = gameSave.SaveMap;
            GenerateMap(mapSave, false, true);
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
            if (_mapDatasDict.ContainsKey(mapName))
            {
                _mapDatasDict.Remove(mapName);
            }
#endif
        }

        /// <summary>
        /// 在当前地图上获取Tilemap，如果不存在，则新生成一个
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isObstacle"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetTilemap(string name, bool isObstacle,out Tilemap result)
        {
            result = null;
            if (_mapDatasDict[CurrentMapName].mapObject == null) return false;
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
                TilemapRenderer tr = newObj.AddComponent<TilemapRenderer>();
                tr.sortingLayerName = "Bottom";
                // 默认光照材质Sprite-Lit-Default，Assets/Resources/Materials/Sprite-Lit-Default.mat
                tr.material = Resources.Load<Material>("Materials/Sprite-Lit-Default");
                if (isObstacle)
                {
                    newObj.AddComponent<TilemapCollider2D>().usedByComposite = true;
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

            // 确保 x 在有效范围内
            if (x < 0) x = 0;
            if (x >= CurMapWidth) x = CurMapWidth - 1;

            // 确保 y 在有效范围内
            if (y < 0) y = 0;
            if (y >= CurMapHeight) y = CurMapHeight - 1;

            return _mapDatasDict[CurrentMapName].mapNodes[x, y];
        }


        #endregion
    }
}