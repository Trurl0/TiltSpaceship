using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobAnimation : MonoBehaviour {

    [System.Serializable]
    public class SpriteSet
    {
        public List<Sprite> sprite_set;
    }

    public List<SpriteSet> sprite_sets;
    public List<Sprite> sprites;
    public float animation_speed = 0.2f;
    public float speed_threshold = 1f;
    public float turn_angle = 10f;
    public bool look_at_player = false;
    public bool look_with_velocity = false;
    public float turn_speed = 0.1f;

    int animation_index = 0;
    float last_frame_time = 0f;

    private SpriteRenderer m_sprite;
    private Rigidbody2D m_rigidbody;
    private GameObject player;

    // Use this for initialization
    void Start ()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_sprite = GetComponent<SpriteRenderer>();
        last_frame_time = Time.time;
        player = References.player;

        // Select sprite_set from available options
        if(sprite_sets.Count>0)
        {
            sprites = sprite_sets[Random.Range(0, sprite_sets.Count)].sprite_set;
            m_sprite.sprite = sprites[0];
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Time.time > last_frame_time + animation_speed)
        {
            last_frame_time = Time.time;

            animation_index++;
            if (animation_index > sprites.Count - 1)
            {
                animation_index = 0;
            }

            m_sprite.sprite = sprites[animation_index];
        }

        if (look_at_player)
        {
            if (References.entity_tracker.player_decoys.Count > 0)
            {
                transform.up = Vector3.Lerp(transform.up,
                    (References.entity_tracker.player_decoys[0].transform.position - transform.position),
                    turn_speed);
            }
            else
            {
                transform.up = Vector3.Lerp(transform.up, (player.transform.position - transform.position), turn_speed);
            }
        }
        else if (look_with_velocity)
        {
            transform.up = Vector3.Lerp(transform.up, (m_rigidbody.velocity), turn_speed);
        }
        else if(m_rigidbody!=null /*Not always a rigidbody, ej, sub-sprites*/)
        {
            if (m_rigidbody.velocity.x > speed_threshold)
            {
                transform.up = Vector3.Lerp(transform.up, (new Vector3(1f * Mathf.Tan(turn_angle * Mathf.Deg2Rad), 1f, 0f)), turn_speed);
            }
            else if (m_rigidbody.velocity.x < -speed_threshold)
            {
                transform.up = Vector3.Lerp(transform.up, (new Vector3(-1f * Mathf.Tan(turn_angle * Mathf.Deg2Rad), 1f, 0f)), turn_speed);
            }
            else
            {
                transform.up = Vector3.Lerp(transform.up, (new Vector3(0f, 1f, 0f)), turn_speed);
            }
        }
    }
}
