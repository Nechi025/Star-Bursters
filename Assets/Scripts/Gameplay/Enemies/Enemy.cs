using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float AttackSpeed;
    public float AttackInterval;
    public List<Transform> PathPoints;
    public List<Transform> FiringPoints;

    protected float nextAttackTime;

    protected virtual void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + AttackInterval;
        }
    }

    public abstract void Attack();
}

