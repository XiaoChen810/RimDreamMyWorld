using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGeneratorTool : MonoBehaviour
{
    public List<GameObject> MonsterPrefabs = new List<GameObject>();

    private Transform monsterParent;

    public GameObject GenerateMaster(Vector2 position, int index = -1)
    {
        if (index == -1) index = Random.Range(0, MonsterPrefabs.Count);

        if (monsterParent == null)
        {
            monsterParent = new GameObject("Masters").transform;
            monsterParent.SetParent(transform, false);
        }
        return Instantiate(MonsterPrefabs[index], position, Quaternion.identity, monsterParent);
    }
}
