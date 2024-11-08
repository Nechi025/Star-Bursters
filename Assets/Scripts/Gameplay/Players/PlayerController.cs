using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    public Vector2 Hrange = Vector2.zero;
    public Vector2 Vrange = Vector2.zero;
    public Bullet bullet;
    public List<Transform> FiringPoints;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += -Vector3.up * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += -Vector3.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * 5 * Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, Vrange.x, Vrange.y),
            Mathf.Clamp(transform.position.y, Hrange.x, Hrange.y),
            transform.position.z
            );
    }

    public void Shoot()
    {
        foreach (var point in FiringPoints)
        {
            GameObject go = PhotonNetwork.Instantiate(bullet.name, point.position, point.rotation);
        }
    }
        
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pv.IsMine && collision.transform.CompareTag("Coin"))
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("CollectCoin", RpcTarget.AllBuffered, collision.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void CollectCoin(int coinViewID)
    {
        PhotonView coinPhotonView = PhotonView.Find(coinViewID);
        if (coinPhotonView != null)
        {
            PhotonNetwork.Destroy(coinPhotonView.gameObject);
            GameManager.instance.AddCoinToPool();
        }
    }*/
}