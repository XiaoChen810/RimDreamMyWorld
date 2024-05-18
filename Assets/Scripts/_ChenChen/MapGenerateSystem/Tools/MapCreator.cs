using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor.U2D.Aseprite;


namespace ChenChen_MapGenerator
{
    public class MapCreator : MonoBehaviour
    {
        [Header("生成器基本信息")]
        public TileBase mainMapDefaultTileBase;
        public float lacunarrty;

        // 生成地图时用到的全局数据
        private int _width;
        private int _height;
        private MapNode[,] _nodes;
        private GameObject _mapObj;
        private Dictionary<string, Tilemap> _layerDict; 

        [Header("地形数据")]
        [SerializeField] private List<TerrainData> _terrainList;
        [Header("花草数据")]
        [SerializeField] private List<FlowerData> _flowersList;

        /// <summary>
        /// 生成地图，并把返回对应的地图GameObject
        /// </summary>
        public MapData GenerateMap(MapData mapData)
        {
            MapData result = mapData;
            // 全局参数
            _width = mapData.width;
            _height = mapData.height;
            _nodes = new MapNode[_width, _height];
            Random.InitState(mapData.seed);

            if (LoadMapPrefab(mapData, out GameObject mapObj))
            {
                InitNodeNoiseValue();
                InitNodeType();
                CheckNode();
                result.mapNodes = _nodes;
                result.mapObject = mapObj;
                return result;
            }

            Debug.Log("第一次生成地图" + mapData.mapName);
            // 地图的Object
            _mapObj = new GameObject(mapData.mapName);
            _mapObj.transform.parent = transform;
            // 添加Grid组件
            GameObject grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
            grid.transform.parent = _mapObj.transform;
            // 添加Terrain相关的Tilemap
            _layerDict = new Dictionary<string, Tilemap>();
            foreach (var t in _terrainList)
            {
                GameObject newObj = new GameObject(t.tilemapName);
                Tilemap tilemap = newObj.AddComponent<Tilemap>();
                newObj.AddComponent<TilemapRenderer>().sortingOrder = t.layerSort;
                newObj.transform.parent = grid.transform;
                if (t.isObstacle)
                {
                    newObj.AddComponent<TilemapCollider2D>().compositeOperation = Collider2D.CompositeOperation.Merge;
                    newObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    newObj.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
                    newObj.layer = (t.type == NodeType.water) ? 4 : 8; //Water Layer or Obstacle Layer
                }
                if (!_layerDict.ContainsKey(t.tilemapName))
                {
                    _layerDict.Add(t.tilemapName, tilemap);
                }
                if (t.type == NodeType.water)
                {
                    newObj.tag = "Water";
                }
            }
            // 添加Flower相关的Tilemap
            foreach (var f in _flowersList)
            {
                GameObject newObj = new GameObject(f.tilemapName);
                Tilemap tilemap = newObj.AddComponent<Tilemap>();
                newObj.AddComponent<TilemapRenderer>().sortingOrder = f.layerSort;
                newObj.transform.parent = grid.transform;
                if (!_layerDict.ContainsKey(f.tilemapName)) _layerDict.Add(f.tilemapName, tilemap);
            }

            InitNodeNoiseValue();
            InitNodeType();
            CheckNode();
            GenerateTileMap();
            GenerateFlowers();

            result.mapNodes = _nodes;
            result.mapObject = _mapObj;

            SaveMapPrefab(mapData);
            return result;
        }

        private void SaveMapPrefab(MapData mapData)
        {
            string saveDirectory = "Assets/SavedGameObjects/";
            string path = "Assets/SavedGameObjects/" + mapData.mapName + mapData.seed + ".prefab";
            if (!string.IsNullOrEmpty(path))
            {
                //创建目录如果它不存在
                AssetDatabase.Refresh();
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                PrefabUtility.SaveAsPrefabAsset(_mapObj, path);
                // 刷新资产数据库
                AssetDatabase.Refresh();

                Debug.Log("GameObject saved as Prefab: " + path);
            }
        }

        public bool LoadMapPrefab(MapData mapData, out GameObject map)
        {
            map = null;
            string prefabPath = "Assets/SavedGameObjects/" + mapData.mapName + mapData.seed + ".prefab";
            if (string.IsNullOrEmpty(prefabPath))
                return false;

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                map = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                map.name = mapData.mapName;
                map.transform.parent = transform;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("GameObject Prefab loaded: " + prefabPath);
                return true;
            }
            else
            {
                Debug.LogWarning("Prefab not found at path: " + prefabPath);
                return false;
            }
        }

        #region 地图生成

