using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private PhotonView pv;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        UpdateScoreText();
    }

   
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    

    private void UpdateScoreText()
    {
        pv.RPC("LogicUpdate", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void LogicUpdate()
    {
        if (scoreText != null)
        {

            scoreText.text = "HS  " + score;

        }
    }
}

