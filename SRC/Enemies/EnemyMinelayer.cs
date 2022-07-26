using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinelayer : Enemy, IDamageable {

    public List<GameObject> my_spawns = new List<GameObject>();
    public int max_spawn = 100;

    [Header("Fire")]
    public Rigidbody2D bullet_prefab;
    public float fire_cooldown = 10f;
    protected float last_fire_time = 0f;
    public float bullet_offset = 1f;
    public float bullet_speed = 10;
    public int burst_size = 3;
    public float burst_delay = 0.03f;
    public float player_velocity_correction = 0.2f;

    // Update is called once per frame
    protected override void Start()
    {
        // Call Start of parent
        base.Start();
        last_fire_time = Time.time;
    }

    protected override Vector2 MoveToTarget()
    {
        Vector2 target_acceleration = Vector2.zero;

        Vector3 target_pos = Vector3.zero;
        if (References.entity_tracker.player_decoys.Count > 0)
        {
            target_pos = References.entity_tracker.player_decoys[0].transform.position;
        }
        else if (!player_ship.ghost)
        {
            target_pos = player.transform.position;
        }

        if (target_pos != Vector3.zero)
        {
            // Target is fixed height  above player
            // pos_target_now = (Vector2)player.transform.position + target_offset;
            pos_target_now = new Vector2(target_pos.x, target_offset.y);
        }

        // go towads target
        target_acceleration = (pos_target_now - (Vector2)transform.position).normalized * move_force;

        //Debug.DrawLine(pos_target_now - new Vector2(0.1f, 0f), pos_target_now + new Vector2(0.1f, 0f), Color.green, 0.1f);
        //Debug.DrawLine(pos_target_now - new Vector2(0f, 0.1f), pos_target_now + new Vector2(0f, 0.1f), Color.green, 0.1f);
        //Debug.DrawLine(transform.position, (Vector2)transform.position + vector_to_target.normalized * speed, Color.blue, 0.1f);

        return target_acceleration;
    }

    protected override Vector2 DodgeFriends()
    {
        Vector2 friend_dodge_force = Vector2.zero;
        foreach (GameObject friend in entity_tracker.enemies)
        {
            // Don't run from my own mines!
            if (friend != null && LayerMask.LayerToName(friend.layer)!= "Enemy mine" && friend != this.gameObject) //Can be destroyed at any time...
            {
                Vector2 vector_to_friend = (friend.transform.position - transform.position);
                if (vector_to_friend.sqrMagnitude > (max_distance_to_friend * max_distance_to_friend))
                {
                    friend_dodge_force = vector_to_friend.normalized * dodge_force;
                }
                else if (vector_to_friend.sqrMagnitude < (min_distance_to_friend * min_distance_to_friend))
                {
                    friend_dodge_force = -vector_to_friend.normalized * dodge_force;
                }
            }
        }
        Debug.DrawLine(transform.position, (Vector2)transform.position + friend_dodge_force, Color.magenta, 0.1f);
        return friend_dodge_force;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        // Call FixedUpdate of parent
        base.FixedUpdate();

        Spawn();
    }

    // To drop mines directly below
    void Spawn()
    {
        // Forget deleted gameobjects
        // Iterate clone of list to avoid "modify during iteration" errors
        foreach (GameObject go in new List<GameObject>(my_spawns))
        {
            if (go == null)
            {
                my_spawns.Remove(go);
            }
        }

        if (!pauser.paused)
        {

            // Change here to allow corrections mid-burst
            fire_vector = new Vector2(0f, bullet_offset);
            fire_ini = (Vector2)transform.position + fire_vector;
            //Debug.Log("bullet_offset: " + bullet_offset+ ", fire_vector: " + fire_vector+", fire_ini: " + fire_ini);

            if ((Time.time > last_fire_time + fire_cooldown) && !dead && (my_spawns.Count < max_spawn))
            {
                last_fire_time = Time.time;
                StartCoroutine(SpawnBurst(bullet_prefab, burst_size, burst_delay));
            }
        }

    }

    IEnumerator SpawnBurst(Rigidbody2D bullet_prefab, int burst_size, float burst_delay)
    {
        for (int i = 0; i < burst_size; i++)
        {
            Rigidbody2D bullet = (Rigidbody2D)Instantiate(bullet_prefab, fire_ini, transform.rotation);
            //bullet.velocity = fire_vector.normalized * bullet_speed;

            my_spawns.Add(bullet.gameObject);
            entity_tracker.enemies.Add(bullet.gameObject); // Add to enemies instead of bullets!

            yield return new WaitForSeconds(burst_delay);
        }
    }

}
