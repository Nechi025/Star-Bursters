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

    public void ShootLaser(Transform player)
    {
        Vector3 start = (Vector2)player.position + (Vector2)player.up * (laserWidth / 2);
        Vector3 end = (Vector2)player.position + (Vector2)player.up * (laserHeight);

        // Configurar la línea localmente
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
        pv.RPC("TurnOffLaserRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void TurnOffLaserRPC()
    {
        lineRenderer.enabled = false;
    }
}
