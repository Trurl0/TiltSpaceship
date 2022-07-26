using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public GUIStyle label_style;
    public List<string> phrases = new List<string>();
    public string phrase;
    public float phrase_duration;
    public float phrase_cooldown_min;
    public float phrase_cooldown_max;
    public Vector2 phrase_offset = Vector2.zero;
    protected float next_phrase_time = 0f;
    protected bool showing_phrase = false;
    protected Pause pauser;

    // Start is called before the first frame update
    void Start()
    {
        pauser = References.pauser;

        if (phrases.Count > 0)
        {
            phrase = phrases[Random.Range(0, phrases.Count)];
        }

        next_phrase_time = Time.time + Random.Range(phrase_cooldown_min, phrase_cooldown_max);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Message();
    }

    protected virtual void Message()
    {
        if (phrases.Count > 0)
        {
            if (!pauser.paused)
            {
                if ((Time.time > next_phrase_time) /*&& !dead*/) //Check dead with parent?
                {
                    // Select random and show
                    phrase = phrases[Random.Range(0, phrases.Count)];
                    showing_phrase = true;
                    StartCoroutine(HidePhrase(phrase_duration));

                    next_phrase_time = Time.time + phrase_duration + Random.Range(phrase_cooldown_min, phrase_cooldown_max);

                }
            }
        }
    }
    public IEnumerator HidePhrase(float delay)
    {
        yield return new WaitForSeconds(delay);
        showing_phrase = false;
    }

    void OnGUI()
    {
        if (showing_phrase)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)phrase_offset);

            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 400, 200), phrase, label_style);
        }
    }
}
