using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ChenChen_Map
{
    public class MapCreator : MonoBehaviour
    {
        [Header("生成器基本信息")]
        public TileBase mainMapDefaultTileBase;
        public float lacunarrty;
        public bool needSaveForPrefab;

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
        [Header("预生成物体数据")]
        [SerializeField] private List<ThingData> _prefabsList;

        /// <summary>
        /// 生成地图，并把返回对应的地图GameObject
        /// </summary>
        public MapData GenerateMap(MapData mapData, bool isSave)
        {
            MapData result = mapData;
            // 全局参数
            _width = mapData.width;
            _height = mapData.height;
            _nodes = new MapNode[_width, _height];
            Random.InitState(mapData.seed);

            //Debug.Log("生成地图" + mapData.mapName);
            InitMapObject(mapData);
            // 噪声
            InitNodeNoiseValue();
            // 根据噪声设置类型
            InitNodeType();
            // 剔除穿帮节点
            CheckNode();
            // 生成瓦片地图
            GenerateTileMap();
            // 生成花草环境
            GenerateFlowers();
            // 如果是第一次生成地图，则同时生成环境
            if (!isSave) GenerateThings(mapData);

            result.mapNodes = _nodes;
            result.mapObject = _mapObj;
            return result;
        }

        #region 地图生成

        private void InitMapObject(MapData mapData)
        {
            // 地图的Object
            _mapObj = new GameObject(mapData.mapName);
            _mapObj.transform.parent = transform;
            InitMapTilemap();

        }

        private void InitMapTilemap()
        {
            GameObject grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
            grid.transform.parent = _mapObj.transform;
            _layerDict = new Dictionary<string, Tilemap>();
            foreach (var t in _terrainList)
            {
                GetOrSetTileamp(name: t.tilemapName,
                    parent: grid,
                    isObstacle: false,
                    layerSort: t.layerSort,
                    tag: (t.type == NodeType.water) ? "Water" : null);
            }
            foreach (var f in _flowersList)
            {
                GetOrSetTileamp(name: f.tilemapName,
                    parent: grid,
                    isObstacle: false,
                    layerSort: f.layerSort
                    );
            }
        }

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

        private void GenerateTileMap()
        {
            Tilemap mainMap = GetOrSetTileamp("Grass");
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

        private void GenerateFlowers()
        {
            foreach (var flower in _flowersList)
            {
                int count = 0;
                List<MapNode> theGrassTile = new List<MapNode>();
                foreach (var node in _nodes)
                {
                    if (node.type == flower.type)
                    {
                        if (flower.farFormOtherTile && Has(node.position.x, node.position.y, flower.type))
                        {
                            continue;
                        }
                        theGrassTile.Add(node);
                        count++;
                    }
                }

                for (int i = 0; i < count * flower.probability; i++)
                {
                    if (theGrassTile.Count == 0) return;
                    // 随机选一块草地
                    int randomIndex = Random.Range(0, theGrassTile.Count);
                    Vector3Int pos = new Vector3Int(theGrassTile[randomIndex].position.x, theGrassTile[randomIndex].position.y);
                    // 随机选一种草
                    TileBase tile = flower.tile[Random.Range(0, flower.tile.Count)];
                    // 生成
                    GetOrSetTileamp(flower.tilemapName).SetTile(pos, tile);
                }
            }

            bool Has(int x, int y, NodeType me)
            {
                bool flag = false;

                // 检查八个方向
                if (IsOtherType(x - 1, y, me)) flag = true; // 左
                if (IsOtherType(x + 1, y, me)) flag = true; // 右
                if (IsOtherType(x, y - 1, me)) flag = true; // 上
                if (IsOtherType(x, y + 1, me)) flag = true; // 下
                if (IsOtherType(x - 1, y - 1, me)) flag = true; // 左上
                if (IsOtherType(x + 1, y - 1, me)) flag = true; // 右上
                if (IsOtherType(x - 1, y + 1, me)) flag = true; // 左下
                if (IsOtherType(x + 1, y + 1, me)) flag = true; // 右下

                return flag;
            }

            bool IsOtherType(int x, int y, NodeType me)
            {
                if (y + 1 > _height || y - 1 < 0) return true;
                if (x + 1 > _width || x - 1 < 0) return true;
                return _nodes[x, y].type != me;
            }
        }

        private void GenerateThings(MapData mapData)
        {
            List<Vector2Int> vector2Ints = new(); 
            ItemCreator itemCreator = new ItemCreator();
            foreach (var thing in _prefabsList)
            {
                int generatedCount = 0;
                int flag = 0;   // 防止无限循环
                while (generatedCount < thing.num && flag < 1000)
                {
                    // 随机生成一个位置
                    Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, _width), UnityEngine.Random.Range(0, _height));

                    // 检查节点类型是否匹配
                    if (!vector2Ints.Contains(pos) && _nodes[pos.x, pos.y].type == thing.type)
                    {
                        itemCreator.GenerateItem(thing.name, pos, mapData.mapName);
                        generatedCount++;
                        vector2Ints.Add(pos);
                    }
                    else
                    {
                        flag++;
                    }
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
                    return GetOrSetTileamp(t.tilemapName);
                }
            }
            Debug.LogError($"未能找到对应的Tilemap : {node.position.x}, {node.position.y}");
            return null;
        }

        /// <summary>
        /// 获取已经生成的Tilemap，如果没有则新生成一个，参数parent作为其父类
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Tilemap GetOrSetTileamp(string name, GameObject parent = null, bool isObstacle = true, int layerSort = 0, string tag = null)
        {
            if(_layerDict != null && _layerDict.ContainsKey(name))
            {
                return _layerDict[name];
            }
            else
            {
                Tilemap tilemap = CreateTilemap(name, parent, isObstacle, layerSort, tag);

                return tilemap;
            }
        }

        private Tilemap CreateTilemap(string name, GameObject parent, bool isObstacle, int layerSort, string tag)
        {
            Debug.Log($"未能找到对应的Tilemap，所以重新生成了一个 : {name}");

            GameObject obj = new GameObject(name);
            obj.transform.parent = parent.transform;

            Tilemap tilemap = obj.AddComponent<Tilemap>();
            _layerDict.Add(name, tilemap);

            TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();
#if UNITY_EDITOR              
            //Bug
#else
            tr.material = Resources.Load<Material>("Materials/Material-Tilemap");
#endif
            tr.sortingOrder = layerSort;
            tr.sortingLayerName = "Bottom";
            
            if (tag != null)
            {
                obj.tag = "Water";
            }

            if (isObstacle)
            {
                obj.AddComponent<TilemapCollider2D>().usedByComposite = true;
                obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                obj.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
                obj.layer = (obj.CompareTag("Water")) ? 4 : 8; //Water Layer or Obstacle Layer
            }

            return tilemap;
        }

#endregion

        #region 检查地图数据，剔除不合理内容

        // 剔除掉一些单独突出的数据，防止穿帮
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

        // 剔除单个的瓦片，剔除只有一块突出的瓦片，剔除仅上下左右连接的瓦片
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

        // 剔除不可以相邻的瓦片，并替换成对应的瓦片
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