        /// <summary>
        /// 设置地图噪声值
        /// </summary>
        private void InitNodeNoiseValue()
        {
            float randomOffset = Random.Range(-1000, 1000);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                    _nodes[x, y] = new MapNode(new Vector2Int(x, y), nosieValue);

                    if (nosieValue < minValue) minValue = nosieValue;
                    if (nosieValue > maxValue) maxValue = nosieValue;
                }
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _nodes[x, y].noiseValue = Mathf.Lerp(minValue, maxValue, _nodes[x, y].noiseValue);
                }
            }
        }

        /// <summary>
        /// 设置瓦片类型
        /// </summary>
        private void InitNodeType()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    foreach (var t in _terrainList)
                    {
                        if (_nodes[x, y].noiseValue <= t.probability)
                        {
                            _nodes[x, y].type = t.type;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 最终的生成瓦片地图，给对应的地图设置瓦片
        /// </summary>
        private void GenerateTileMap()
        {
            Tilemap mainMap = GetTileamp("Grass");
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {                   
                    mainMap.SetTile(new Vector3Int(x, y), mainMapDefaultTileBase);
                    TileBase tile = GetTileBase(_nodes[x, y]);
                    if (tile != null)
                    {
                        GetTileamp(_nodes[x,y]).SetTile(new Vector3Int(x, y), tile);
                    }
                }
            }
        }

        /// <summary>
        /// 生成花草
        /// </summary>
        private void GenerateFlowers()
        {
            // 统计有多少块草地
            int count = 0;
            List<MapNode> theGrassTile = new List<MapNode>();
            foreach (var md in _nodes)
            {
                if (md.type == NodeType.grass)
                {
                    theGrassTile.Add(md);
                    count++;
                }
            }

            foreach(var flower in _flowersList)
            {
                for(int i = 0;i<count * flower.probability; i++)
                {
                    if (theGrassTile.Count == 0) return;
                    int randomIndex = Random.Range(0, theGrassTile.Count);
                    Vector3Int pos = new Vector3Int(theGrassTile[randomIndex].postion.x, theGrassTile[randomIndex].postion.y);
                    TileBase tile = flower.tile[Random.Range(0, flower.tile.Count)];
                    GetTileamp(flower.tilemapName).SetTile(pos, tile);
                }
            }
        }

        private TileBase GetTileBase(MapNode node)
        {
            foreach (var t in _terrainList)
            {
                if (t.type == node.type) return t.tile;
            }
            return null;
        }

        private Tilemap GetTileamp(MapNode node)
        {
            foreach (var t in _terrainList)
            {
                if (t.type == node.type)
                {
                    //return _mapObj.transform.Find(t.tilemapName).GetComponent<Tilemap>();
                    return _layerDict[t.tilemapName];
                }
            }
            Debug.Log($"未能找到对应的Tilemap : {node.postion.x}, {node.postion.y}");
            return null;
        }

        /// <summary>
        /// 获取已经生成的Tilemap，如果没有则新生成一个，参数parent作为其父类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Tilemap GetTileamp(string name, GameObject parent = null, bool isObstacle = true)
        {
            if(_layerDict != null && _layerDict.ContainsKey(name))
            {
                return _layerDict[name];
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
                newObj.transform.parent = parent.transform;
                _layerDict.Add(name, tilemap);
                return tilemap;
            }
        }

        #endregion

        #region 检查地图数据，剔除不合理内容

        /// <summary>
        /// 剔除掉一些单独突出的数据，防止穿帮
        /// </summary>
        private void CheckNode()
        {
            bool flag = false;
            for (int i = 0; i < 100; i++)
            {
                flag = true;
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        if (!CheckTileMapDataIndividual(x, y, NodeType.water, NodeType.none)) flag = false;
                        if (!CheckTileMapDataIndividual(x, y, NodeType.ground, NodeType.none)) flag = false;
                        if (!CheckTileMapDataOutline(x, y, NodeType.ground, NodeType.water, NodeType.none)) flag = false;
                    }
                }
                if (flag)
                {
                    //Debug.Log("CheckFinish");
                    return;
                }
            }
            Debug.LogError("ERROR");
        }

        /// <summary>
        /// 剔除单个的瓦片，剔除只有一块突出的瓦片，剔除仅上下左右连接的瓦片
        /// </summary>
        /// <param name="check"></param>
        /// <param name="substitute"></param>
        private bool CheckTileMapDataIndividual(int x, int y, NodeType check, NodeType substitute)
        {

            if (_nodes[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, check);

                if (count <= 1)
                {
                    _nodes[x, y].type = substitute;
                    return false;
                }

                if (count == 2)
                {
                    // 计数为2且是只有上下两格
                    if (y < _height - 1 && _nodes[x, y + 1].type == check &&
                    y > 1 && _nodes[x, y - 1].type == check)
                    {
                        _nodes[x, y].type = substitute;
                        return false;
                    }
                    // 计数为2且是只有左右两格
                    if (x < _width - 1 && _nodes[x + 1, y].type == check &&
                     x > 1 && _nodes[x - 1, y].type == check)
                    {
                        _nodes[x, y].type = substitute;
                        return false;
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// 剔除不可以相邻的瓦片，并替换成对应的瓦片
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="check"></param>
        /// <param name="cannot"></param>
        /// <param name="substitute"></param>
        /// <returns></returns>
        private bool CheckTileMapDataOutline(int x, int y, NodeType check, NodeType cannot, NodeType substitute)
        {

            if (_nodes[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, cannot);

                if (count > 0)
                {
                    _nodes[x, y].type = substitute;
                    return false;
                }
            }
            return true;
        }

        private int CalculatNeighborGrass(int x, int y, NodeType check)
        {
            int count = 0;
            if (y < _height - 1 && _nodes[x, y + 1].type == check) { count++; }
            if (y > 1 && _nodes[x, y - 1].type == check) { count++; }
            if (x < _width - 1 && _nodes[x + 1, y].type == check) { count++; }
            if (x > 1 && _nodes[x - 1, y].type == check) { count++; }
            return count;
        }

        #endregion
    }
}