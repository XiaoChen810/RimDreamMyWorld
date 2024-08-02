using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ChenChen_Map
{
    public class MapCreator : MonoBehaviour
    {
        [Header("������������Ϣ")]
        public TileBase mainMapDefaultTileBase;
        public float lacunarrty;
        public bool needSaveForPrefab;

        // ���ɵ�ͼʱ�õ���ȫ������
        private int _width;
        private int _height;
        private MapNode[,] _nodes;
        private GameObject _mapObj;
        private Dictionary<string, Tilemap> _layerDict; 

        [Header("��������")]
        [SerializeField] private List<TerrainData> _terrainList;
        [Header("��������")]
        [SerializeField] private List<FlowerData> _flowersList;
        [Header("Ԥ������������")]
        [SerializeField] private List<ThingData> _prefabsList;

        /// <summary>
        /// ���ɵ�ͼ�����ѷ��ض�Ӧ�ĵ�ͼGameObject
        /// </summary>
        public MapData GenerateMap(MapData mapData, bool isSave)
        {
            MapData result = mapData;
            // ȫ�ֲ���
            _width = mapData.width;
            _height = mapData.height;
            _nodes = new MapNode[_width, _height];
            Random.InitState(mapData.seed);

            //Debug.Log("���ɵ�ͼ" + mapData.mapName);
            InitMapObject(mapData);
            // ����
            InitNodeNoiseValue();
            // ����������������
            InitNodeType();
            // �޳�����ڵ�
            CheckNode();
            // ������Ƭ��ͼ
            GenerateTileMap();
            // ���ɻ��ݻ���
            GenerateFlowers();
            // ����ǵ�һ�����ɵ�ͼ����ͬʱ���ɻ���
            if (!isSave) GenerateThings(mapData);

            result.mapNodes = _nodes;
            result.mapObject = _mapObj;
            return result;
        }

        #region ��ͼ����

        private void InitMapObject(MapData mapData)
        {
            // ��ͼ��Object
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
                    // ���ѡһ��ݵ�
                    int randomIndex = Random.Range(0, theGrassTile.Count);
                    Vector3Int pos = new Vector3Int(theGrassTile[randomIndex].position.x, theGrassTile[randomIndex].position.y);
                    // ���ѡһ�ֲ�
                    TileBase tile = flower.tile[Random.Range(0, flower.tile.Count)];
                    // ����
                    GetOrSetTileamp(flower.tilemapName).SetTile(pos, tile);
                }
            }

            bool Has(int x, int y, NodeType me)
            {
                bool flag = false;

                // ���˸�����
                if (IsOtherType(x - 1, y, me)) flag = true; // ��
                if (IsOtherType(x + 1, y, me)) flag = true; // ��
                if (IsOtherType(x, y - 1, me)) flag = true; // ��
                if (IsOtherType(x, y + 1, me)) flag = true; // ��
                if (IsOtherType(x - 1, y - 1, me)) flag = true; // ����
                if (IsOtherType(x + 1, y - 1, me)) flag = true; // ����
                if (IsOtherType(x - 1, y + 1, me)) flag = true; // ����
                if (IsOtherType(x + 1, y + 1, me)) flag = true; // ����

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
                int flag = 0;   // ��ֹ����ѭ��
                while (generatedCount < thing.num && flag < 1000)
                {
                    // �������һ��λ��
                    Vector2Int pos = new Vector2Int(UnityEngine.Random.Range(0, _width), UnityEngine.Random.Range(0, _height));

                    // ���ڵ������Ƿ�ƥ��
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
            Debug.LogError($"δ���ҵ���Ӧ��Tilemap : {node.position.x}, {node.position.y}");
            return null;
        }

        /// <summary>
        /// ��ȡ�Ѿ����ɵ�Tilemap�����û����������һ��������parent��Ϊ�丸��
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
            Debug.Log($"δ���ҵ���Ӧ��Tilemap����������������һ�� : {name}");

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

        #region ����ͼ���ݣ��޳�����������

        // �޳���һЩ����ͻ�������ݣ���ֹ����
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

        // �޳���������Ƭ���޳�ֻ��һ��ͻ������Ƭ���޳��������������ӵ���Ƭ
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
                    // ����Ϊ2����ֻ����������
                    if (y < _height - 1 && _nodes[x, y + 1].type == check &&
                    y > 1 && _nodes[x, y - 1].type == check)
                    {
                        _nodes[x, y].type = substitute;
                        return false;
                    }
                    // ����Ϊ2����ֻ����������
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

        // �޳����������ڵ���Ƭ�����滻�ɶ�Ӧ����Ƭ
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