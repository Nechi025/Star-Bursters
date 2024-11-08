using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBullet : Bullet
{
    void Update()
    {
        velocity = direction * speed;
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        pos += velocity * Time.fixedDeltaTime;

        transform.position = pos;
    }
}
