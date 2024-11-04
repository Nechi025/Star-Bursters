using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShooterFactory : IEnemyFactory
{
    public Enemy CreateEnemy()
    {
        GameObject enemyPrefab = Resources.Load<GameObject>("StraightShooterPrefab");
        if (enemyPrefab != null)
        {
            GameObject enemyInstance = Object.Instantiate(enemyPrefab);
            return enemyInstance.GetComponent<Enemy>();
        }
        else
        {
            Debug.LogError("StraightShooterPrefab not found in Resources.");
            return null;
        }
    }
}
