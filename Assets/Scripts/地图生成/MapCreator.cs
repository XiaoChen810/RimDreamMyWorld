using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreatorTileData
{
    // 位置
    public int x;
    public int y;

    // 瓦片类型
    public enum Type
    {
        none,grass,water,ground,mountain
    }
    public Type type = Type.none;

    // 噪声值
    public float noiseValue;

    // 附属的瓦片地图
    public Tilemap loadingTilemap;

    // 瓦片上是否有物体
    public bool aboveEmpty;
}


public class MapCreator : MonoBehaviour 
{
    [Header("生成器基本信息")]
    public Tilemap mainMap;
    public TileBase mainMapDefaultTileBase;
    public Tilemap leavesMap;

    public int height;
    public int width;

    public float lacunarrty;

    public int seed;
    public bool useRandomSeed;

    [System.Serializable]
    public struct TileProbability
    {
        [Range(0, 1f)]
        public float probability;

        public TileBase tile;

        public MapCreatorTileData.Type type;

        public Tilemap loadingTilemap;
    }
    [Header("瓦片生成")]
    public List<TileProbability> tileList;

    public MapCreatorTileData[,] mapData;

    [Header("花草生成")]
    [Range(0, 1f)]
    public float leavesProbability;
    public List<TileBase> leavesTileBase = new List<TileBase>();

    private bool useCheck = true;
    private int checkNumber = 100;

    [System.Serializable]
    public struct ItemProbability
    { 
        public string name;
        [Range(0, 1f)] public float probability;
        public int number;
        public bool useNumber;
        public GameObject prefab;
        public MapCreatorTileData.Type environment;
        public Vector3 offset;
    }
    [Header("物体生成")]
    public List<ItemProbability> itemList;


    public void GenerateMap()
    {
        GenerateMapData();

        GenerateTileMap();
        Debug.Log("Create Map Finish");
    }

    #region 生成地图数据，设置瓦片

