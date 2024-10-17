using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy
{
    public abstract void Attack();
    public abstract void TakeDamage(int damage);
}

