using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReconLaser : MonoBehaviour
{
    public LineRenderer lineRenderer; // Referencia al LineRenderer
    private PhotonView pv;
    public float laserHeight = 5f;    // Longitud máxima del láser
    public float laserWidth = 1.1f;   // Desplazamiento inicial del láser
    public float damageInterval = 0.5f; // Intervalo entre cada aplicación de daño
    public float damageAmount = 10f;   // Cantidad de daño por intervalo
    public LayerMask collisionLayers;

    private Coroutine damageCoroutine;

    public void ShootLaser(Transform player)
    {
        Vector3 start = (Vector2)player.position + (Vector2)player.up * (laserWidth / 2);
        Vector3 end = (Vector2)player.position + (Vector2)player.up * (laserHeight);

        // Configurar la línea localmente
        UpdateLaserPositions(start, end);

        // Verificar colisión con enemigos
        RaycastHit2D hit = Physics2D.Raycast(start, (Vector2)(end - start).normalized, laserHeight, collisionLayers);

        if (hit.collider != null)
        {
            // El láser choca con algo: ajustar su posición
            end = hit.point;

            // Si es un enemigo, comenzar a aplicar daño
            if (hit.collider.CompareTag("Enemy"))
            {
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(ApplyDamageOverTime(hit.collider.GetComponent<Enemy>()));
                }
            }
            else
            {
                StopDamageCoroutine(); // No es un enemigo, detener el daño
            }
        }
        else
        {
            StopDamageCoroutine(); // No hay colisión, detener el daño
        }

        // Configurar las posiciones del láser localmente
        UpdateLaserPositions(start, end);

        // Enviar datos a los demás jugadores
        pv.RPC("SyncLaser", RpcTarget.Others, start, end);
    }

    // Método RPC para sincronizar la línea
    [PunRPC]
    public void SyncLaser(Vector3 start, Vector3 end)
    {
        UpdateLaserPositions(start, end);
    }

    // Configuración de las posiciones de la línea
    private void UpdateLaserPositions(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    // Método para apagar el láser
    public void TurnOffLaser()
    {
        lineRenderer.enabled = false;
        StopDamageCoroutine();
        pv.RPC("TurnOffLaserRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void TurnOffLaserRPC()
    {
        lineRenderer.enabled = false;
    }

    // Coroutine para aplicar daño
    private IEnumerator ApplyDamageOverTime(Enemy enemy)
    {
        while (enemy != null && lineRenderer.enabled)
        {
            enemy.Health -= damageAmount; // Aplica daño al enemigo
            yield return new WaitForSeconds(damageInterval); // Espera el intervalo antes de aplicar daño de nuevo
        }
    }

    // Detener el Coroutine de daño
    private void StopDamageCoroutine()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
}
