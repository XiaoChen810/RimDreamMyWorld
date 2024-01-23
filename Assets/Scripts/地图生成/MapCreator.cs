using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour 
{
    [Header("������������Ϣ")]
    public Tilemap mainMap;
    public TileBase mainMapDefaultTileBase;
    public Tilemap leavesMap;

    public float lacunarrty;

    [System.Serializable]
    public struct TileProbability
    {
        [Range(0, 1f)]
        public float probability;

        public TileBase tile;

        public MapTileData.Type type;

        public Tilemap loadingTilemap;
    }
    [Header("��Ƭ����")]
    [SerializeField] private List<TileProbability> _tileList;

    public class MapTileData
    {
        public MapTileData(int x, int y, float noiseValue)
        {
            this.x = x; this.y = y;
            this.noiseValue = noiseValue;
        }

        // λ��
        public int x;
        public int y;

        // ��Ƭ����
        public enum Type
        {
            none, grass, water, ground, mountain
        }
        public Type type = Type.none;

        // ����ֵ
        public float noiseValue;

        // ��������Ƭ��ͼ
        public Tilemap loadingTilemap;

        // ��Ƭ���Ƿ�������
        public bool aboveEmpty = true;
    }
    private MapTileData[,] _mapData;
    private int _mapWidth;
    private int _mapHeight;

    [Header("��������")]
    [Range(0, 1f)]
    public float leavesProbability;
    [SerializeField] private List<TileBase> _leavesTileBaseList = new List<TileBase>();

    [System.Serializable]
    public struct ItemProbability
    { 
        public string name;
        [Range(0, 1f)] public float probability;
        public int number;
        public bool useNumber;
        public GameObject prefab;
        public MapTileData.Type environment;
        public Vector3 offset;
        public int height;
        public int width;
        public int priority;
    }
    [Header("��������")]
    [SerializeField] private List<ItemProbability> _itemList;

    public MapTileData[,] GenerateMap(int width,int height, int seed)
    {
        _mapData = new MapTileData[width, height];
        _mapWidth = width;
        _mapHeight = height;
        UnityEngine.Random.InitState(seed);

        InitMapData();

        // ����ͼ����
        Check();

        // ���ݵ�ͼ����������Ƭ
        GenerateTileMap();

        // ���ɻ���
        GenerateLeaveAndFlowers();

        // ���ɻ���
        _itemList.Sort((item1, item2) => item1.priority.CompareTo(item2.priority));

        foreach (var item in _itemList)
        {
            GenerateItem(item.name);
        }

        return _mapData;
    }

    public void CleanMap()
    {
        // ������Ƭ
        foreach (var t in _tileList)
        {
            t.loadingTilemap.ClearAllTiles();
        }
        leavesMap.ClearAllTiles();

        // ������
#if UNITY_EDITOR

        // �洢�����������
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            childrenToDestroy.Add(child);
        }

        // ����������
        foreach (Transform child in childrenToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }

#else
        // �洢�����������
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            childrenToDestroy.Add(child);
        }


        // ������ʱ����������
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

