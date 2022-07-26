using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGhost : BaseAbility
{
    public float ability_time = 1f;

    /*protected override void Use()
    {
        // TODO: Player is in charge of this, in case this ability is lost
        player_script.EnterGhost(ghost_time);
    }*/

    protected override void Use()
    {
        References.player.GetComponent<PlayerShip>().ghost = true;
        //References.player.GetComponent<BoxCollider2D>().enabled = false;
        References.player.layer = LayerMask.NameToLayer("Ghost");

        Color c = References.player.GetComponent<SpriteRenderer>().color;
        c.a = 0.2f;
        References.player.GetComponent<SpriteRenderer>().color = c;

        foreach (Drone drone in References.player.GetComponentsInChildren<Drone>())
        {
            GameObject drone_gameobject = drone.gameObject;
            //drone_gameobject.GetComponent<CircleCollider2D>().enabled = false;
            drone_gameobject.layer = LayerMask.NameToLayer("Ghost");

            Color d = drone_gameobject.GetComponent<SpriteRenderer>().color;
            d.a = 0.2f;
            drone_gameobject.GetComponent<SpriteRenderer>().color = d;

            // All components (gun)
            foreach (SpriteRenderer sprite in drone_gameobject.GetComponentsInChildren<SpriteRenderer>())
            {
                d = sprite.color;
                d.a = 0.2f;
                sprite.color = d;
            }
        }

        // This includes drone shields!
        foreach (Shield shield in References.player.GetComponentsInChildren<Shield>())
        {
            //shield.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            shield.gameObject.layer = LayerMask.NameToLayer("Ghost");
        }


        StartCoroutine(EndAbility(ability_time));
    }

    public override void Unset()
    {
        References.player.GetComponent<PlayerShip>().ghost = false;
        //References.player.GetComponent<BoxCollider2D>().enabled = true;
        References.player.layer = LayerMask.NameToLayer("Player ship");

        Color c = References.player.GetComponent<SpriteRenderer>().color;
        c.a = 1f;
        References.player.GetComponent<SpriteRenderer>().color = c;

        foreach (Drone drone in References.player.GetComponentsInChildren<Drone>())
        {
            GameObject drone_gameobject = drone.gameObject;
            //drone_gameobject.GetComponent<CircleCollider2D>().enabled = true;
            drone_gameobject.layer = LayerMask.NameToLayer("Player ship");

            Color d = drone_gameobject.GetComponent<SpriteRenderer>().color;
            d.a = 1f;
            drone_gameobject.GetComponent<SpriteRenderer>().color = d;
            // All components (gun)
            foreach (SpriteRenderer sprite in drone_gameobject.GetComponentsInChildren<SpriteRenderer>())
            {
                d = sprite.color;
                d.a = 1f;
                sprite.color = d;
            }
        }

        // This includes drone shields!
        foreach (Shield shield in References.player.GetComponentsInChildren<Shield>())
        {
            //shield.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            shield.gameObject.layer = LayerMask.NameToLayer("Player bullet");
        }

    }

    public IEnumerator EndAbility(float delay)
    {
        yield return new WaitForSeconds(delay);

        Unset();
    }

}
