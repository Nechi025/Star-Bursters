using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAura : MonoBehaviour
{
    public class HealingArea : MonoBehaviour
    {
        [SerializeField] int healingAmount = 5; //Cantidad de curación por ciclo
        [SerializeField] float healingInterval = 1f; //Intervalo en segundos entre curaciones

        private Coroutine healingCoroutine;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Te toque");
            IHealable healable = collision.GetComponent<IHealable>();
            if (healable != null)
            {
                //Inicia la curación constante
                healingCoroutine = StartCoroutine(HealOverTime(healable));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            IHealable healable = collision.GetComponent<IHealable>();
            if (healable != null && healingCoroutine != null)
            {
                //Detiene la curación cuando el jugador sale del área
                StopCoroutine(healingCoroutine);
                healingCoroutine = null;
            }
        }

        private IEnumerator HealOverTime(IHealable healable)
        {
            while (true)
            {
                healable.Heal(healingAmount);
                Debug.Log($"Jugador curado por {healingAmount}. Intervalo: {healingInterval} segundos.");
                yield return new WaitForSeconds(healingInterval);
            }
        }
    }
}
