using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandom : Enemy, IDamageable
{

    public int target_length = 10;

    // Use this for initialization
    protected override void Start()
    {

        // Call constructor of parent
        base.Start();

        // Set some random targets across the map and then go away
        target_path = new List<Vector2>();
        for (int i = 0; i < target_length - 1; i++)
        {
            float x = Random.Range(map_manager.down_left.x, map_manager.top_right.x);
            float y = Random.Range(map_manager.down_left.y, map_manager.top_right.y);
            target_path.Add(new Vector2(x, y));
        }
        float last_x = Random.Range(map_manager.down_left.x, map_manager.top_right.x);
        target_path.Add(new Vector2(last_x, despawn_y - 10));
    }
}
