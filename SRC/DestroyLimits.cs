using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLimits : MonoBehaviour
{
    public float max_y = 100f;
    public float min_y = -100f;
    public float max_x = 100f;
    public float min_x = -100f;

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < min_x ||
            transform.position.x > max_x ||
            transform.position.y < min_y || 
            transform.position.y > max_y)
        {
            Destroy(gameObject);
        }

    }
}
