using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviourPun
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    private int score = 0;

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
    }

    private void Start()
    {
        UpdateScoreText();
    }

    
    public void AddScore(int amount)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            score += amount;
            photonView.RPC("UpdateScoreRPC", RpcTarget.All, score);
        }
    }

    [PunRPC]
    private void UpdateScoreRPC(int updatedScore)
    {
        
        score = updatedScore;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score   " + score;
        }
    }
}
