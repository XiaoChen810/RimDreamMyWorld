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
        // 随机选择一个精灵
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[Random.Range(0, sprites.Count)];
        sr.color = color;
        sr.DOFade(color.a, 1f);

        // 随机设定一个移动方向
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // 随机大小
        transform.localScale = new Vector3(Random.Range(2, 4f), Random.Range(1.5f, 2.3f));

        // 初始化计时器
        timer = time;
    }

    private void Update()
    {
        // 移动云朵
        transform.Translate(direction * speed * Time.deltaTime);

        // 计时器递减
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            sr.DOFade(0, 2).OnComplete(() =>
            {
                // 时间到，销毁对象
                Destroy(gameObject);
            });

        }
    }

    private void OnDestroy()
    {
        sr.DOKill();
    }
}

