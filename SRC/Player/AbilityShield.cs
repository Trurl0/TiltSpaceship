using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShield : BaseAbility
{
    public float ability_time = 1f;
    private PlayerShip player_ship;

    private void Start()
    {
        player_ship = References.player.GetComponent<PlayerShip>();
    }

    protected override void Use()
    {

        // Disable shields, including drone shields!
        // But not invulnerable shields (more than one shield ability...)
        foreach (Shield shield in References.player.GetComponentsInChildren<Shield>())
        {
            if (!shield.invulnerable)
            {
                shield.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }

        // Enable red shield
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        // Make player invulnerable in case shield fails
        player_ship.SetInvulnerable(ability_time);

        // Make drones without shields invulnerable
        foreach (Drone drone in References.player.GetComponentsInChildren<Drone>())
        {
            drone.invulnerable = true;
        }

        StartCoroutine(EndAbility(ability_time));
    }

    public override void Unset()
    {
        // Enable shields, including drone shields!
        // But not invulnerable shields (more than one shield ability...)
        foreach (Shield shield in References.player.GetComponentsInChildren<Shield>(true))
        {
            if (!shield.invulnerable)
            {
                shield.gameObject.GetComponent<Collider2D>().enabled = true;
            }
        }

        // Disable red shield
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        // Make drones vulnerable
        foreach (Drone drone in References.player.GetComponentsInChildren<Drone>())
        {
            drone.invulnerable = false;
        }
    }

    public IEnumerator EndAbility(float delay)
    {
        yield return new WaitForSeconds(delay);

        Unset();
    }

}
