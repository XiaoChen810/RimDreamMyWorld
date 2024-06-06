using ChenChen_AI;
using ChenChen_Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterGeneratorTool : MonoBehaviour
{
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    /// <summary>
    /// �������еĹ����б�
    /// </summary>
    public List<Monster> MonstersList = new List<Monster>();

    private Transform monsterParent;
    private float spawnTimer = 0f;  // ���ڿ�������Ƶ��
    public float spawnInterval = 15f; // ���ɹ����ʱ����
    public int spawnCountMax = 15;  // ���ɹ�������ֵ

    private ObjectPool<GameObject> _monsterPool;

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
            result = Instantiate(MonsterPrefabs[index], position, Quaternion.identity, monsterParent);
            Monster m = result.GetComponent<Monster>();
            m.IndexId = index;
            MonstersList.Add(m);
        }

        return result;
    }

    public void LoadMonstersFromSave(Data_GameSave gameSave)
    {
        foreach(var save in gameSave.SaveMonster)
        {
            GenerateMonster(save.position, save.indexID);
        }
    }

    private void Start()
    {
        if (monsterParent == null)
        {
            monsterParent = new GameObject("Masters").transform;
            monsterParent.SetParent(transform, false);
        }

        _monsterPool = new ObjectPool<GameObject>(PoolCreate, PoolGet, PoolRelease, PoolDestroy, false);
    }

    #region Pool
    private GameObject PoolCreate()
    {
        int index = Random.Range(0, MonsterPrefabs.Count);
        GameObject newMonsterObj = Instantiate(MonsterPrefabs[index], monsterParent);
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
        PoolDestroy(monster);
    }
    #endregion

    private void Update()
    {
        bool isDayLight = (GameManager.Instance.currentHour >= 6 && GameManager.Instance.currentHour <= 18);
        if (!isDayLight)
        {
            SpawnMonsterInterval();
        }
    }

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
            float centerRange = 25f;   // ���ĵ㷶Χ�ڲ�������
            Vector2 rangePosition;

            // ���������
            do
            {
                rangePosition = new Vector2(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            } while (Vector2.Distance(rangePosition, center) < centerRange);

            // ���ɹ���
            GenerateMonster(rangePosition);

            spawnTimer = 0f;  // ���ü�ʱ��
        }
    }
}
