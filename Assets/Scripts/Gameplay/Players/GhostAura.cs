using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAura : MonoBehaviour
{
    [SerializeField] private float rapidfireMultiplier = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto tocado implementa IHealable
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.attackCooldown = player.attackCooldown * rapidfireMultiplier;
            Debug.Log("DISPAREN");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.attackCooldown = player.attackCooldown / rapidfireMultiplier;
            Debug.Log("Alto al fuego");
        }
    }
}
