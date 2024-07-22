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
            // ��ͼ��Object
            _mapObj = new GameObject(mapData.mapName);
            _mapObj.transform.parent = transform;
            // ���Grid���
            GameObject grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
            grid.transform.parent = _mapObj.transform;
            // ���Terrain��ص�Tilemap
            _layerDict = new Dictionary<string, Tilemap>();
            var material = Resources.Load<Material>("Materials/Material-Tilemap");
            foreach (var t in _terrainList)
            {
                if (!_layerDict.ContainsKey(t.tilemapName))
                {            
                    GameObject obj = new GameObject(t.tilemapName);
                    obj.transform.parent = grid.transform;
                    Tilemap tilemap = obj.AddComponent<Tilemap>();
                    TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();
                    tr.sortingOrder = t.layerSort;
                    tr.material = material;
                    if (t.isObstacle)
                    {
                        obj.AddComponent<TilemapCollider2D>().usedByComposite = true;
                        obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                        obj.AddComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
                        obj.layer = (t.type == NodeType.water) ? 4 : 8; //Water Layer or Obstacle Layer
                    }
                    if (t.type == NodeType.water)
                    {
                        obj.tag = "Water";
                    }
                    _layerDict.Add(t.tilemapName, tilemap);
                }
            }
            // ���Flower��ص�Tilemap
            foreach (var f in _flowersList)
            {
                if (!_layerDict.ContainsKey(f.tilemapName))
                {
                    GameObject obj = new GameObject(f.tilemapName);
                    obj.transform.parent = grid.transform;
                    Tilemap tilemap = obj.AddComponent<Tilemap>();
                    TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();
                    tr.sortingOrder = f.layerSort;
                    tr.material = material;

                    _layerDict.Add(f.tilemapName, tilemap);
                }
            }
            // ����
            InitNodeNoiseValue();
            // ����������������
            InitNodeType();
            // �޳�����ڵ�
            CheckNode();
            // ������Ƭ��ͼ
            GenerateTileMap();
            // ���ɻ���
            GenerateFlowers();
            // ����ǵ�һ�����ɵ�ͼ����ͬʱ���ɻ���
            if(!isSave) GenerateThings(mapData);

            result.mapNodes = _nodes;
            result.mapObject = _mapObj;
            return result;
        }

        #region ��ͼ����

        /// <summary>
        /// ���õ�ͼ����ֵ
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
        /// ������Ƭ����
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
        /// ���յ�������Ƭ��ͼ������Ӧ�ĵ�ͼ������Ƭ
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
        /// ���ɻ���
        /// </summary>
        private void GenerateFlowers()
        {
            foreach (var flower in _flowersList)
            {
                // ͳ��ʣ�¶��ٿ�ݵ�
                int count = 0;
                List<MapNode> theGrassTile = new List<MapNode>();
                foreach (var node in _nodes)
                {
                    if (node.type == flower.type)
                    {
                        // ��������û���������͵���Ƭ
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
                    GetTileamp(flower.tilemapName).SetTile(pos, tile);
                }
            }

            // ��Χ����������
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

            // �ǲ�ͬ������
            bool IsOtherType(int x, int y, NodeType me)
            {
                if (y + 1 > _height || y - 1 < 0) return true;
                if (x + 1 > _width || x - 1 < 0) return true;
                return _nodes[x, y].type != me;
            }
        }

        /// <summary>
        /// ����Ԥ�Ƽ�
        /// </summary>
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
                    //return _mapObj.transform.Find(t.tilemapName).GetComponent<Tilemap>();
                    return _layerDict[t.tilemapName];
                }
            }
            Debug.Log($"δ���ҵ���Ӧ��Tilemap : {node.position.x}, {node.position.y}");
            return null;
        }

        /// <summary>
        /// ��ȡ�Ѿ����ɵ�Tilemap�����û����������һ��������parent��Ϊ�丸��
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
                Debug.Log($"δ���ҵ���Ӧ��Tilemap��������������һ�� : {name}");
                GameObject newObj = new GameObject(name);
                Tilemap tilemap = newObj.AddComponent<Tilemap>();
                TilemapRenderer tr = newObj.AddComponent<TilemapRenderer>();
                tr.sortingLayerName = "Bottom";
                // Ĭ�Ϲ��ղ���Sprite-Lit-Default��Assets/Resources/Materials/Material-Tilemap.mat
                tr.material = Resources.Load<Material>("Materials/Material-Tilemap");
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

        #region ����ͼ���ݣ��޳�����������

        /// <summary>
        /// �޳���һЩ����ͻ�������ݣ���ֹ����
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
        /// �޳���������Ƭ���޳�ֻ��һ��ͻ������Ƭ���޳��������������ӵ���Ƭ
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

        /// <summary>
        /// �޳����������ڵ���Ƭ�����滻�ɶ�Ӧ����Ƭ
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