using ChenChen_Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��Ҫ�������ɵ���Ч����
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
    [Header("��Ч��������")]
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

        GenerateEffect("����", 5);
        StartCoroutine(CloudsCo());  
        //StartCoroutine(LightRaysCo());
    }

    IEnumerator CloudsCo()
    {     
        while (true)
        {
            GenerateEffect("�ƶ�");
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.name == "�ƶ�") count++;
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
                if (child.name == "����") count++;
            }
            if (count < 5)
            {
                GenerateEffect("����");
                float time = 15f;
                yield return new WaitForSeconds(time);
            }
        }
    }

    private void GenerateEffect(string effectName,int num = 1)
    {
        EffectData effect = _effectsList.FirstOrDefault(x => x.name == effectName);
        if (effect.prefab == null) return;
        // �Ѿ����ɵ���Ч��λ��
        List<Vector2> vector2s = new();
        foreach (Transform child in transform)
        {
            if (child.name == effectName) vector2s.Add(child.position);
        }
        // ��ֹ����ѭ��
        int count = 0;
        int flag = 0;
        while (count < num)
        {
            // �������һ��λ��
            Vector2 pos = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
            // ���ڵ�λ�ã�������ͬһ��λ�ã��������Ҳ����С��effect.spacing
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
                flag = 0; // ����flag����Ϊ�ɹ�������һ����Чλ��
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
