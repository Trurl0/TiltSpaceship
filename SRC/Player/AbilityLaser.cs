using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLaser : BaseAbility
{
    public float damage = 0.01f;
    public float fire_time = 1f;

    private Collider2D m_collider;
    private SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();

        // Start off
        m_SpriteRenderer.enabled = false;
        m_collider.enabled = false;
    }

    protected override void Use()
    {
            m_SpriteRenderer.enabled = true;
            m_collider.enabled = true;

            StartCoroutine(TunOff());
    }

    public IEnumerator TunOff()
    {
        yield return new WaitForSeconds(fire_time);
        m_SpriteRenderer.enabled = false;
        m_collider.enabled = false;
    }

    private void OnTriggerStay2D (Collider2D other)
    {
        //Debug.Log("Laser Trigger with " + other.gameObject.name);
        IDamageable damageable = other.gameObject.GetComponent((typeof(IDamageable))) as IDamageable;
        if (damageable != null && (other.gameObject.tag != "Player" && other.gameObject.tag != "Shield" && other.gameObject.tag != "PlayerDrone"))
        {
            damageable.TakeDamage(damage, "Laser");
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy bullet"))
        {
            Destroy(other.gameObject);
        }
    }


}
