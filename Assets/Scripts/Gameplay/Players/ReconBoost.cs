using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReconBoost : MonoBehaviour
{
    [SerializeField] private int damageIncrease = 10; // Amount to increase bullet damage
    [SerializeField] private float boostDuration = 5f; // Optional: Duration of boosted bullets

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is a bullet
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && !bullet.isEnemy) // Ensure it's a player bullet
        {
            PhotonView pv = bullet.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Apply the damage increase via RPC
                pv.RPC("IncreaseDamage", RpcTarget.AllBuffered, damageIncrease, boostDuration);
            }
        }
    }
}
