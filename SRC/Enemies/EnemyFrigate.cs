using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrigate : Enemy, IDamageable
{
    public GameObject shield;
    public int shield_regeneration_value = 10;
    public int shield_max_value = 50;
    float last_shield_regeneration = 10f;
    public int phase = 0;
    public int asteroids_level = 20;
    public float asteroids_time = 20f;
    public float shooting_time = 20f;
    public float anti_jittering_tolerance = 0.2f;
    bool invulnerable = true;
    bool once = false;
    EnemyGun[] guns;

    [System.Serializable]
    public class GunMode
    {
        public float turn_speed;
        public float inactive_turn_speed;
        public float shooting_arc_angle;
    }

    public List<GunMode> gun_modes;

    protected override void Start()
    {
        base.Start();
        last_shield_regeneration = 0f;

        // Phase 0, go to position without shooting
        target_path = new List<Vector2>();
        target_path.Add(new Vector2(0f, 3f));
        guns = GetComponentsInChildren<EnemyGun>();
        foreach(EnemyGun gun in guns)
        {
            gun.active = false;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Not periodical anymore, triggered in each phase
        //RegenerateShield();

        CheckPhase();
    }

    void SetGunMode(int mode)
    {
        if (mode < gun_modes.Count)
        {
            foreach (EnemyGun gun in guns)
            {
                gun.active = true;

                gun.turn_speed = gun_modes[mode].turn_speed;
                gun.inactive_turn_speed = gun_modes[mode].inactive_turn_speed;
                gun.shooting_arc_angle = gun_modes[mode].shooting_arc_angle;
            }
        }
        else
        {
            // Default
            foreach (EnemyGun gun in guns)
            {
                gun.active = true;

                gun.turn_speed = 30f;
                gun.inactive_turn_speed = 80f;
                gun.shooting_arc_angle = 12f;
            }
        }
    }

    void CheckPhase()
    {

        switch (phase)
        {
            case 0:

                // Recharge shield continuously, to reach down at maximum shield
                //RegenerateShield();

                //phase 0 descend without shoting until (0,3)
                if (CheckTargetReached(new Vector2(0f, 3f)))
                {
                    invulnerable = false;

                    SetGunMode(0);
                    phase = 100 + phase;
                    StartCoroutine(ChangePhaseDelayed(1, shooting_time));
                }
                break;

            case 1:
                //retire until (0,10)
                invulnerable = true;
                target_path = new List<Vector2>();
                target_path.Add(new Vector2(0f, 10f));
                foreach (EnemyGun gun in guns)
                {
                    gun.active = false;
                }
                if (CheckTargetReached(new Vector2(0f, 10f)))
                {
                    phase++;
                }
                break;

            case 2:
                // Start shooting asteroids
                References.map_manager.GetComponent<LevelManager>().SetAsteroids(asteroids_level);
                References.map_manager.GetComponent<LevelManager>().SetBurstAsteroids(asteroids_level);
                References.map_manager.GetComponent<LevelManager>().SetBigAsteroids(asteroids_level);

                //Start shooting and go to default while waiting for ChangePhaseDelayed
                phase = 100 + phase;


                StartCoroutine(ChangePhaseDelayed(3, asteroids_time));
                once = false;
                break;

            case 3:

                if (!once)
                {
                    once = true;
                    //Spawn my specific powerup
                    powerup_spawner.SpawnPowerup(transform.position, powerup_prefab);

                    // Recharge shield to protect while downwards
                    //RegenerateShield();
                }

                //descend until (0,3) again
                References.map_manager.GetComponent<LevelManager>().asteroid_spawner.spawn_asteroids = false;
                References.map_manager.GetComponent<LevelManager>().asteroid_spawner.spawn_burst_asteroids = false;
                References.map_manager.GetComponent<LevelManager>().asteroid_spawner.spawn_big_asteroids = false;


                target_path = new List<Vector2>();
                target_path.Add(new Vector2(0f, 3f));

                if (CheckTargetReached(new Vector2(0f, 3f)))
                {
                    phase++;
                    invulnerable = false;
                }
                break;

            case 4:
                // Alternate gun modes
                SetGunMode(0);

                phase = 100 + phase;
                StartCoroutine(ChangePhaseDelayed(5, shooting_time));

                break;
            case 5:
                // Alternate gun modes
                SetGunMode(1);

                phase = 100 + phase;
                StartCoroutine(ChangePhaseDelayed(6, shooting_time));

                break;

            case 6:
                // Alternate gun modes
                SetGunMode(2);

                phase = 100 + phase;
                StartCoroutine(ChangePhaseDelayed(7, shooting_time));
                break;

            case 7:
                // Alternate gun modes
                SetGunMode(3);

                phase = 100 + phase;
                StartCoroutine(ChangePhaseDelayed(1, 4));
                break;

            default:
                // Wait here for ChangePhaseDelayed while doing nothing

                //Remain in position but avoid jittering, in all modes except asteorids
                if(phase!=102)
                {
                    if (CheckTargetReached(new Vector2(0f, 3f)))
                    {
                        target_path = new List<Vector2>();
                    }
                    else
                    {
                        target_path = new List<Vector2>();
                        target_path.Add(new Vector2(0f, 3f));
                    }
                }
                break;
        }

    }

    IEnumerator ChangePhaseDelayed(int new_phase, float delay)
    {
        yield return new WaitForSeconds(delay);
        phase = new_phase;
    }

    bool CheckTargetReached(Vector2 target)
    {
        bool reached = false;

        try
        {
            // Current target position
            //Vector2 pos_target_now = target_path[target_index] + target_offset;

            // Change goal when final target is reached (not instant target)
            if ((target - (Vector2)transform.position).sqrMagnitude < anti_jittering_tolerance * anti_jittering_tolerance)
            {
                reached = true;
            }
        }
        catch
        {
            //return true if no target
            reached = true;
        }

        return reached;
    }

    void RegenerateShield()
    {
        Shield old_shield = null;
        foreach (Shield shield in GetComponentsInChildren<Shield>())
        {
            // Distinguish gun shield from frigate shield!
            if (shield.transform.parent == transform)
            {
                old_shield = shield;
            }
        }

        // Spawn a new shield or add energy to existing one
        if (old_shield != null)
        {
            // Capped
            if (old_shield.hp < shield_max_value)
            {
                old_shield.hp += shield_regeneration_value;
            }
        }
        else
        {
            Vector3 spawn = transform.position;// + new Vector3(0f, 0f, 0f);
            GameObject new_shield = (GameObject)Instantiate(shield, spawn, transform.rotation);
            new_shield.transform.parent = transform;
            new_shield.GetComponent<Shield>().hp = shield_regeneration_value;
        }
    }

    // Hide parent IDamageable.TakeDamage to implement invulnerability
    void IDamageable.TakeDamage(float damage, string origin = "Unkown")
    {
        if(!invulnerable)
        {
            hp -= damage;
        }
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
    protected override void Die(string cause_of_death = "Unknown")
    {
        StartCoroutine(DieDelay(3, 0.1f));
    }

    IEnumerator DieDelay(int num, float burst_delay)
    {

        for (int i = 0; i < num; i++)
        {
            //Transform from map coordinates to 0-1 for Shader
            float x = (transform.position.x - References.map_manager.GetComponent<MapManager>().down_left.x)
                    / (References.map_manager.GetComponent<MapManager>().top_right.x - References.map_manager.GetComponent<MapManager>().down_left.x);
            float y = (transform.position.y - References.map_manager.GetComponent<MapManager>().down_left.y)
                    / (References.map_manager.GetComponent<MapManager>().top_right.y - References.map_manager.GetComponent<MapManager>().down_left.y);

            //Emit gravity ripple effect
            float refraction_strength = 1f;
            float reflection_strength = 1f;
            float wave_speed = 2f;
            References.main_camera.GetComponent<RippleEffect>().Emit(new Vector2(x, y), refraction_strength, reflection_strength, wave_speed);

            yield return new WaitForSeconds(burst_delay);
        }

        //Now die
        base.Die();
    }
}
