using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour, IAbility
{
    public float use_cooldown = 1f;
    public Sprite sprite;
    float last_use_time = 0f;
    public bool ready = false;

    // For public calls from player
    bool IAbility.Use()
    {
        // Check if has been actually used
        bool used = false;

        if (ready)
        {
            used = true;

            last_use_time = Time.time;

            // To call child implementation, different than IAbility.Use (TODO: find a less confusing name)
            Use();

            ready = false;
        }
        return used;
    }

    // Only internal calls from IAbility.Use()
    // Each particular ability is defined overriding this
    protected virtual void Use()
    {
        // Implement in derived classes
        Debug.Log("BASE ABILITY USED!");
    }

    public virtual void Unset()
    {
        // Implement in derived classes
        // Debug.Log("BASE ABILITY RESET!");

    }

    protected virtual void Update()
    {
        // Update button readyness
        if (!ready && Time.time > last_use_time + use_cooldown)
        {
            //Set ready to use
            ready = true;
        }
    }
    void OnDestroy()
    {
        Unset();
    }

}
