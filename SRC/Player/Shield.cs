using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IDamageable
{

    //public GameObject user;
    public float hp = 1f;
    public float collision_damage = 1f;
    public float explosion_radius = 0.5f;
    public bool invulnerable = false;

    public GameObject explosion_prefab;
    bool dead = false;

    /* Not neeed with kinematic rigidbody
     * void Update () {
        transform.position = user.transform.position;
    }*/

    void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("Shield collision with: "+other.gameObject.name);
        IDamageable damageable = other.gameObject.GetComponent((typeof(IDamageable))) as IDamageable;
        if (damageable != null)
        {
            damageable.TakeDamage(collision_damage, "Shield collision");
        }
    }

    void IDamageable.TakeDamage(float damage, string origin = "Unkown")
    {
        if (!invulnerable)
        {
            hp -= damage;
        }
        if (hp <= 0)
        {
            Die();
        }
        if (damage > 0)
        {
            StartCoroutine(DamageExplosion(explosion_prefab, 3, 0.05f, explosion_radius));
        }
    }
    protected void Die()
    {
        if (!dead)
        {
            dead = true;
            StartCoroutine(DeathExplosion(explosion_prefab, 5, 0.05f, explosion_radius));
        }
    }
    public IEnumerator DamageExplosion(GameObject explosion_prefab, int explosion_num, float explosion_interval, float explosion_radius)
    {
        for (int i = 0; i < explosion_num; i++)
        {
            GameObject bullet = (GameObject)Instantiate(explosion_prefab, transform.position + (Vector3)(Random.insideUnitCircle * explosion_radius), transform.rotation);

            yield return new WaitForSeconds(explosion_interval);
        }
    }
    public IEnumerator DeathExplosion(GameObject explosion_prefab, int explosion_num, float explosion_interval, float explosion_radius)
    {
        for (int i = 0; i < explosion_num; i++)
        {
            GameObject bullet = (GameObject)Instantiate(explosion_prefab, transform.position + (Vector3)(Random.insideUnitCircle * explosion_radius), transform.rotation);

            yield return new WaitForSeconds(explosion_interval * i);
        }

        Destroy(gameObject);
    }
}
