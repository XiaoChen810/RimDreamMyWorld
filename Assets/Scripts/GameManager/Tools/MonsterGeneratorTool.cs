using ChenChen_AI;
using ChenChen_Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterGeneratorTool : MonoBehaviour
{
    [Header("预制件")]
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    [Header("现存列表")]
    public List<Monster> MonstersList = new List<Monster>();    // 场景现有的怪物列表

    [Header("生成参数")]
    [SerializeField] private float spawnInterval = 15f; // 生成怪物的时间间隔
    [SerializeField] private int spawnCountMax = 15;  // 生成怪物的最大值
    [SerializeField] private float spawnAvoidAreaOfCenter = 25; // 中心点范围内避免生成的区域

    private float spawnTimer = 0f;  // 生成计时器
    private ObjectPool<GameObject> _monsterPool;

    #region - 生成 -

    /// <summary>
    /// 生成一个怪物，index为指定怪物，如果不设置index则会从池中随机挑选
    /// </summary>
    /// <param name="position"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GenerateMonster(Vector2 position, int index = -1)
    {
        GameObject result = null;

        if(index == -1)
        {
            result = _monsterPool.Get();
            result.transform.position = position;
            Monster m = result.GetComponent<Monster>();
            m.Init(_monsterPool);
            MonstersList.Add(m);
        }
        else
        {
            result = Instantiate(MonsterPrefabs[index], position, Quaternion.identity);
            Monster m = result.GetComponent<Monster>();
            m.IndexId = index;
            MonstersList.Add(m);
        }

        return result;
    }
    // 生成从存档
    public void LoadMonstersFromSave(Data_GameSave gameSave)
    {
        foreach(var save in gameSave.SaveMonster)
        {
            GenerateMonster(save.position, save.indexID);
        }
    }
    // 生成一批大量怪物
    public void SpawnMassiveMonsters(int num)
    {
        float mapWidth = MapManager.Instance.CurMapWidth;
        float mapHeight = MapManager.Instance.CurMapHeight;
        Vector2 center = new Vector2(mapWidth / 2, mapHeight / 2);
        for (int i = 0;i < num; i++)
        {
            Vector2 rangePosition;
            // 生成随机点
            do
            {
                rangePosition = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            } while (Vector2.Distance(rangePosition, center) < spawnAvoidAreaOfCenter);
            GenerateMonster(rangePosition);
        }
    }
    // 生成根据计时器
    private void SpawnMonsterInterval()
    {
        if (!GameManager.Instance.GameIsStart) return;
        if (MonstersList.Count >= spawnCountMax) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            float mapWidth = MapManager.Instance.CurMapWidth;
            float mapHeight = MapManager.Instance.CurMapHeight;
            Vector2 center = new Vector2(mapWidth / 2, mapHeight / 2);
            Vector2 rangePosition;

            // 生成随机点
            do
            {
                rangePosition = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            } while (Vector2.Distance(rangePosition, center) < spawnAvoidAreaOfCenter);

            // 生成怪物
            GenerateMonster(rangePosition);

            spawnTimer = 0f;  // 重置计时器
        }
    }
    
    
    #endregion

    #region Pool
    private GameObject PoolCreate()
    {
        int index = Random.Range(0, MonsterPrefabs.Count);
        GameObject newMonsterObj = Instantiate(MonsterPrefabs[index]);
        Monster newMonster = newMonsterObj.GetComponent<Monster>();
        newMonster.IndexId = index;
        return newMonsterObj;
    }

    private void PoolGet(GameObject monster)
    {
        monster.SetActive(true);
    }

    private void PoolRelease(GameObject monster)
    {
        monster.SetActive(false);
    }

    private void PoolDestroy(GameObject monster)
    {
        Destroy(monster);
    }
    #endregion

    private void Start()
    {
        _monsterPool = new ObjectPool<GameObject>(PoolCreate, PoolGet, PoolRelease, PoolDestroy, false);
    }

    private void Update()
    {
        bool isDayLight = (GameManager.Instance.currentHour >= 6 && GameManager.Instance.currentHour <= 18);
        if (!isDayLight)
        {
            SpawnMonsterInterval();
        }
    }

    
}
