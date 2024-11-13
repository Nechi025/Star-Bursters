using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float speed;
    public List<Transform> PathPoints; 
    private int currentTargetIndex = 0;

    void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (PathPoints == null || PathPoints.Count == 0) return;
        Transform targetPoint = PathPoints[currentTargetIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;


        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentTargetIndex++;

            if (currentTargetIndex >= PathPoints.Count)
            {
                Destroy(gameObject);
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("BulletDestroy"))
        {
            Destroy(this.gameObject);
        }

    }
}


