using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAbility : MonoBehaviour
{
    public float activation_time = 20f;
    public float last_activation = 0f;
    public float last_successful_activation = 0f;
    public bool activate_by_time = false;
    public bool activate_by_contact = false;
    public bool activate_at_start = false;
    public bool activated_at_start = false;
    public bool use_warning_light = false; // for drones
    public float warning_time = 1f;
    Light warning_light;
    FadeLight fade_light;

    private void Start()
    {
        last_activation = Time.time;
        if (use_warning_light)
        {
            warning_light = GetComponentInChildren<Light>();
            fade_light = warning_light.GetComponent<FadeLight>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Here instead of start to avoid problems with Start times of different scripts...
        if (activate_at_start && !activated_at_start)
        {
            foreach (IAbility ability in GetComponentsInChildren<IAbility>())
            {
                bool used = ability.Use();
                if (used)
                {
                    activated_at_start = true;
                }
                //Careful: activation_time should be strictly greater than ability use cooldown
                //Debug.Log("Try to use AutoAbility: " + ability);
                //Debug.Log("actually used: " + used);
            }
        }

        if (activate_by_time)
        {
            // Light always enabled for loaded drones
            if (use_warning_light && warning_light!=null)
            {
                warning_light.enabled = false;
                foreach (IAbility ability in GetComponentsInChildren<IAbility>())
                {
                    warning_light.enabled = true;
                }
            }

            if (Time.time > last_activation + activation_time)
            {

                last_activation = Time.time;
                foreach (IAbility ability in GetComponentsInChildren<IAbility>())
                {
                    bool used = ability.Use();
                    //Careful: activation_time should be strictly greater than ability use cooldown
                    //Debug.Log("Try to use AutoAbility: " + ability);
                    //Debug.Log("actually used: " + used);
                    if (used)
                    {
                        last_successful_activation = Time.time;
                    }
                }

                if (use_warning_light)
                {
                    //warning_light.enabled = false;
                }
            }

            if (use_warning_light)
            {
                foreach (BaseAbility ability in GetComponentsInChildren<BaseAbility>())
                {
                    if (Time.time > last_successful_activation + ability.use_cooldown - warning_time)
                    {
                        //warning_light.enabled = true;
                        fade_light.enabled = true;
                        fade_light.speed = 5f;
                    }
                    else
                    {
                        warning_light.intensity = 20f;
                        fade_light.enabled = false;
                    }
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (activate_by_contact)
        {
           foreach (IAbility ability in GetComponentsInChildren<IAbility>())
           {
               bool used = ability.Use();
               //Careful: activation_time should be strictly greater than ability use cooldown
               //Debug.Log("Try to use AutoAbility: " + ability);
               //Debug.Log("actually used: " + used);
           }
        }
    }
}
