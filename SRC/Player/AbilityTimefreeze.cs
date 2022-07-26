using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTimefreeze : BaseAbility
{
    public float ability_time = 1f; //Careful, this is BEFORE timefreeze
    public float freeze_time_scale = 0.3f;
    public float acceleration_boost = 1.5f;
    public float max_speed_boost = 1.5f;
    public float fire_cooldown_reduction = 0.3f;
    public float gun_prefab_fire_cooldown = 1f;
    public float big_gun_prefab_fire_cooldown = 2f;
    public float ray_prefab_fire_cooldown = 1.5f;
    float old_time_scale = 1f;
    float old_player_acceleration = 10f;
    float old_player_max_speed = 3f;
    //float old_player_fire_cooldown = 1f;

    protected override void Use()
    {
        old_time_scale = Time.timeScale;
        old_player_acceleration = References.player.GetComponent<PlayerShip>().acceleration;
        old_player_max_speed = References.player.GetComponent<PlayerShip>().max_speed;

        Time.timeScale = freeze_time_scale;
        References.player.GetComponent<PlayerShip>().acceleration *= acceleration_boost;
        References.player.GetComponent<PlayerShip>().max_speed *= max_speed_boost;

        if (References.player.GetComponent<PlayerShip>().main_gun != null)
        {
            //old_player_fire_cooldown = References.player.GetComponent<PlayerShip>().main_gun.fire_cooldown;
            References.player.GetComponent<PlayerShip>().main_gun.fire_cooldown *= fire_cooldown_reduction;
        }
        else if (References.player.GetComponent<PlayerShip>().main_ray != null)
        {
            //old_player_fire_cooldown = References.player.GetComponent<PlayerShip>().main_ray.fire_cooldown;
            References.player.GetComponent<PlayerShip>().main_ray.fire_cooldown *= fire_cooldown_reduction;
        }

        StartCoroutine(EndAbility(ability_time));
    }

    public override void Unset()
    {
        Time.timeScale = old_time_scale;
        References.player.GetComponent<PlayerShip>().acceleration = old_player_acceleration;
        References.player.GetComponent<PlayerShip>().max_speed = old_player_max_speed;

        // HACK in case player switches guns mid-timefreeze: Apply rate of fire modfiers to orignal prefab cooldowns
        if (References.player.GetComponent<PlayerShip>().main_gun != null)
        {
            if(References.player.GetComponent<PlayerShip>().main_gun.gameObject.name.ToLower().Contains("big"))
            {
                References.player.GetComponent<PlayerShip>().main_gun.fire_cooldown = big_gun_prefab_fire_cooldown + References.player.GetComponent<PlayerShip>().gun_upgrades.fire_cooldown;
            }
            else
            {
                References.player.GetComponent<PlayerShip>().main_gun.fire_cooldown = gun_prefab_fire_cooldown + References.player.GetComponent<PlayerShip>().gun_upgrades.fire_cooldown;
            }
        }
        else if (References.player.GetComponent<PlayerShip>().main_ray != null)
        {
            References.player.GetComponent<PlayerShip>().main_ray.fire_cooldown = ray_prefab_fire_cooldown + References.player.GetComponent<PlayerShip>().gun_upgrades.fire_cooldown;
        }
    }

    public IEnumerator EndAbility(float delay)
    {
        yield return new WaitForSeconds(delay);

        Unset();
    }
}
