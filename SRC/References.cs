using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour {

    // Set in inspector
    public MapManager       map_manager_ref;
    public LevelManager     level_manager_ref;
    public EntityTracker    entity_tracker_ref;
    public Pause            pauser_ref;
    public GameObject       player_ref;
    public GameObject       main_camera_ref;
    public PowerupSpawner   powerup_spawner_ref;
    public SaveManager      save_manager_ref;

    // Global instance
    static References instance;
    void Start()
    {
        instance = this;

        // SaveManager is kept between scenes due to android file access issues.
        // SaveManager is also singleton-like, so it is important it goes first in execution order, so the original is used!
        save_manager_ref = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
    }

    // External access
    public static MapManager        map_manager              { get { return instance.map_manager_ref; } }
    public static LevelManager      level_manager            { get { return instance.level_manager_ref; } }
    public static GameObject        main_camera              { get { return instance.main_camera_ref; } }
    public static EntityTracker     entity_tracker           { get { return instance.entity_tracker_ref; } }
    public static Pause             pauser                   { get { return instance.pauser_ref; } }
    public static GameObject        player                   { get { return instance.player_ref; } }
    public static PowerupSpawner    powerup_spawner          { get { return instance.powerup_spawner_ref; } }
    public static SaveManager       save_manager { get { return instance.save_manager_ref; } }

}
