using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGive : BaseAbility
{

    public int give_life = 0;
    public int give_shield = 0;
    public GameObject shield_prefab;
    PlayerShip player_ship;

    private void Start()
    {
        player_ship = References.player.GetComponent<PlayerShip>();
    }

    protected override void Use()
    {
        if (give_life > 0)
        {
            player_ship.hp += give_life;
            if (player_ship.hp > 10)
            {
                if (!References.save_manager.saved_data.achievement_up_to_eleven) { References.save_manager.GiveAchievement("Up to Eleven"); }
                References.save_manager.SaveData();
            }
        }

        if (give_shield>0)
        {
            Shield player_shield = null;
            foreach (Shield shield in References.player.GetComponentsInChildren<Shield>())
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
                player_shield.hp += give_shield;
                if (player_shield.hp > 10)
                {
                    if (!References.save_manager.saved_data.achievement_shields_to_maximum_yarnell) { References.save_manager.GiveAchievement("Shields to Maximum Yarnell"); }
                    References.save_manager.SaveData();
                }
                Destroy(gameObject);
            }
            else
            {
                GameObject new_shield = (GameObject)Instantiate(shield_prefab, References.player.transform.position, transform.rotation);
                new_shield.transform.parent = References.player.transform;
                new_shield.GetComponent<Shield>().hp = give_shield;
            }
        }
    }
}
