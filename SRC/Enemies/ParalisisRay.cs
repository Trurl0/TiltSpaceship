using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalisisRay : MonoBehaviour
{
    public float paralisis_distance = 1.5f;
    public float paralisis_time = 1f;
    public float shield_damage = 1f;
    public float fire_cooldown = 10f;
    protected float last_fire_time = 0f;
    BoltSingle bolt;
    protected GameObject player;
    protected PlayerShip player_ship;
    protected Pause pauser;

    void Start()
    {
        bolt = GetComponentInChildren<BoltSingle>();
        player = References.player;
        player_ship = player.GetComponent<PlayerShip>();
        pauser = References.pauser;

        last_fire_time = Time.time;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Fire();
    }

    void Fire()
    {
        if (!pauser.paused && !player_ship.ghost)
        {
            if ((Time.time > last_fire_time + fire_cooldown))
            {
                // Here if we also want some wait inside distance before shooting
                last_fire_time = Time.time;

                // Only if in range
                if ((player.transform.position - transform.position).sqrMagnitude < paralisis_distance * paralisis_distance)
                {
                    //last_fire_time = Time.time;

                    //Draw ray
                    bolt.Fire((Vector2)transform.position, (Vector2)player.transform.position);

                    Shield shield = player.GetComponentInChildren<Shield>();
                    if (shield != null && shield.transform.parent == player.transform)
                    {
                        ((IDamageable)shield).TakeDamage(shield_damage, "Enemy ray");
                    }
                    else
                    {
                        player_ship.GetParalized(paralisis_time);
                    }
                }
            }
        }
    }
}
