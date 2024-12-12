using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyShot"))
        {
            Destroy(collision.gameObject);
        }
    }
}
