using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

namespace MyMapGenerate
{
    public class MapCreator : MonoBehaviour
    {
        [Header("生成器基本信息")]
        public Tilemap mainMap;
        public TileBase mainMapDefaultTileBase;
        public Tilemap leavesMap;
        public float lacunarrty;
        private int _mapWidth;
        private int _mapHeight;

        /// <summary>
        /// 瓦片的数据
        /// </summary>
        public class TileData
        {
            public TileData(int x, int y, float noiseValue)
            {
                this.x = x; this.y = y;
                this.noiseValue = noiseValue;
                this.walkAble = true;
            }

            // 位置
            public int x;
            public int y;
            // 噪声值
            public float noiseValue;
            // 瓦片类型
            public enum Type
            {
                none, grass, water, ground, mountain
            }
            public Type type = Type.none;
            // 附属的瓦片地图
            public Tilemap loadingTilemap;
            // 人物能否走在上面
            public bool walkAble;
        }
        private TileData[,] _mapData;
        /// <summary>
        /// 储存地形数据的结构体
        /// </summary>
        [System.Serializable]
        public struct TerrainData
        {
            [Range(0, 1f)]
            public float probability;

            public TileBase tile;

            public TileData.Type type;

            public Tilemap loadingTilemap;
        }
        [Header("地形数据")]
        [SerializeField] private List<TerrainData> _terrainList;


        [Header("生成花花草草")]
        [Range(0, 1f)]
        public float leavesProbability;
        [SerializeField] private List<TileBase> _leavesTileBaseList = new List<TileBase>();

        /// <summary>
        /// 物体数据,树木,房子等
        /// </summary>
        [System.Serializable]
        public struct ItemData
        {
            public string name;
            [Range(0, 1f)] public float probability;
            public int number;
            public bool useNumber;
            public GameObject prefab;
            public TileData.Type environment;
            public Vector3 offset;
            public int height;
            public int width;
            public int priority;
        }
        [Header("物体数据")]
        [SerializeField] private List<ItemData> _itemList;

        /// <summary>
        /// 生成物体的存放位置父类
        /// </summary>
        [SerializeField] private GameObject _environmentObject;

        /// <summary>
        /// 生成地图，并把对应的数据返回
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public TileData[,] GenerateMap(int width, int height, int seed)
        {
            transform.Find("当前地图").gameObject.SetActive(true);

            _mapData = new TileData[width, height];
            _mapWidth = width;
            _mapHeight = height;
            Random.InitState(seed);

            InitMapData();

            // 检查地图数据
            Check();

            // 根据地图数据生成瓦片
            SetTileWalkable();
            GenerateTileMap();

            // 生成花草
            GenerateLeaveAndFlowers();

            // 生成环境
            _itemList.Sort((item1, item2) => item1.priority.CompareTo(item2.priority));

            foreach (var item in _itemList)
            {
                GenerateItem(item.name);
            }

            transform.Find("当前地图").gameObject.SetActive(false);
            return _mapData;
        }

        #region 地图生成

        /// <summary>
        /// 清空瓦片地图，和销毁环境的物体
        /// </summary>
        private void CleanMap()
        {
            // 清理瓦片
            foreach (var t in _terrainList)
            {
                t.loadingTilemap.ClearAllTiles();
            }
            leavesMap.ClearAllTiles();

            // 清理环境

            // 在运行时清理子物体
            foreach (Transform child in _environmentObject.transform)
            {
                Destroy(child.gameObject);
            }

            // Debug.Log("Clear All");
        }

