using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockShooter : MonoBehaviour
{
    public Sprite[] rock_sprites; // Keep growing in size until max, then shoot
    public GameObject rock_prefab;
    GameObject rock; 
    public float growth_time = 0.5f;
    float last_growth_time = 0f;
    public float offset = 0f;
    public float speed = 2f;
    int stage_index = 0;
    PlayerShip player_ship;
    Pause pauser;
    EntityTracker entity_tracker;

    // Start is called before the first frame update
    void Start()
    {
        last_growth_time = Time.time;
        player_ship = References.player.GetComponent<PlayerShip>();
        pauser = References.pauser;
        entity_tracker = References.entity_tracker;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pauser.paused && !player_ship.ghost)
        {
            if (Time.time> last_growth_time+ growth_time)
            {
                last_growth_time = Time.time;

                // If no rock, create new
                if (rock == null)
                {
                    stage_index = 0;
                    rock = (GameObject)Instantiate(rock_prefab, transform.position + transform.up * offset + transform.forward/*keep under enemy*/, transform.rotation);

                    rock.transform.parent = transform;

                    // Add as asteroids so it can be pushed, make owner not avoid it
                    entity_tracker.asteroids.Add(rock);
                }
                else if (stage_index < rock_sprites.Length - 1)
                {
                    // Grow
                    rock.GetComponent<SpriteRenderer>().sprite = rock_sprites[++stage_index];
                }
                else
                {
                        // Shoot old and forget about it
                        rock.GetComponent<Rigidbody2D>().velocity = transform.up * speed;
                        rock.transform.parent = null;
                        rock = null;
                }
            }
        }
    }
}
