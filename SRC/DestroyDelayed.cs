using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelayed : MonoBehaviour
{
    public float death_delay = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyDelayedCoroutine(death_delay));
    }

    IEnumerator DestroyDelayedCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        {
            Destroy(gameObject);
        }
    }
}