        /// <summary>
        /// 设置地图噪声值，生成对应的TileData，然后根据噪声值确定瓦片类型
        /// </summary>
        private void InitMapData()
        {
            float randomOffset = Random.Range(-1000, 1000);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                    _mapData[x, y] = new TileData(x, y, nosieValue);

                    if (nosieValue < minValue) minValue = nosieValue;
                    if (nosieValue > maxValue) maxValue = nosieValue;
                }
            }

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _mapData[x, y].noiseValue = Mathf.Lerp(minValue, maxValue, _mapData[x, y].noiseValue);
                    foreach (var t in _terrainList)
                    {
                        if (_mapData[x, y].noiseValue <= t.probability)
                        {
                            _mapData[x, y].type = t.type;
                            _mapData[x, y].loadingTilemap = t.loadingTilemap;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置人物能否在这种瓦片上通行
        /// </summary>
        private void SetTileWalkable()
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    if (_mapData[x, y].type == TileData.Type.water)
                    {
                        _mapData[x, y].walkAble = false;
                    }
                }
            }
        }

        /// <summary>
        /// 最终的生成瓦片地图，给对应的地图设置瓦片
        /// </summary>
        private void GenerateTileMap()
        {
            CleanMap();

            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    TileBase tile = GetTileBaseByTerrainType(x, y);
                    mainMap.SetTile(new Vector3Int(x, y), mainMapDefaultTileBase);

                    if (tile != null)
                    {
                        _mapData[x, y].loadingTilemap.SetTile(new Vector3Int(x, y), tile);
                    }
                }
            }
        }

        /// <summary>
        /// 生成花花草草
        /// </summary>
        private void GenerateLeaveAndFlowers()
        {
            // 统计有多少块草地
            int count = 0;
            List<TileData> theGrassTile = new List<TileData>();
            foreach (var md in _mapData)
            {
                if (md.type == TileData.Type.grass)
                {
                    theGrassTile.Add(md);
                    count++;
                }
            }

            for (int i = 0; i < count * leavesProbability; i++)
            {
                if (theGrassTile.Count == 0) return;
                int randomIndex = Random.Range(0, theGrassTile.Count);
                leavesMap.SetTile(new Vector3Int(theGrassTile[randomIndex].x, theGrassTile[randomIndex].y)
                    , _leavesTileBaseList[Random.Range(0, _leavesTileBaseList.Count)]);
            }
        }

        /// <summary>
        /// 生成物体
        /// </summary>
        /// <param name="itemName"></param>
        private void GenerateItem(string itemName)
        {
            ItemData item = _itemList.FirstOrDefault(it => it.name == itemName);
            if (item.prefab != null)
            {
                List<TileData> theEnvironmentTile = GetEnvironmentTileList(item);

                // 计算是否满足空间
                List<TileData> empty = new List<TileData>();

                for (int x = 0; x < _mapWidth - item.width + 1; x++)
                {
                    for (int y = 0; y < _mapHeight - item.height + 1; y++)
                    {
                        if (theEnvironmentTile.Contains(_mapData[x, y]))
                        {
                            int temp = IsSpaceAvailable(x, y, item);
                            if (temp == -1)
                            {
                                empty.Add(_mapData[x, y]);
                            }
                            else
                            {
                                y = temp;
                            }
                        }
                    }
                }

                // 生成
                if (item.useNumber)
                {
                    Create(item, empty, item.number);
                }
                else
                {
                    Create(item, empty, empty.Count * item.probability);
                }

                void Create(ItemData item, List<TileData> empty, float number)
                {
                    GameObject parentObject = _environmentObject;
                    GameObject environmentObjs = new GameObject(item.name);
                    environmentObjs.transform.parent = parentObject.transform;
                    for (int i = 0; i < number; i++)
                    {
                        if (empty.Count <= 0) break;

                        // 确定生成位置
                        int randomIndex = Random.Range(0, empty.Count);
                        Vector3Int tilePos = new Vector3Int(empty[randomIndex].x, empty[randomIndex].y);
                        Vector3 createPos = tilePos + item.offset;

                        // 检测生成位置是否可行
                        bool flag = true;
                        for (int x = tilePos.x - item.width / 2; x <= tilePos.x + item.width / 2; x++)
                        {
                            for (int y = tilePos.y - item.height / 2; y <= tilePos.y + item.height / 2; y++)
                            {
                                if (_mapData[x, y].walkAble == false)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            Instantiate(item.prefab, createPos, Quaternion.identity, environmentObjs.transform);
                            for (int x = tilePos.x - item.width / 2; x <= tilePos.x + item.width / 2; x++)
                            {
                                for (int y = tilePos.y - item.height / 2; y <= tilePos.y + item.height / 2; y++)
                                {
                                    _mapData[x, y].walkAble = false;
                                    if (empty.Contains(_mapData[x, y]))
                                    {
                                        empty.Remove(_mapData[x, y]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            i--;
                        }

                    }
                }
                List<TileData> GetEnvironmentTileList(ItemData item)
                {
                    List<TileData> theEnvironmentTile = new List<TileData>();

                    for (int x = 0; x < _mapWidth; x++)
                    {
                        for (int y = 0; y < _mapHeight; y++)
                        {
                            if (_mapData[x, y].type == item.environment && CalculatNeighborGrass(x, y, TileData.Type.water) <= 0 && _mapData[x, y].walkAble)
                            {
                                theEnvironmentTile.Add(_mapData[x, y]);
                            }
                        }
                    }

                    return theEnvironmentTile;
                }
                int IsSpaceAvailable(int startX, int startY, ItemData item)
                {
                    for (int x = startX - item.width / 2; x < startX + item.width / 2; x++)
                    {
                        for (int y = startY - item.height / 2; y < startY + item.height / 2; y++)
                        {
                            if (_mapData[x, y].walkAble == false && _mapData[x, y].type != item.environment)
                            {
                                return y;
                            }
                        }
                    }

                    return -1; // 如果满足条件
                }
            }
        }

        private TileBase GetTileBaseByTerrainType(int x, int y)
        {
            foreach (var t in _terrainList)
            {
                if (t.type == _mapData[x, y].type) return t.tile;
            }
            return null;
        }

        #endregion

        #region 检查地图数据，剔除不合理内容

        /// <summary>
        /// 剔除掉一些单独突出的数据，防止穿帮
        /// </summary>
        private void Check()
        {
            bool flag = false;
            for (int i = 0; i < 100; i++)
            {
                flag = true;
                for (int x = 0; x < _mapWidth; x++)
                {
                    for (int y = 0; y < _mapHeight; y++)
                    {
                        if (!CheckTileMapDataIndividual(x, y, TileData.Type.water, TileData.Type.none)) flag = false;
                        if (!CheckTileMapDataIndividual(x, y, TileData.Type.ground, TileData.Type.none)) flag = false;
                        if (!CheckTileMapDataOutline(x, y, TileData.Type.ground, TileData.Type.water, TileData.Type.none)) flag = false;
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
        private bool CheckTileMapDataIndividual(int x, int y, TileData.Type check, TileData.Type substitute)
        {

            if (_mapData[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, check);

                if (count <= 1)
                {
                    _mapData[x, y].type = substitute;
                    return false;
                }

                if (count == 2)
                {
                    // 计数为2且是只有上下两格
                    if (y < _mapHeight - 1 && _mapData[x, y + 1].type == check &&
                    y > 1 && _mapData[x, y - 1].type == check)
                    {
                        _mapData[x, y].type = substitute;
                        return false;
                    }
                    // 计数为2且是只有左右两格
                    if (x < _mapWidth - 1 && _mapData[x + 1, y].type == check &&
                     x > 1 && _mapData[x - 1, y].type == check)
                    {
                        _mapData[x, y].type = substitute;
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
        private bool CheckTileMapDataOutline(int x, int y, TileData.Type check, TileData.Type cannot, TileData.Type substitute)
        {

            if (_mapData[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, cannot);

                if (count > 0)
                {
                    _mapData[x, y].type = substitute;
                    return false;
                }
            }
            return true;
        }

        private int CalculatNeighborGrass(int x, int y, TileData.Type check)
        {
            int count = 0;
            if (y < _mapHeight - 1 && _mapData[x, y + 1].type == check) { count++; }
            if (y > 1 && _mapData[x, y - 1].type == check) { count++; }
            if (x < _mapWidth - 1 && _mapData[x + 1, y].type == check) { count++; }
            if (x > 1 && _mapData[x - 1, y].type == check) { count++; }
            return count;
        }

        #endregion


    }
}