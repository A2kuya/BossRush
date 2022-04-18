using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float delay;
    public float lifeTime;
    public float damage;
    [SerializeField]
    private LayerMask layerMask;

    private CircleCollider2D col;
    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        timer = 0;
    }

    private void Start()
    {
        col.enabled = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delay)
        {
            col.enabled = true;
        }
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == layerMask.value)
        {
            // 데미지 입히기;
        }
    }
}
