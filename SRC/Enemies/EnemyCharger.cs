using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharger : Enemy, IDamageable {

    float last_max_distance_to_player = 100f;
    float last_min_distance_to_player = 2f;
    public float charge_cooldown = 10f;
    public float charge_duration = 1f;
    public float warning_duration = 0.5f;
    bool charging = false;
    protected float last_fire_time = 0f;

    // Dodge only on x axis
    protected override void Start()
    {
        base.Start();
        last_fire_time = Time.time;
    }

    protected override Vector2 DodgeBullets()
    {
        Vector2 bullets_dodge_force = Vector2.zero;
        foreach (GameObject bullet in entity_tracker.player_bullets)
        {
            // if above
            Vector2 vector_to_bullet = (bullet.transform.position - transform.position);
            if (vector_to_bullet.sqrMagnitude < (bullet_repulsion_radius * bullet_repulsion_radius))
            {
                //Gravity repulsion only on x axis
                bullets_dodge_force += new Vector2(-vector_to_bullet.x, 0f).normalized * dodge_force;
            }
        }
        //Debug.DrawLine(transform.position, (Vector2)transform.position + bullets_dodge_force, Color.red, 0.1f);
        return bullets_dodge_force;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        // Call FixedUpdate of parent
        base.FixedUpdate();

        CheckCharge();
    }

    void CheckCharge()
    {
        if (!pauser.paused)
        {
            if ((Time.time > last_fire_time + charge_cooldown) && !dead)
            {
                StartCoroutine(Charge(warning_duration));
            }
        }
    }
    public IEnumerator Charge(float warning_duration)
    {
        if (!charging)
        {
            last_fire_time = Time.time;
            charging = true;

            // Inform player
            BlobAnimation animator = GetComponent<BlobAnimation>();
            animator.look_at_player = true;
            yield return new WaitForSeconds(warning_duration);

            // Charge
            last_max_distance_to_player = max_distance_to_player;
            last_min_distance_to_player = min_distance_to_player;
            max_distance_to_player = 0;
            min_distance_to_player = 0;
            yield return new WaitForSeconds(charge_duration);

            // Stop charge
            max_distance_to_player = last_max_distance_to_player;
            min_distance_to_player = last_min_distance_to_player;
            //animator.look_at_player = false;

            charging = false;
        }
    }

    void IDamageable.TakeDamage(float damage, string origin = "Unkown")
    {
        //Attack immediatly if damaged
        StartCoroutine(Charge(0f));

        hp -= damage;
        if (hp <= 0)
        {
            Die(origin);
        }
        if (damage > 0)
        {
            if (Time.time > last_hit_sound + hit_sound_cooldown)
            {
                last_hit_sound = Time.time;
                audioSource.PlayOneShot(hit_sound, 1F);
            }
            StartCoroutine(DamageExplosion(explosion_prefab, 2, 0.1f, 0.3f));
        }
    }

}
