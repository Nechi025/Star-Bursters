using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float timeToStartSpawning;
    [SerializeField] float timeBetweenSpawning;
    [SerializeField] StraightShooterFactory factory;

    bool readyToSpawn;
    float timer;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                timer += Time.deltaTime;
                if (timeToStartSpawning < timer)
                {
                    readyToSpawn = true;
                    timer = 0;
                }

                if (readyToSpawn && timer > timeBetweenSpawning)
                {
                    timer = 0;
                    factory.CreateEnemy();
                }
            }
        }
    }
}
