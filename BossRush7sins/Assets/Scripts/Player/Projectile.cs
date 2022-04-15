using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float range;
    public float damage;
    [SerializeField]
    private LayerMask layerMask;

    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer * speed >= range)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == layerMask.value)
        {
            Destroy(gameObject);
        }
    }
}
