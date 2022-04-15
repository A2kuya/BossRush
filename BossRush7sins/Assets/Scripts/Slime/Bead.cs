using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bead : MonoBehaviour
{
    public float explodeTime;
    public GameObject projectile;

    private void Start()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime);
        for (int i = 0; i < 8; i++)
        {
            Instantiate(projectile, transform.position, Quaternion.AngleAxis(i * 45, Vector3.forward));
            Destroy(gameObject);
        }
    }
}
