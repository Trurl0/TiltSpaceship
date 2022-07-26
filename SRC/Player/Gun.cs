using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IFire
{

    public Rigidbody2D bullet_prefab;
    public float damage = 1f;
    public float size = 1f;
    public float fire_cooldown = 1f;
    public float min_fire_cooldown = 0.2f;
    private float last_fire_time = 0f;
    public float bullet_offset = 1f;
    public float bullet_speed = 10;
    public int burst_size = 3;
    public float burst_delay = 0.03f;
    public int barrels = 1;
    public float barrel_separation = 0.2f;
    public float barrel_angle = 0.5f;  // bullet spread in radians
    public float barrel_ini_angle = 0f;  // bullet spread in radians
    public bool manual_fire = true;
    //public bool manual_aim = false;
    public bool auto_fire = false;
    public bool auto_aim = false;
    public bool manual_aim = false;
    public float auto_aim_rate = 1f;
    public float auto_aim_velocity_correction = 1f;
    public float max_targeting_distance = 10f;
    private float target_distance = 0f;
    private float next_auto_aim = 0f;
    private GameObject aim_target;
    private Rigidbody2D player_rigidbody;
    private PlayerShip player_script;
    private EntityTracker entity_tracker;
    public List<Rigidbody2D> inactive_bullets;
    public List<Rigidbody2D> active_bullets;
    public int bullet_pool_size = 10;

    public AudioClip sound;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        GameObject player = References.player;
        player_rigidbody = player.GetComponent<Rigidbody2D>();
        player_script = player.GetComponent<PlayerShip>();

        entity_tracker = References.entity_tracker;

        //Start counters at 0, abilities should be ready on pick
        //next_auto_aim = Time.time + auto_aim_rate;
        last_fire_time = Time.time - fire_cooldown;

        List<Rigidbody2D> inactive_bullets = new List<Rigidbody2D>();
        List<Rigidbody2D> active_bullets = new List<Rigidbody2D>();
        CreateBulletPool();
    }

    public void Update()
    {
        if(auto_aim)
        {
            if (Time.time > next_auto_aim)
            {
                next_auto_aim = Time.time + auto_aim_rate;

                aim_target = AcquireTarget();
            }
        }

        if (auto_fire)
        {
             AutoFire();
        }

        GetBackInactiveBullets();
    }

    GameObject AcquireTarget()
    {
        // Get closest target, closer than max_targeting_distance and over the lower Y of the map
        GameObject new_target = null;
        float closest_sqrdistance = max_targeting_distance * max_targeting_distance;


        //enemies with priority
        foreach (GameObject enemy in entity_tracker.enemies)
        {
            float sqrdistance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (sqrdistance < closest_sqrdistance && enemy.transform.position.y > References.map_manager.down_left.y)
            {
                closest_sqrdistance = sqrdistance;
                new_target = enemy;
            }
        }
    
        if (new_target == null)
        {
            //bullets
            foreach (GameObject enemy in entity_tracker.enemy_bullets)
            {
                float sqrdistance = (enemy.transform.position - transform.position).sqrMagnitude;
                if (sqrdistance < closest_sqrdistance && enemy.transform.position.y > References.map_manager.down_left.y)
                {
                    closest_sqrdistance = sqrdistance;
                    new_target = enemy;
                }
            }
        }

        // Don't shoot asteroids if there are enemies
        if (new_target == null)
        {
            foreach (GameObject asteroid in entity_tracker.asteroids)
            {
                float sqrdistance = (asteroid.transform.position - transform.position).sqrMagnitude;
                if (sqrdistance < closest_sqrdistance && asteroid.transform.position.y > References.map_manager.down_left.y)
                {
                    closest_sqrdistance = sqrdistance;
                    new_target = asteroid;
                }
            }
        }

        // Get magnitude unsquared only once (global parameter)
        if (new_target != null)
        {
            target_distance = (new_target.transform.position - transform.position).magnitude;
        }

        return new_target;
    }

    // For player input
    void IFire.Fire(Vector2 fire_vector)
    {
        if(manual_fire)
        {
            // Shoot straight if no aim allowed
            if (!manual_aim)
            {
                fire_vector = new Vector2(0f, 1f);
            }
            Fire(fire_vector);
        }
    }

    // For internal calls or scripts
    public void AutoFire()
    {

        Vector2 fire_vector = new Vector2(0f, 1f);
        if (aim_target!=null)
        {
            // aim to trayectory, also factor target distance
            Vector2 velocity_correction = aim_target.GetComponent<Rigidbody2D>().velocity * target_distance * auto_aim_velocity_correction;
            fire_vector = ((Vector2)aim_target.transform.position -  (Vector2)transform.position + velocity_correction).normalized;

            //fire_vector = (Vector2)(aim_target.transform.position - transform.position).normalized;
        }

        Fire(fire_vector);
    }

    private void Fire(Vector2 fire_vector)
    {
        if (Time.time > last_fire_time + fire_cooldown && !player_script.dead)
        {
            last_fire_time = Time.time;

            StartCoroutine(FireBurst(bullet_prefab, fire_vector));

            References.save_manager.IncreaseBulletsFired(burst_size * barrels);
        }
    }

    //public IEnumerator FireBurst(Rigidbody2D bullet_prefab, Vector3 pos, Vector2 velocity, int burst_size, float burst_delay)
    public IEnumerator FireBurst(Rigidbody2D bullet_prefab, Vector2 fire_vector)
    {

        for (int i = 0; i < burst_size; i++)
        {

            // Avoid overloading...
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(sound, 1F);
            }

            // Calculate fire vector each burst as origin can move!

            //  |    | |   | | |
            //  S     S      S
            float ini_pos = -(barrel_separation / 2) * (barrels - 1);
            float ini_angle = -(barrel_angle / 2) * (barrels - 1);
            /*Debug.Log("ini_pos" + ini_pos);
            Debug.Log("ini_angle" + ini_angle);*/
            for (int b = 0; b < barrels; b++)
            {
                float x = ini_pos + b * barrel_separation;
                float angle = barrel_ini_angle + (ini_angle + b * barrel_angle);
                float rad = angle * Mathf.Deg2Rad;

                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                Vector2 new_fire_vector = new Vector2(
                                            (fire_vector.x * cos) - (fire_vector.y * sin),
                                            (fire_vector.x * sin) + (fire_vector.y * cos)
                                          ).normalized;
                /*Debug.Log((new_fire_vector.x * cos) + " - " + (new_fire_vector.y * sin));
                Debug.Log((new_fire_vector.x * sin) + " + " + (new_fire_vector.y * cos));
                Debug.Log("new_fire_vector: " + new_fire_vector);
                Debug.Log("barrel_" + b + ", angle: " + angle + ", rad: " + rad + ", x: " + x+ ", new_fire_vector: " + new_fire_vector);
                */

                Vector3 pos = transform.position + new Vector3(x, 0f, 0f) + (Vector3)new_fire_vector * bullet_offset;
                Vector2 velocity = player_rigidbody.velocity + new_fire_vector * bullet_speed;

                Rigidbody2D bullet;
                if (inactive_bullets.Count > 0)
                {
                    bullet = inactive_bullets[0];
                    bullet.transform.position = pos;

                    inactive_bullets.Remove(bullet);
                    active_bullets.Add(bullet);
                }
                else
                {
                    bullet = (Rigidbody2D)Instantiate(bullet_prefab, pos, transform.rotation);
                    active_bullets.Add(bullet);// Increase pool permanently
                }
                bullet.gameObject.SetActive(true);
                bullet.velocity = velocity;
                bullet.gameObject.GetComponent<Bullet>().damage = damage;
                //bullet.transform.localScale = bullet.transform.localScale * size; //This fucks up the pool

                //Timed death/deactivation started here instead of Bullet.Start()
                bullet.gameObject.GetComponent<Bullet>().Activate();

                entity_tracker.player_bullets.Add(bullet.gameObject);

            }

            yield return new WaitForSeconds(burst_delay);
        }
    }

    void CreateBulletPool()
    {
        for (int i=0;i< bullet_pool_size; i++)
        {
            Rigidbody2D bullet = (Rigidbody2D)Instantiate(bullet_prefab, new Vector3(-10f, -10f, 0f), transform.rotation);
            bullet.gameObject.SetActive(false);
            inactive_bullets.Add(bullet);
        }
    }

    void GetBackInactiveBullets()
    {
        List<Rigidbody2D> recovered_bullets = new List<Rigidbody2D>(); //temp list to avoid modifying while iterating
        foreach(Rigidbody2D bullet in active_bullets)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                inactive_bullets.Add(bullet);
                recovered_bullets.Add(bullet);
                try { entity_tracker.player_bullets.Remove(bullet.gameObject);  } catch { }
            }
        }
        //Remove from active
        foreach (Rigidbody2D bullet in recovered_bullets)
        {
            active_bullets.Remove(bullet);
        }

    }
    void OnDestroy()
    {
        //Cleanup my pools (but don't remove active ones... garbage but not much)
        foreach (Rigidbody2D bullet in inactive_bullets)
        {
            bullet.gameObject.SetActive(true);
            Destroy(bullet.gameObject);
        }
    }
}
