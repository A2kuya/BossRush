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
        // yield return new WaitForSeconds(fallingTime - 애니메이션시간);
        // 애니메이션 재생
        //yield return new WaitForSeconds(애니메이션시간);
        yield return new WaitForSeconds(1); // 임시시간
        col.enabled = true;
        isFalling = false;
        currentCo = null;
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime);
        // 폭발
        for (int i = 0; i < 8; i++)
        {
            Instantiate(projectile, transform.position, Quaternion.AngleAxis(i * 45, Vector3.forward));
        }
        Destroy(gameObject);
    }
}
