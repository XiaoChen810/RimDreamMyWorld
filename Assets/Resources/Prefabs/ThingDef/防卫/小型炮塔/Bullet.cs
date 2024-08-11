using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 _destination;
    private float speed = 0f;

    private float lifetime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, _destination) <= 0.5f)
        {
            Destroy(gameObject);
        }
        lifetime += Time.deltaTime;

        if (lifetime > 5f)
        {
            Destroy(gameObject);
        }
    }

    public void Shot(Vector2 destination)
    {
        _destination = destination;

        // ���㷽����ת��Ŀ���
        Vector2 direction = (_destination - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // ����
        speed = 30f;
        rb.velocity = direction * speed;
    }
}
