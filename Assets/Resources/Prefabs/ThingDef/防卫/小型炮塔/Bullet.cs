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

    private bool isHit = false;
    private ObjectPool<GameObject> _pool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// ±ª∑¢…‰
    /// </summary>
    public void Shot(ObjectPool<GameObject> pool)
    {
        _pool = pool;
        isHit = false;
        rb.velocity = transform.up * speed;
        StartCoroutine(ReturnToPoolAfterTime(lifetime));
    }

    public void Hit()
    {
        if(!isHit)
        {
            isHit = true;
            _pool.Release(gameObject);
        }
    }

    private IEnumerator ReturnToPoolAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (!isHit)
        {
            isHit = true;
            _pool.Release(gameObject);
        }
    }
}
