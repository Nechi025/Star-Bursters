using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Assign the prefab in the Inspector

    void Start()
    {
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab);
        }
    }
}


