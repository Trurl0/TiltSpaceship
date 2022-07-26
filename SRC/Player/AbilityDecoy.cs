using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDecoy : BaseAbility
{
    public GameObject decoy_prefab;
    public float decoy_offset = 2.5f;

    protected override void Use()
    {
        // Spawn towards the other side of the screen
        float pos_x = decoy_offset;
        if (transform.position.x > 0f)
        {
            pos_x = -decoy_offset;
        }

        Vector3 pos = transform.position + new Vector3(pos_x, decoy_offset, 0f);

        GameObject decoy = Instantiate(decoy_prefab, pos, transform.rotation);

        References.entity_tracker.player_decoys.Add(decoy);
    }
}
