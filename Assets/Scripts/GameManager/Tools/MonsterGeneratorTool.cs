using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGeneratorTool : MonoBehaviour
{
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    public List<Monster> MonstersList = new List<Monster>();

    private Transform monsterParent;

    public GameObject GenerateMaster(Vector2 position, int index = -1)
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
            GenerateMaster(save.position, save.indexID);
        }
    }
}
