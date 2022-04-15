using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float thrust;
    [SerializeField] private float knockTime;
    [SerializeField] private string otherTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(otherTag))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                // �˹� ������ �����¿�밢 �������� ���� ����ó�� ���� ���.
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
