using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMumwurm : Enemy, IDamageable {

    [Header("Fire")]
    public Rigidbody2D bullet_prefab;
    public float fire_cooldown = 10f;
    protected float last_fire_time = 0f;
    public float bullet_offset = 1f;
    public float bullet_speed = 10;
    public int burst_size = 3;
    public float burst_delay = 0.03f;
    public float player_velocity_correction = 0.2f;

    public List<GameObject> my_spawns = new List<GameObject>();
    public int max_spawn = 100;

    // Update is called once per frame
    protected override void Start()
    {
        // Call Start of parent
        base.Start();
        last_fire_time = Time.time;
    }

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
            fire_vector = Random.insideUnitCircle * bullet_offset;
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
            // Here to spawn EACH bat around randomly, else they are an easy target
            fire_ini = (Vector2)transform.position + fire_vector;
            //Debug.Log("bullet_offset: " + bullet_offset + ", fire_vector: " + fire_vector + ", fire_ini: " + fire_ini);
            Rigidbody2D bullet = (Rigidbody2D)Instantiate(bullet_prefab, fire_ini, transform.rotation);
            bullet.velocity = fire_vector.normalized * bullet_speed;

            my_spawns.Add(bullet.gameObject);
            entity_tracker.enemies.Add(bullet.gameObject); // Add to enemies instead of bullets!

            yield return new WaitForSeconds(burst_delay);
        }
    }

    protected override void Die(string cause_of_death = "Unknown")
    {
        if (!dead)
        {
            // Tell level manager to stop spawning my minions
            References.level_manager.DeactivateAll();
        }

        base.Die(cause_of_death);

    }

}
