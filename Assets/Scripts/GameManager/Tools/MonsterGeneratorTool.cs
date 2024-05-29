using ChenChen_AI;
using ChenChen_Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGeneratorTool : MonoBehaviour
{
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    public List<Monster> MonstersList = new List<Monster>();

    private Transform monsterParent;
    private float spawnTimer = 0f;  // ���ڿ�������Ƶ��
    public float spawnInterval = 15f; // ���ɹ����ʱ����
    public int spawnCountMax = 15;  // ���ɹ�������ֵ

    public GameObject GenerateMonster(Vector2 position, int index = -1)
    {
        if (index == -1) index = Random.Range(0, MonsterPrefabs.Count);

        if (monsterParent == null)
        {
            monsterParent = new GameObject("Masters").transform;
            monsterParent.SetParent(transform, false);
        }
        GameObject newMonsterObj = Instantiate(MonsterPrefabs[index], position, Quaternion.identity, monsterParent);
        Monster newMonster = newMonsterObj.GetComponent<Monster>();
        newMonster.IndexId = index;
        MonstersList.Add(newMonster);
        return newMonsterObj;
    }

    public void LoadMonstersFromSave(Data_GameSave gameSave)
    {
        foreach(var save in gameSave.SaveMonster)
        {
            GenerateMonster(save.position, save.indexID);
        }
    }


    private void Update()
    {
        SpawnMonsterInterval();
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
