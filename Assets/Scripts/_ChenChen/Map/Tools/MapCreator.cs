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

            Debug.Log("生成地图" + mapData.mapName);
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
                newObj.transform.parent = grid.transform;
                Tilemap tilemap = newObj.AddComponent<Tilemap>();
                TilemapRenderer tr = newObj.AddComponent<TilemapRenderer>();
                tr.sortingOrder = t.layerSort;
                // 默认光照材质Sprite-Lit-Default，Assets/Resources/Materials/Sprite-Lit-Default.mat
                tr.material = Resources.Load<Material>("Materials/Sprite-Lit-Default");           
                if (t.isObstacle)
                {
                    newObj.AddComponent<TilemapCollider2D>().usedByComposite = true;
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
            // 噪声
            InitNodeNoiseValue();
            // 根据噪声设置类型
            InitNodeType();
            // 剔除穿帮节点
            CheckNode();
            // 生成瓦片地图
            GenerateTileMap();
            // 生成花草
            GenerateFlowers();
            // 如果是第一次生成地图，则同时生成环境
            if(!isSave) GenerateThings(mapData);

            result.mapNodes = _nodes;
            result.mapObject = _mapObj;
            return result;
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
            foreach (var flower in _flowersList)
            {
                // 统计剩下多少块草地
                int count = 0;
                List<MapNode> theGrassTile = new List<MapNode>();
                foreach (var node in _nodes)
                {
                    if (node.type == flower.type)
                    {
                        // 上下左右没有其他类型的瓦片
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
                    GetTileamp(flower.tilemapName).SetTile(pos, tile);
                }
            }

            // 周围有其他类型
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

            // 是不同的类型
            bool IsOtherType(int x, int y, NodeType me)
            {
                if (y + 1 > _height || y - 1 < 0) return true;
                if (x + 1 > _width || x - 1 < 0) return true;
                return _nodes[x, y].type != me;
            }
        }

        /// <summary>
        /// 生成预制件
        /// </summary>
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
                    //return _mapObj.transform.Find(t.tilemapName).GetComponent<Tilemap>();
                    return _layerDict[t.tilemapName];
                }
            }
            Debug.Log($"未能找到对应的Tilemap : {node.position.x}, {node.position.y}");
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