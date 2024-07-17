using ChenChen_Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 需要程序生成的特效数据
/// </summary>
[Serializable]
public struct EffectData
{
    public GameObject prefab;

    public string name;

    public int spacing;
}

public class LightEffect : MonoBehaviour
{
    [Header("特效生成数据")]
    [SerializeField] private List<EffectData> _effectsList;

    private Transform effectParent;

    public void Start()
    {
        effectParent = transform.Find("Effect");
        if (effectParent == null)
        {
            var go = new GameObject("Effect");
            go.transform.parent = this.transform;
            effectParent = go.transform;
        }

        GenerateEffect("阳光", 5);
        StartCoroutine(CloudsCo());  
        //StartCoroutine(LightRaysCo());
    }

    IEnumerator CloudsCo()
    {     
        while (true)
        {
            GenerateEffect("云朵");
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.name == "云朵") count++;
            }
            if (count > 10)
            {
                float time = UnityEngine.Random.Range(25, 35f);
                yield return new WaitForSeconds(time);
            }
            else
            {
                float time = UnityEngine.Random.Range(5, 15f);
                yield return new WaitForSeconds(time);
            }
        }
    }

    IEnumerator LightRaysCo()
    {
        while (true)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.name == "阳光") count++;
            }
            if (count < 5)
            {
                GenerateEffect("阳光");
                float time = 15f;
                yield return new WaitForSeconds(time);
            }
        }
    }

    private void GenerateEffect(string effectName,int num = 1)
    {
        EffectData effect = _effectsList.FirstOrDefault(x => x.name == effectName);
        if (effect.prefab == null) return;
        // 已经生成的特效的位置
        List<Vector2> vector2s = new();
        foreach (Transform child in transform)
        {
            if (child.name == effectName) vector2s.Add(child.position);
        }
        // 防止无限循环
        int count = 0;
        int flag = 0;
        while (count < num)
        {
            // 随机生成一个位置
            Vector2 pos = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
            // 检查节点位置，不会在同一个位置，间隔距离也不会小于effect.spacing
            bool validPosition = true;
            foreach (var vec in vector2s)
            {
                if (Vector2.Distance(pos, vec) < effect.spacing)
                {
                    validPosition = false;
                    break;
                }
            }
            if (validPosition)
            {
                GameObject obj = Instantiate(effect.prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, effectParent.transform);
                obj.name = effectName;
                vector2s.Add(pos);
                flag = 0; // 重置flag，因为成功生成了一个有效位置
                count++;
            }
            else
            {
                flag++;
            }
            if (flag >= 1000)
            {
                Debug.LogWarning("Failed to generate sufficient effects without overlap.");
                break;
            }
        }
    }
}
