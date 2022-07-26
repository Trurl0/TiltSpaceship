using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
    
    public float value = 1f;
    public float despawn_y = -10f;
    public float speed = -1f;

    public AudioClip sound;
    AudioSource audioSource;

    public enum PowerupType
    {
        hp,
        shield,
        damage,
        fire_rate,
        speed,
        burst,
        drone,
        ability,
        main_gun,
        secondary_gun,
        ability_slot
    };
    public PowerupType type = PowerupType.hp;

    public GameObject prefab;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // FixedUpdate pause without rigidbody
    void FixedUpdate()
    {
        if (transform.position.y < despawn_y)
        {
            Destroy(gameObject);
        }
        /*else
        {
            transform.position += new Vector3(0f, speed, 0f);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            PlayerShip player_ship = References.player.gameObject.GetComponent<PlayerShip>();
            player_ship.PlayAudio(sound, 1.3F);
            References.save_manager.IncreasePowerupCount(gameObject.name);
            switch (type)
            {
                case PowerupType.hp:
                    player_ship.hp += value;
                    if(player_ship.hp > 10)
                    {
                        if (!References.save_manager.saved_data.achievement_up_to_eleven) { References.save_manager.GiveAchievement("Up to Eleven"); }
                        References.save_manager.SaveData();
                    }
                    break;

                case PowerupType.speed:
                    float increment = value / player_ship.max_speed;
                    player_ship.max_speed += value;
                    player_ship.acceleration *= (1+increment); // increase acceleration accordingly... test this
                    break;

                case PowerupType.fire_rate:
                    player_ship.IncreaseFireRate(value);
                    /*if (player_ship.main_gun != null)
                    {
                        player_ship.main_gun.fire_cooldown -= value; // Apply to main gun
                        if (player_ship.main_gun.fire_cooldown < 0.2f) { player_ship.main_gun.fire_cooldown = 0.2f; } //Hard limit TODO:config in player
                    }
                    if (player_ship.main_ray != null)
                    {
                        player_ship.main_ray.fire_cooldown -= value; // Apply to current ray
                        if (player_ship.main_ray.fire_cooldown < 0.2f) { player_ship.main_ray.fire_cooldown = 0.2f; } //Hard limit TODO:config in player
                    }
                    if (player_ship.secondary_gun != null)
                    {
                        player_ship.secondary_gun.fire_cooldown -= value; // Apply to secondary gun
                    }
                    player_ship.gun_upgrades.fire_cooldown -= value; // remember in case of switching guns
                    */
                    break;

                case PowerupType.damage:
                    if (player_ship.main_gun != null)
                    {
                        player_ship.main_gun.damage += value; // Apply to current gun
                    }
                    if (player_ship.main_ray != null)
                    {
                        player_ship.main_ray.damage += value/10; // Apply to current ray
                    }
                    if (player_ship.secondary_gun != null)
                    {
                        player_ship.secondary_gun.damage += value; // Apply to secondary gun
                    }
                    player_ship.gun_upgrades.damage -= value; // remember in case of switching guns
                    break;

                case PowerupType.burst:
                    player_ship.IncreaseBurst((int)value);
                    /*if (player_ship.main_gun != null)
                    {
                        // Apply to current gun
                        player_ship.main_gun.burst_size += (int)value;
                    }
                    if (player_ship.main_ray != null)
                    {
                        // Apply to current ray
                        player_ship.main_ray.gameObject.GetComponent<BoltMoving>().fadeoutRate /= 
                                    (float)(player_ship.gun_upgrades.burst_size + value + 1);
                    }
                    /*if (player_ship.secondary_gun != null)
                    {
                        player_ship.secondary_gun.burst_size += (int)value; // Apply to secondary gun
                    }*/
                    /*player_ship.gun_upgrades.burst_size += (int)value; // remember in case of switching guns
                    */
                    break;

                case PowerupType.main_gun:
                    player_ship.SetMainGun(prefab);
                    break;

                case PowerupType.secondary_gun:
                    player_ship.SetSecondaryGun(prefab);
                    break;

                    case PowerupType.ability:
                        player_ship.SetAbility(prefab);
                    break;

                case PowerupType.drone:
                    player_ship.SetDrones(prefab, value);
                    break;

                case PowerupType.shield:

                    Shield player_shield = null; 
                    foreach(Shield shield in References.player.GetComponentsInChildren<Shield>())
                    {
                        // Distinguish drone shield from player shield!
                        if (shield.transform.parent == References.player.transform)
                        {
                            player_shield = shield;
                        }
                    }

                    // Spawn a new shield or add energy to existing one
                    if (player_shield != null)
                    {
                        //Debug.Log("More shield");
                        player_shield.hp += value;
                        if (player_shield.hp > 10)
                        {
                            if (!References.save_manager.saved_data.achievement_shields_to_maximum_yarnell) { References.save_manager.GiveAchievement("Shields to Maximum Yarnell"); }
                            References.save_manager.SaveData();
                        }
                        Destroy(gameObject);
                    }
                    else
                    {
                        GameObject new_shield = (GameObject)Instantiate(prefab, References.player.transform.position, transform.rotation);
                        new_shield.transform.parent = References.player.transform;
                        new_shield.GetComponent<Shield>().hp = value;
                    }
                    break;

                case PowerupType.ability_slot:
                    //Set ability in a new slot
                    player_ship.GiveAbilitySlot((int)value);
                    if (prefab != null)
                    {
                        player_ship.SetAbility(prefab);
                    }
                    break;

                default:
                    break;
            }

            Destroy(gameObject);
        }


        else if (other.gameObject.tag == "PlayerDrone")
        {
            PlayerShip player_ship = References.player.gameObject.GetComponent<PlayerShip>();
            player_ship.PlayAudio(sound, 1.3F);
            //References.save_manager.IncreasePowerupCount(gameObject.name);

            // Only for drones with abilities?
            //if(other.GetComponent<AutoAbility>()!=null)

            switch (type)
            {
                case PowerupType.shield:

                    if (!References.save_manager.saved_data.achievement_motherly_instinct) { References.save_manager.GiveAchievement("Motherly Instinct"); }
                    References.save_manager.SaveData();

                    Shield my_shield = null;
                    foreach (Shield shield in other.gameObject.GetComponentsInChildren<Shield>())
                    {
                        // Distinguish drone shield from player shield!
                        if (shield.transform.parent == other.transform)
                        {
                            my_shield = shield;
                        }
                    }

                    // Spawn a new shield or add energy to existing one
                    if (my_shield != null)
                    {
                        //Debug.Log("More shield");
                        my_shield.hp += value;
                        Destroy(gameObject);
                    }
                    else
                    {
                        GameObject new_shield = (GameObject)Instantiate(prefab, other.transform.position, transform.rotation);
                        new_shield.transform.parent = other.transform;
                        new_shield.GetComponent<Shield>().hp = value;

                        // smaller for drones
                        new_shield.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
                        new_shield.GetComponent<Light>().spotAngle = 3;
                        new_shield.GetComponent<Light>().range = 10;
                    }
                    Destroy(gameObject);
                    break;

                /*
                case PowerupType.hp:
                    other.GetComponent<Drone>().hp += value;

                    // Only destory if picked up
                    Destroy(gameObject);
                    break;
                        
                case PowerupType.ability:

                    // Only one ability per drone?
                    if (other.GetComponentInChildren<BaseAbility>() == null)
                    {
                        // spawn to drone
                        GameObject new_ab = (GameObject)Instantiate(prefab, other.transform.position, transform.rotation);
                        new_ab.transform.parent = other.transform;

                        // Hack to center laser ray
                        if (new_ab.GetComponent<AbilityLaser>() != null)
                        {
                            new_ab.transform.position += new Vector3(0f, 6.1f, 0f);
                        }

                        // Only destory if picked up
                        Destroy(gameObject);
                    }

                    break;

                case PowerupType.burst:
                    other.GetComponent<Gun>().burst_size += (int)value;
                    other.GetComponent<Drone>().gun_upgrades.burst_size += (int)value; // remember in case of switching guns

                    Destroy(gameObject);
                    break;

                case PowerupType.main_gun:
                    Drone drone = other.GetComponent<Drone>();
                    Gun drone_gun = other.GetComponent<Gun>();
                    Gun new_gun = prefab.GetComponent<Gun>();

                    // Copy new gun stats, add previous upgrades to base gun
                    drone_gun.bullet_prefab = new_gun.bullet_prefab;
                    drone_gun.size = new_gun.size;
                    drone_gun.damage = new_gun.damage + drone.gun_upgrades.damage;
                    drone_gun.fire_cooldown = new_gun.fire_cooldown + drone.gun_upgrades.fire_cooldown;
                    drone_gun.bullet_offset = new_gun.bullet_offset;
                    drone_gun.bullet_speed = new_gun.bullet_speed;
                    drone_gun.burst_size = new_gun.burst_size + drone.gun_upgrades.burst_size;
                    drone_gun.burst_delay = new_gun.burst_delay;
                    drone_gun.barrels = new_gun.barrels;
                    drone_gun.barrel_separation = new_gun.barrel_separation;
                    drone_gun.barrel_angle = new_gun.barrel_angle;
                    drone_gun.barrel_ini_angle = new_gun.barrel_ini_angle;

                    Destroy(gameObject);
                    break;
                */

                default:
                    break;
            }
        }




    }
}

