using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public float fallingTime;
    public Vector3 targetPos;
    //private Vector3 startPos;

    private Coroutine currentCo;
    
    private bool isFalling;
    private BoxCollider2D col;

    public float explodeTime;
    public GameObject projectile;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        isFalling = true;
        col.enabled = false;
        currentCo = StartCoroutine(Falling());
    }

    void Update()
    {
        if (!isFalling && currentCo == null)
        {
            currentCo = StartCoroutine(Explode());
        }
    }

    private IEnumerator Falling()
    {
        // yield return new WaitForSeconds(fallingTime - �ִϸ��̼ǽð�);
        // �ִϸ��̼� ���
        //yield return new WaitForSeconds(�ִϸ��̼ǽð�);
        yield return new WaitForSeconds(1); // �ӽýð�
        col.enabled = true;
        isFalling = false;
        currentCo = null;
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime);
        // ����
        for (int i = 0; i < 8; i++)
        {
            Instantiate(projectile, transform.position, Quaternion.AngleAxis(i * 45, Vector3.forward));
        }
        Destroy(gameObject);
    }
}