#endif

        // Debug.Log("Clear All");
    }

    #region ��ͼ����

    private void InitMapData()
    {   
        float randomOffset = UnityEngine.Random.Range(-1000, 1000);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                _mapData[x, y] = new MapTileData(x, y, nosieValue);

                if(nosieValue < minValue) minValue = nosieValue;
                if(nosieValue > maxValue) maxValue = nosieValue;
            }
        }

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                _mapData[x, y].noiseValue = Mathf.Lerp(minValue, maxValue, _mapData[x, y].noiseValue);
                foreach (var t in _tileList)
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

    private void GenerateTileMap()
    {
        CleanMap();

        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                TileBase tile = GetMapDataType(x, y);
                mainMap.SetTile(new Vector3Int(x, y), mainMapDefaultTileBase);

                if (tile != null)
                {
                    _mapData[x, y].loadingTilemap.SetTile(new Vector3Int(x, y), tile);
                }
            }
        }
    }

    private void GenerateLeaveAndFlowers()
    {
        // ͳ���ж��ٿ�ݵ�
        int count = 0;
        List<MapTileData> theGrassTile = new List<MapTileData>();
        foreach (var md in _mapData)
        {
            if (md.type == MapTileData.Type.grass)
            {
                theGrassTile.Add(md);
                count++;
            }
        }

        for (int i = 0; i < count * leavesProbability; i++)
        {
            if (theGrassTile.Count == 0) return;
            int randomIndex = UnityEngine.Random.Range(0, theGrassTile.Count);
            leavesMap.SetTile(new Vector3Int(theGrassTile[randomIndex].x, theGrassTile[randomIndex].y)
                , _leavesTileBaseList[UnityEngine.Random.Range(0, _leavesTileBaseList.Count)]);
        }
    }

    /// <summary>
    /// ��������Ԥ�Ƽ�
    /// </summary>
    /// <param name="itemName"></param>
    private void GenerateItem(string itemName)
    {
        ItemProbability item = _itemList.FirstOrDefault(it => it.name == itemName);
        if(item.prefab != null)
        {
            List<MapTileData> theEnvironmentTile = GetEnvironmentTileList(item);

            // �����Ƿ�����ռ�
            List<MapTileData> empty = new List<MapTileData>();

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

            // ����
            if (item.useNumber)
            {
                Create(item, empty,item.number);
            }
            else
            {
                Create(item, empty, empty.Count * item.probability);
            }
        }
    }

    private void Create(ItemProbability item, List<MapTileData> empty , float number)
    {
        GameObject parentObject = GameObject.Find("��ǰ��ͼ");
        GameObject environmentObjs = new GameObject(item.name);
        environmentObjs.transform.parent = parentObject.transform;
        for (int i = 0; i < number; i++)
        {
            if (empty.Count <= 0) break;

            // ȷ������λ��
            int randomIndex = UnityEngine.Random.Range(0, empty.Count);
            Vector3Int tilePos = new Vector3Int(empty[randomIndex].x, empty[randomIndex].y);
            Vector3 createPos = tilePos + item.offset;

            // �������λ���Ƿ����
            bool flag = true;
            for (int x = tilePos.x; x < tilePos.x + item.width; x++)
            {
                for (int y = tilePos.y; y < tilePos.y + item.height; y++)
                {
                    if (_mapData[x, y].aboveEmpty == false)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                Instantiate(item.prefab, createPos, Quaternion.identity, environmentObjs.transform);
                for (int x = tilePos.x; x < tilePos.x + item.width; x++)
                {
                    for (int y = tilePos.y; y < tilePos.y + item.height; y++)
                    {
                        _mapData[x, y].aboveEmpty = false;
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

    private List<MapTileData> GetEnvironmentTileList(ItemProbability item)
    {
        List<MapTileData> theEnvironmentTile = new List<MapTileData>();   

        for(int x = 0; x < _mapWidth; x++)
        {
            for(int y = 0; y < _mapHeight; y++)
            {
                if (_mapData[x,y].type == item.environment && CalculatNeighborGrass(x, y, MapTileData.Type.water) <= 0 && _mapData[x, y].aboveEmpty)
                {
                    theEnvironmentTile.Add(_mapData[x, y]);
                }
            }
        }

        return theEnvironmentTile;
    }

    private int IsSpaceAvailable( int startX, int startY, ItemProbability item)
    {
        // ����Ƿ���Ͽռ�����
        // ʹ�ÿռ����ݽṹ����ǰ����Ļ����ؿ�

        for (int x = startX; x < startX + item.width; x++)
        {
            for (int y = startY; y < startY + item.height; y++)
            {
                if (_mapData[x, y].aboveEmpty == false && _mapData[x, y].type != item.environment)
                {
                    return y;
                }
            }
        }

        return -1; // �����������
    }

    private TileBase GetMapDataType(int x, int y)
    {
        foreach (var t in _tileList)
        {
            if (t.type == _mapData[x, y].type) return t.tile;
        }
        return null;
    }

    #endregion

    #region ����ͼ���ݣ��޳�����������

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
                    if (!CheckTileMapDataIndividual(x, y, MapTileData.Type.water, MapTileData.Type.none)) flag = false;
                    if (!CheckTileMapDataIndividual(x, y, MapTileData.Type.ground, MapTileData.Type.none)) flag = false;
                    if (!CheckTileMapDataOutline(x, y, MapTileData.Type.ground, MapTileData.Type.water, MapTileData.Type.none)) flag = false;
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
    private bool CheckTileMapDataIndividual(int x,int y ,MapTileData.Type check, MapTileData.Type substitute)
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
                // ����Ϊ2����ֻ����������
                if ((y < _mapHeight - 1 && _mapData[x, y + 1].type == check) &&
                (y > 1 && _mapData[x, y - 1].type == check))
                {
                    _mapData[x, y].type = substitute;
                    return false;
                }
                // ����Ϊ2����ֻ����������
                if ((x < _mapWidth - 1 && _mapData[x + 1, y].type == check) &&
                 (x > 1 && _mapData[x - 1, y].type == check))
                {
                    _mapData[x, y].type = substitute;
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
    private bool CheckTileMapDataOutline(int x, int y, MapTileData.Type check, MapTileData.Type cannot, MapTileData.Type substitute)
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

    private int CalculatNeighborGrass(int x, int y, MapTileData.Type check)
    {
        int count = 0;
        if (y < _mapHeight - 1 && _mapData[x, y + 1].type == check) { count++; }
        if (y > 1 && _mapData[x, y - 1].type == check) { count++; }
        if (x < _mapWidth - 1 && _mapData[x + 1, y].type == check) { count++; }
        if (x > 1 && _mapData[x - 1, y].type == check) { count++; }
        return count;
    }

    #endregion

    public string GetData(Vector3 pos)
    {
        if (pos.x > _mapWidth || pos.x < 0) return null;
        if (pos.y > _mapHeight || pos.y < 0) return null;
        if (_mapData == null) return null;

        Vector3Int mouseCellPos = mainMap.WorldToCell(pos);

        string result = "";
        result += $"{_mapData[mouseCellPos.x, mouseCellPos.y].x} : {_mapData[mouseCellPos.x, mouseCellPos.y].y}\n";
        result += $"{_mapData[mouseCellPos.x, mouseCellPos.y].type}\n";
        result += $"{_mapData[mouseCellPos.x, mouseCellPos.y].aboveEmpty}\n";
        result += $"{_mapData[mouseCellPos.x, mouseCellPos.y].loadingTilemap.name}\n";

        return result;
    }
}
