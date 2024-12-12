using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float AttackSpeed;
    public float AttackInterval;
    public List<Transform> PathPoints;
    public List<Transform> FiringPoints;
    public int AttackAmount;
    protected int currentTargetIndex = 0;
    protected float nextAttackTime;
    private bool isAttacking;
    public float RotationSpeed = 180f;
    public int ScoreValue;
    public GameObject enemyBody;


    protected virtual void Update()
    {
        MoveAlongPath();
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(AttackBurst());
                nextAttackTime = Time.time + AttackInterval;
            }
        }

        

        if (Health <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ScoreManager.Instance.AddScore(ScoreValue);
            }
            Destroy(enemyBody);
           
        }
    }

    protected void MoveAlongPath()
    {
        if (PathPoints != null && PathPoints.Count > 0)
        {
            Transform targetPoint = PathPoints[currentTargetIndex];
            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

            float targetRotation = targetPoint.eulerAngles.z;
            float newRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetRotation, RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newRotation);


            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentTargetIndex++;

         
                if (currentTargetIndex >= PathPoints.Count)
                {
                    currentTargetIndex = 0;
                }
            }
        }
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null && !bullet.CompareTag("EnemyShot"))
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
