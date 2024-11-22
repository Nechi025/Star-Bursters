using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingObject : MonoBehaviour
{
    [SerializeField] private int healingAmount = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto tocado implementa IHealable
        IHealable healable = collision.GetComponent<IHealable>();
        if (healable != null)
        {
            healable.Heal(healingAmount);
            Debug.Log("Curo al jugador");
        }
    }
}
