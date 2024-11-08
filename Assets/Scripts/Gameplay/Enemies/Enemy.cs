using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float AttackSpeed;
    public float AttackInterval;
    public List<Transform> PathPoints;
    public List<Transform> FiringPoints;
    public int AttackAmount;

    protected float nextAttackTime;
    private bool isAttacking;

    protected virtual void Update()
    {
        MoveAlongPath();


        if (Time.time >= nextAttackTime && !isAttacking)
        {
            StartCoroutine(AttackBurst());
            nextAttackTime = Time.time + AttackInterval;
        }

        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    protected void MoveAlongPath()
    {
        if (PathPoints != null && PathPoints.Count > 0)
        {
            Transform targetPoint = PathPoints[0];
            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

            
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                PathPoints.RemoveAt(0);
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null)
        {
            Health -= bullet.damage;
            Destroy(bullet.gameObject);
        }
    }

    protected IEnumerator AttackBurst()
    {
        isAttacking = true;
        for (int i = 0; i < AttackAmount; i++)
        {
            Attack();
            yield return new WaitForSeconds(AttackSpeed);
        }
        isAttacking = false;
    }

    public abstract void Attack();
}
