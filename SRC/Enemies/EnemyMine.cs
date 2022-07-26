using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMine : EnemyRound, IDamageable
{
    public bool set_target_down = false;

    // Use this for initialization
    protected override void Start()
    {
        // Call constructor of parent
        base.Start();

        if (set_target_down)
        {
            target_path = new List<Vector2>();
            target_path.Add(new Vector2(transform.position.x, despawn_y - 10));
        }
    }
}