using ChenChen_AI;
using ChenChen_Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterGeneratorTool : MonoBehaviour
{
    [Header("Ԥ�Ƽ�")]
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    [Header("�ִ��б�")]
    public List<Monster> MonstersList = new List<Monster>();    // �������еĹ����б�

    [Header("���ɲ���")]
    [SerializeField] private float spawnInterval = 15f; // ���ɹ����ʱ����
    [SerializeField] private int spawnCountMax = 15;  // ���ɹ�������ֵ
    [SerializeField] private float spawnAvoidAreaOfCenter = 25; // ���ĵ㷶Χ�ڱ������ɵ�����

    private float spawnTimer = 0f;  // ���ɼ�ʱ��
    private ObjectPool<GameObject> _monsterPool;

    #region - ���� -

    /// <summary>
    /// ����һ�����indexΪָ��������������index���ӳ��������ѡ
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
    // ���ɴӴ浵
    public void LoadMonstersFromSave(Data_GameSave gameSave)
    {
        foreach(var save in gameSave.SaveMonster)
        {
            GenerateMonster(save.position, save.indexID);
        }
    }
    // ����һ����������
    public void SpawnMassiveMonsters(int num)
    {
        float mapWidth = MapManager.Instance.CurMapWidth;
        float mapHeight = MapManager.Instance.CurMapHeight;
        Vector2 center = new Vector2(mapWidth / 2, mapHeight / 2);
        for (int i = 0;i < num; i++)
        {
            Vector2 rangePosition;
            // ���������
            do
            {
                rangePosition = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            } while (Vector2.Distance(rangePosition, center) < spawnAvoidAreaOfCenter);
            GenerateMonster(rangePosition);
        }
    }
    // ���ɸ��ݼ�ʱ��
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

            // ���������
            do
            {
                rangePosition = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            } while (Vector2.Distance(rangePosition, center) < spawnAvoidAreaOfCenter);

            // ���ɹ���
            GenerateMonster(rangePosition);

            spawnTimer = 0f;  // ���ü�ʱ��
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
