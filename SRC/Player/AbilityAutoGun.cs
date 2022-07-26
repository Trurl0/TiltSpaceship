using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAutoGun : BaseAbility
{
    public float ability_time = 1f;
    Gun gun;
    int burst_size;

    void Start()
    {
        gun = GetComponent<Gun>();

        // memorize original burst
        burst_size = gun.burst_size;

        // Start deactivated
        gun.burst_size = 0;
    }

    protected override void Use()
    {
        gun.burst_size = burst_size;
        StartCoroutine(EndAbility(ability_time));
    }

    public IEnumerator EndAbility(float delay)
    {
        yield return new WaitForSeconds(delay);

        Unset();
    }

    public override void Unset()
    {
        gun.burst_size = 0;
    }
}
