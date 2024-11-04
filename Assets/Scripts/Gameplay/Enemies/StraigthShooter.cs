using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShooter : Enemy
{
    public GameObject BulletPrefab;

    public override void Attack()
    {
        if (BulletPrefab != null)
        {
            foreach (var point in FiringPoints)
            {
                GameObject bullet = Instantiate(BulletPrefab, point.position, point.rotation);
                // Assuming the bullet script handles its own movement.
                bullet.GetComponent<Bullet>().SetDirection(Vector2.up); // Adjust as needed
            }
        }
        else
        {
            Debug.LogError("BulletPrefab not assigned.");
        }
    }
}
