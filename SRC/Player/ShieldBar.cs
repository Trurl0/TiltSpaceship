using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : MonoBehaviour {

    public GameObject bar_prefab;
    public float x_offset;
    public float y_offset;
    public float separation;

    private GameObject player;
    private MapManager map_manager;
    private int bars;

    // Use this for initialization
    void Start ()
    {
        map_manager = References.map_manager;
        player = References.player;

        // Start as 0 to force first draw
        bars = 0;
    }
	
	// Update is called once per frame
	void Update () {

        // Distinguish drone shield from player shield
        Shield player_shield = null;
        foreach (Shield shield in References.player.GetComponentsInChildren<Shield>())
        {
            if (shield.transform.parent == References.player.transform)
            {
                player_shield = shield;
            }
        }

        if (player_shield != null)
        {
            if ((int)player_shield.hp != bars)
            {
                bars = (int)player_shield.hp;
                //float rest = player.hp - bars;  // percentage for incomplete bars

                // Clean
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < bars; i++)
                {
                    // z coordinate corrects camera height from down_left coordinates 
                    GameObject new_bar = Instantiate(bar_prefab, map_manager.down_left + new Vector3(x_offset + (i * separation), y_offset, 40f), Quaternion.identity);
                    new_bar.transform.parent = transform;
                }

            }
        }
        else
        {
            // Clean
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
