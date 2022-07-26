using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFire : BaseAbility
{
    public float fire_delay = 0.1f; // Delay between start of each gun component
    public Gun[] guns;

    void Start()
    {
        guns = GetComponents<Gun>();
    }

    protected override void Use()
    {
        float delay = 0f;
        foreach(Gun gun in guns)
        {
            StartCoroutine(FireDelayed(gun, delay));
            delay += fire_delay;
        }
    }

    // Add x offset for each barrel
    IEnumerator FireDelayed(Gun gun, float delay)
    {
        yield return new WaitForSeconds(delay);
        gun.AutoFire();
    }


}
