using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public enum characters
{
    ghost,
    healer,
    tank,
    recon
}


public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject healerPrefab;
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject reconPrefab;

    private GameObject player;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SpawnCharacter(characters selectedCharacter)
    {
        GameObject prefabToSpawn = selectedCharacter switch
        {
            characters.ghost => ghostPrefab,
            characters.healer => healerPrefab,
            characters.tank => tankPrefab,
            characters.recon => reconPrefab,
            _ => ghostPrefab // Valor por defecto si algo falla
        };

        player = PhotonNetwork.Instantiate(prefabToSpawn.name,
                            new Vector2(Random.Range(-4, 4), Random.Range(-4, -2)),
                            Quaternion.identity);
    }
}