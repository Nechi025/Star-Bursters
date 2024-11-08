using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StraightShooterFactory : MonoBehaviour, IEnemyFactory
{

    [SerializeField] GameObject enemyPrefab;

    public Enemy CreateEnemy()
    {
        GameObject enemyInstance = PhotonNetwork.Instantiate(enemyPrefab.name, new Vector2(Random.Range(-4, 4), Random.Range(2, 4)), Quaternion.identity);
        return enemyInstance.GetComponent<Enemy>();
    }
}
