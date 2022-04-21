using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float thrust;
    [SerializeField] private float knockTime;
    [SerializeField] private string otherTag;
    [SerializeField] private float coolTime;
    private float timer;
    private bool canDamagable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = coolTime;
        canDamagable = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= coolTime)
        {
            canDamagable = true;
            rb.WakeUp();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canDamagable)
        {
            return;
        }

        if (other.gameObject.CompareTag(otherTag))
        {
            rb.Sleep();
            canDamagable = false;
            timer = 0;

            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                // 넉백 방향을 상하좌우대각 고정으로 할지 지금처럼 할지 고민.
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (other.gameObject.CompareTag("Player"))
                {
                    if (other.GetComponent<PlayerController>().state != PlayerController.State.Stagger)
                    {
                        hit.GetComponent<PlayerController>().state = PlayerController.State.Stagger;
                        other.GetComponent<PlayerController>().Knockback(knockTime);
                    }
                }
            }
        }
    }
}
