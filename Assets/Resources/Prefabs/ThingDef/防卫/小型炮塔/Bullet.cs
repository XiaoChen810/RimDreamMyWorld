using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float lifetime = 5f;
    public int damage = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// ±ª∑¢…‰
    /// </summary>
    public void Shot(ObjectPool<GameObject> pool)
    {
        rb.velocity = transform.up * speed;
        StartCoroutine(ReturnToPoolAfterTime(pool, lifetime));
    }

    private IEnumerator ReturnToPoolAfterTime(ObjectPool<GameObject> pool, float time)
    {
        yield return new WaitForSeconds(time);
        pool.Release(gameObject);
    }
}
