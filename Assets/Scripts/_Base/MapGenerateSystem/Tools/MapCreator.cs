using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ChenChen_MapGenerator
{
    public class MapCreator : MonoBehaviour
    {
        [Header("������������Ϣ")]
        public Tilemap mainMap;
        public TileBase mainMapDefaultTileBase;
        public Tilemap leavesMap;
        public float lacunarrty;
        public GameObject mapPrefab;

        // ���ɵ�ͼʱ�õ���ȫ������
        private int _mapWidth;
        private int _mapHeight;
        private MapNode[,] _mapNodes;
        private SceneMapData _mapData;

        [Header("��������")]
        [SerializeField] private List<TerrainData> _terrainList;


        [Header("���ɻ����ݲ�")]
        [Range(0, 1f)]
        public float leavesProbability;
        [SerializeField] private List<TileBase> _leavesTileBaseList = new List<TileBase>();


        [Header("���������б�")]
        [SerializeField] private List<ItemData> _itemList;

        /// <summary>
        /// ��������Ĵ��λ�ø���
        /// </summary>
        [SerializeField] private GameObject _environmentObject;

        /// <summary>
        /// ���ɵ�ͼ�����Ѷ�Ӧ�����ݷ���
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public SceneMapData GenerateMap(int width, int height,SceneMapData mapData)
        {
            transform.Find("��ǰ��ͼ").gameObject.SetActive(true);

            _mapWidth = width;
            _mapHeight = height;
            Random.InitState(mapData.seed);
            _mapNodes = new MapNode[width, height];
            _mapData = mapData;

            InitMapNode();

            // ����ͼ����
            Check();

            // ���ݵ�ͼ����������Ƭ
            // SetTileWalkable();
            GenerateTileMap();

            // ���ɻ���
            GenerateLeaveAndFlowers();

            // ���ɻ���
            _itemList.Sort((item1, item2) => item1.priority.CompareTo(item2.priority));

            foreach (var item in _itemList)
            {
                GenerateItem(item.name);
            }

            transform.Find("��ǰ��ͼ").gameObject.SetActive(false);
            _mapData.mapNodes = _mapNodes;

            return _mapData;
        }

        #region ��ͼ����

        /// <summary>
        /// �����Ƭ��ͼ�������ٻ���������
        /// </summary>
        private void CleanMap()
        {
            // ������Ƭ
            foreach (var t in _terrainList)
            {
                t.loadingTilemap.ClearAllTiles();
            }
            leavesMap.ClearAllTiles();

            // ������

            // ������ʱ����������
            foreach (Transform child in _environmentObject.transform)
            {
                Destroy(child.gameObject);
            }

            // Debug.Log("Clear All");
        }

        /// <summary>
        /// ���õ�ͼ����ֵ�����ɶ�Ӧ��TileData��Ȼ���������ֵȷ����Ƭ����
        /// </summary>
        private void InitMapNode()
        {
            float randomOffset = Random.Range(-1000, 1000);

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                    _mapNodes[x, y] = new MapNode(new Vector2Int(x, y), nosieValue);

                    if (nosieValue < minValue) minValue = nosieValue;
                    if (nosieValue > maxValue) maxValue = nosieValue;
                }
            }

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _mapNodes[x, y].noiseValue = Mathf.Lerp(minValue, maxValue, _mapNodes[x, y].noiseValue);
                    foreach (var t in _terrainList)
                    {
                        if (_mapNodes[x, y].noiseValue <= t.probability)
                        {
                            _mapNodes[x, y].type = t.type;
                            _mapNodes[x, y].loadingTilemap = t.loadingTilemap;
                            break;
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// ���������ܷ���������Ƭ��ͨ��
        ///// </summary>
        //private void SetTileWalkable()
        //{
        //    for (int x = 0; x < _mapWidth; x++)
        //    {
        //        for (int y = 0; y < _mapHeight; y++)
        //        {
        //            if (_mapData[x, y].type == MapNode.Type.water)
        //            {
        //                _mapData[x, y].noObstacles = false;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// ���յ�������Ƭ��ͼ������Ӧ�ĵ�ͼ������Ƭ
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
                        _mapNodes[x, y].loadingTilemap.SetTile(new Vector3Int(x, y), tile);
                    }
                }
            }
        }

        /// <summary>
        /// ���ɻ����ݲ�
        /// </summary>
        private void GenerateLeaveAndFlowers()
        {
            // ͳ���ж��ٿ�ݵ�
            int count = 0;
            List<MapNode> theGrassTile = new List<MapNode>();
            foreach (var md in _mapNodes)
            {
                if (md.type == MapNode.Type.grass)
                {
                    theGrassTile.Add(md);
                    count++;
                }
            }

            for (int i = 0; i < count * leavesProbability; i++)
            {
                if (theGrassTile.Count == 0) return;
                int randomIndex = Random.Range(0, theGrassTile.Count);
                leavesMap.SetTile(new Vector3Int(theGrassTile[randomIndex].postion.x, theGrassTile[randomIndex].postion.y)
                    , _leavesTileBaseList[Random.Range(0, _leavesTileBaseList.Count)]);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="itemName"></param>
        private void GenerateItem(string itemName)
        {
            ItemData item = _itemList.FirstOrDefault(it => it.name == itemName);
            if (item.prefab != null)
            {
                List<MapNode> theEnvironmentTile = GetEnvironmentTileList(item);
                List<MapNode> empty = new(theEnvironmentTile);

                // ����
                if (item.useNumber)
                {
                    Create(item, empty, item.number);
                }
                else
                {
                    Create(item, empty, empty.Count * item.probability);
                }

                void Create(ItemData item, List<MapNode> empty, float number)
                {
                    GameObject parentObject = _environmentObject;
                    GameObject environmentObjs = new GameObject(item.name);
                    environmentObjs.transform.parent = parentObject.transform;
                    for (int i = 0; i < number; i++)
                    {
                        if (empty.Count <= 0) break;

                        // ȷ������λ��
                        int randomIndex = Random.Range(0, empty.Count);
                        Vector3Int tilePos = new Vector3Int(empty[randomIndex].postion.x, empty[randomIndex].postion.y);
                        Vector3 createPos = tilePos + item.offset;

                        // �������λ���Ƿ����
                        bool flag = true;
                        for (int x = tilePos.x - item.width / 2; x <= tilePos.x + item.width / 2; x++)
                        {
                            for (int y = tilePos.y - item.height / 2; y <= tilePos.y + item.height / 2; y++)
                            {
                                if (_mapData.obstaclesPositionList.Contains(new Vector3(x,y)))
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
                                    _mapData.obstaclesPositionList.Add(new Vector3(x,y));
                                    if (empty.Contains(_mapNodes[x, y]))
                                    {
                                        empty.Remove(_mapNodes[x, y]);
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
                List<MapNode> GetEnvironmentTileList(ItemData item)
                {
                    List<MapNode> theEnvironmentTile = new List<MapNode>();

                    for (int x = 0; x < _mapWidth; x++)
                    {
                        for (int y = 0; y < _mapHeight; y++)
                        {
                            if (_mapNodes[x, y].type == item.environment && CalculatNeighborGrass(x, y, MapNode.Type.water) <= 0 
                                && !_mapData.obstaclesPositionList.Contains(new Vector3(x, y)))
                            {
                                theEnvironmentTile.Add(_mapNodes[x, y]);
                            }
                        }
                    }

                    return theEnvironmentTile;
                }
            }
        }

        private TileBase GetTileBaseByTerrainType(int x, int y)
        {
            foreach (var t in _terrainList)
            {
                if (t.type == _mapNodes[x, y].type) return t.tile;
            }
            return null;
        }

        #endregion

        #region ����ͼ���ݣ��޳�����������

        /// <summary>
        /// �޳���һЩ����ͻ�������ݣ���ֹ����
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
                        if (!CheckTileMapDataIndividual(x, y, MapNode.Type.water, MapNode.Type.none)) flag = false;
                        if (!CheckTileMapDataIndividual(x, y, MapNode.Type.ground, MapNode.Type.none)) flag = false;
                        if (!CheckTileMapDataOutline(x, y, MapNode.Type.ground, MapNode.Type.water, MapNode.Type.none)) flag = false;
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
        /// �޳���������Ƭ���޳�ֻ��һ��ͻ������Ƭ���޳��������������ӵ���Ƭ
        /// </summary>
        /// <param name="check"></param>
        /// <param name="substitute"></param>
        private bool CheckTileMapDataIndividual(int x, int y, MapNode.Type check, MapNode.Type substitute)
        {

            if (_mapNodes[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, check);

                if (count <= 1)
                {
                    _mapNodes[x, y].type = substitute;
                    return false;
                }

                if (count == 2)
                {
                    // ����Ϊ2����ֻ����������
                    if (y < _mapHeight - 1 && _mapNodes[x, y + 1].type == check &&
                    y > 1 && _mapNodes[x, y - 1].type == check)
                    {
                        _mapNodes[x, y].type = substitute;
                        return false;
                    }
                    // ����Ϊ2����ֻ����������
                    if (x < _mapWidth - 1 && _mapNodes[x + 1, y].type == check &&
                     x > 1 && _mapNodes[x - 1, y].type == check)
                    {
                        _mapNodes[x, y].type = substitute;
                        return false;
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// �޳����������ڵ���Ƭ�����滻�ɶ�Ӧ����Ƭ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="check"></param>
        /// <param name="cannot"></param>
        /// <param name="substitute"></param>
        /// <returns></returns>
        private bool CheckTileMapDataOutline(int x, int y, MapNode.Type check, MapNode.Type cannot, MapNode.Type substitute)
        {

            if (_mapNodes[x, y].type == check)
            {
                int count = CalculatNeighborGrass(x, y, cannot);

                if (count > 0)
                {
                    _mapNodes[x, y].type = substitute;
                    return false;
                }
            }
            return true;
        }

        private int CalculatNeighborGrass(int x, int y, MapNode.Type check)
        {
            int count = 0;
            if (y < _mapHeight - 1 && _mapNodes[x, y + 1].type == check) { count++; }
            if (y > 1 && _mapNodes[x, y - 1].type == check) { count++; }
            if (x < _mapWidth - 1 && _mapNodes[x + 1, y].type == check) { count++; }
            if (x > 1 && _mapNodes[x - 1, y].type == check) { count++; }
            return count;
        }

        #endregion


    }
}