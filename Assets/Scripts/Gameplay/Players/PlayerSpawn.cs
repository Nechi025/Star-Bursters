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

    private void Start()
    {
        player = PhotonNetwork.Instantiate(ghostPrefab.name,
                            new Vector2(Random.Range(-4, 4), Random.Range(-4, -2)),
                            Quaternion.identity);

        int playerIndex = PhotonNetwork.PlayerList.Length;

        //pv.RPC("ChangeColor", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, playerIndex);
    }

    [PunRPC]
    private void ChangeColor(int playerViewID, int playerIndex)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);

        if (targetPhotonView != null)
        {
            targetPhotonView.gameObject.GetComponent<SpriteRenderer>().color = (playerIndex == 1) ? Color.red : Color.blue;
        }
    }
}