using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public List<Sprite> sprites;
    public float speed;
    public float time;
    public Color color;

    private SpriteRenderer sr;
    private Vector2 direction;
    private float timer;

    private void Start()
    {
        // ���ѡ��һ������
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[Random.Range(0, sprites.Count)];
        sr.color = color;
        sr.DOFade(color.a, 1f);

        // ����趨һ���ƶ�����
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // �����С
        transform.localScale = new Vector3(Random.Range(2, 4f), Random.Range(1.5f, 2.3f));

        // ��ʼ����ʱ��
        timer = time;
    }

    private void Update()
    {
        // �ƶ��ƶ�
        transform.Translate(direction * speed * Time.deltaTime);

        // ��ʱ���ݼ�
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            sr.DOFade(0, 2).OnComplete(() =>
            {
                // ʱ�䵽�����ٶ���
                Destroy(gameObject);
            });

        }
    }

    private void OnDestroy()
    {
        sr.DOKill();
    }
}

