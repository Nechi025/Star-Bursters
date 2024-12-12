using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public int damage;
    public float speed;
    public bool isEnemy = false;
    public List<Transform> PathPoints;
    private int currentTargetIndex = 0;

    [SerializeField] private float maxLifetime = 5f; 
    private float lifetime = 0f;

    private void Update()
    {

        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            DestroyBullet();
            return;
        }

        
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
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BulletDestroy"))
        {
            DestroyBullet();
        }
        if (collision.gameObject.CompareTag("BulletEnemyDestroy") && isEnemy)
        {
            DestroyBullet();
        }
    }

    [PunRPC]
    public void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void IncreaseDamage(int amount, float duration)
    {
        damage += amount;

    
        if (duration > 0)
        {
            StartCoroutine(RevertDamageAfterDuration(amount, duration));
        }
    }

    private IEnumerator RevertDamageAfterDuration(int amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        damage -= amount;
    }



}


