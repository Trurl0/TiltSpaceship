using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker : MonoBehaviour
{

    public List<GameObject> asteroids = new List<GameObject>();
    public List<GameObject> hulls = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> player_bullets = new List<GameObject>();
    public List<GameObject> enemy_bullets = new List<GameObject>();
    public List<GameObject> powerups = new List<GameObject>();
    public List<GameObject> player_decoys = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        // Remove deleted gameobjects
        UpdateList(asteroids);
        UpdateList(hulls);
        UpdateList(enemies);
        UpdateList(player_bullets);
        UpdateList(enemy_bullets);
        UpdateList(powerups);
        UpdateList(player_decoys);
    }

    void UpdateList(List<GameObject> list)
    {
        // Iterate clone of list to avoid "modify during iteration" errors
        foreach (GameObject go in new List<GameObject>(list))
        {
            if (go == null)
            {
                list.Remove(go);
            }
        }
    }
}
