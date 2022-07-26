using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{

    public float turn_speed;
    public float turn_change_time_min;
    public float turn_change_time_max;
    public float min_x;
    public float max_x;
    public bool full_circle = false; // if insead of random tilt, look at full circle
    float next_change;
    Vector2 target = Vector2.zero;


    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time > next_change)
        {
            next_change = Time.time +Random.Range(turn_change_time_min, turn_change_time_max);

            if (full_circle)
            {
                target = Random.insideUnitCircle;
            }
            else
            {
                // Small variations of general orientation
                if(transform.localScale.x < 0)
                {
                    // If the GameObject is flipped, flip target boundaries
                    target = new Vector2(Random.Range(-max_x, -min_x), 1f);
                }
                else
                {
                    target = new Vector2(Random.Range(min_x, max_x), 1f);
                }
            }
        }

        transform.up = Vector3.Lerp(transform.up, target, turn_speed);

    }
}