    public void GenerateMapData()
    {
        // 计算噪声值
        if (!useRandomSeed) seed = Time.time.GetHashCode();
        UnityEngine.Random.InitState(seed);

        mapData = new MapCreatorTileData[height,width];

        float randomOffset = UnityEngine.Random.Range(-1000, 1000);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                mapData[x, y] = new MapCreatorTileData();
                mapData[x, y].x = x;
                mapData[x, y].y = y;
                mapData[x, y].noiseValue = nosieValue;
                if(nosieValue < minValue) minValue = nosieValue;
                if(nosieValue > maxValue) maxValue = nosieValue;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nosieValue = Mathf.PerlinNoise(x * lacunarrty + randomOffset, y * lacunarrty + randomOffset);

                mapData[x, y].noiseValue = Mathf.Lerp(minValue, maxValue, nosieValue);
                foreach (var t in tileList)
                {
                    if (mapData[x, y].noiseValue <= t.probability)
                    {
                        mapData[x, y].type = t.type;
                        mapData[x, y].loadingTilemap = t.loadingTilemap;
                        break;
                    }
                }
            }
        }


        if (useCheck) Check();
    }

    public void GenerateTileMap()
    {
        CleanMap();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile = GetMapDataType(x, y);
                mainMap.SetTile(new Vector3Int(x, y), mainMapDefaultTileBase);

                if (tile != null)
                {
                    mapData[x, y].loadingTilemap.SetTile(new Vector3Int(x, y), tile);
                }
            }
        }

        // 生成花草
        GenerateLeaveAndFlowers();

        // 生成树木
        GenerateTrees("树");
    }

    private void GenerateTrees(string treeName)
    {
        ItemProbability tree = itemList.FirstOrDefault(item => item.name == treeName);
        if (tree.prefab != null)
        {
            int count = 0;
            List<MapCreatorTileData> theEnvironmentTile = new List<MapCreatorTileData>();
            foreach (var md in mapData)
            {
                if (md.type == tree.environment && CalculatNeighborGrass(md.x, md.y, MapCreatorTileData.Type.water) <= 0)
                {
                    theEnvironmentTile.Add(md);
                    count++;
                }
            }

            if (tree.useNumber)
            {
                for (int i = 0; i < tree.number; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, theEnvironmentTile.Count);
                    Vector3Int tilePos = new Vector3Int(theEnvironmentTile[randomIndex].x, theEnvironmentTile[randomIndex].y);
                    Vector3 createPos = tilePos + tree.offset;
                    Instantiate(tree.prefab, createPos, Quaternion.identity, this.transform);
                    mapData[tilePos.x, tilePos.y].aboveEmpty = false;
                }
            }
            else
            {
                for (int i = 0; i < count * tree.probability; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, theEnvironmentTile.Count);
                    Vector3Int tilePos = new Vector3Int(theEnvironmentTile[randomIndex].x, theEnvironmentTile[randomIndex].y);
                    Vector3 createPos = tilePos + tree.offset;
                    Instantiate(tree.prefab, createPos, Quaternion.identity, this.transform);
                    mapData[tilePos.x, tilePos.y].aboveEmpty = false;
                }
            }

        }
    }

    private void GenerateLeaveAndFlowers()
    {
        // 统计有多少块草地
        int count = 0;
        List<MapCreatorTileData> theGrassTile = new List<MapCreatorTileData>();
        foreach (var md in mapData)
        {
            if (md.type == MapCreatorTileData.Type.grass)
            {
                theGrassTile.Add(md);
                count++;
            }
        }

        for (int i = 0; i < count * leavesProbability; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, theGrassTile.Count);
            leavesMap.SetTile(new Vector3Int(theGrassTile[randomIndex].x, theGrassTile[randomIndex].y)
                , leavesTileBase[UnityEngine.Random.Range(0, leavesTileBase.Count)]);
        }
    }

    private TileBase GetMapDataType(int x, int y)
    {
        foreach (var t in tileList)
        {
            if (t.type == mapData[x, y].type) return t.tile;
        }
        return null;
    }

    #endregion

    #region 检查地图数据，剔除不合理内容

    public void Check()
    {
        bool flag = false;
        for (int i = 0; i < checkNumber; i++)
        {
            flag = true;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!CheckTileMapDataIndividual(x, y, MapCreatorTileData.Type.grass, MapCreatorTileData.Type.water)) flag = false;
                    if (!CheckTileMapDataIndividual(x, y, MapCreatorTileData.Type.ground, MapCreatorTileData.Type.none)) flag = false;
                    if (!CheckTileMapDataOutline(x, y, MapCreatorTileData.Type.ground, MapCreatorTileData.Type.water, MapCreatorTileData.Type.none)) flag = false;
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
    public bool CheckTileMapDataIndividual(int x,int y ,MapCreatorTileData.Type check, MapCreatorTileData.Type substitute)
    {

        if (mapData[x, y].type == check)
        {
            int count = CalculatNeighborGrass(x, y, check);

            if (count <= 1)
            {
                mapData[x, y].type = substitute;
                return false;
            }

            if (count == 2)
            {
                // 计数为2且是只有上下两格
                if ((y < height - 1 && mapData[x, y + 1].type == check) &&
                (y > 1 && mapData[x, y - 1].type == check))
                {
                    mapData[x, y].type = substitute;
                    return false;
                }
                // 计数为2且是只有左右两格
                if ((x < width - 1 && mapData[x + 1, y].type == check) &&
                 (x > 1 && mapData[x - 1, y].type == check))
                {
                    mapData[x, y].type = substitute;
                    return false;
                }
            }

        }

        return true;
    }

    public bool CheckTileMapDataOutline(int x, int y, MapCreatorTileData.Type check, MapCreatorTileData.Type cannot, MapCreatorTileData.Type substitute)
    {

        if (mapData[x, y].type == check)
        {
            int count = CalculatNeighborGrass(x, y, cannot);

            if (count > 0)
            {
                mapData[x, y].type = substitute;
                return false;
            }
        }
        return true;
    }

    private int CalculatNeighborGrass(int x, int y, MapCreatorTileData.Type check)
    {
        int count = 0;
        if (y < height - 1 && mapData[x, y + 1].type == check) { count++; }
        if (y > 1 && mapData[x, y - 1].type == check) { count++; }
        if (x < width - 1 && mapData[x + 1, y].type == check) { count++; }
        if (x > 1 && mapData[x - 1, y].type == check) { count++; }
        return count;
    }

    #endregion

    public void CleanMap()
    {
        // 清理瓦片
        foreach(var t in tileList)
        {
            t.loadingTilemap.ClearAllTiles();
        }
        leavesMap.ClearAllTiles();

        // 清理环境
#if UNITY_EDITOR

        // 存储子物体的引用
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            childrenToDestroy.Add(child);
        }

        // 销毁子物体
        foreach (Transform child in childrenToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }

#else

    // 在运行时清理子物体
    foreach (Transform child in this.transform)
    {
        Destroy(child.gameObject);
    }

#endif

        Debug.Log("Clear All");
    }
}
