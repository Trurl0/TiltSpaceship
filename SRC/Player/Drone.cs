using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IDamageable
{

    public float hp = 10f;
    public float orbit_radius = 0.1f;
    public float speed = 0.1f;
    public bool clockwise = false;
    private float angle = 0f;  // To set the next movement
    public float collision_damage = 1f;

    public bool invulnerable = false; // Attack shield activates this for all drones
    public bool dead = false;
    public GameObject explosion_prefab;
    protected Rigidbody2D m_rigidbody;
    protected Pause pauser;
    protected GameObject player;

    public GunUpgrades gun_upgrades;

    // Use this for initialization
    void Start () {

        m_rigidbody = GetComponent<Rigidbody2D>();
        pauser = References.pauser;
        player = References.player;

    }
	
	// Update is called once per frame
	protected virtual void FixedUpdate ()
    {

        if(!pauser.paused)
        { 
            Vector3 vector_to_player = (Vector2)(transform.position - player.transform.position).normalized;

            // orbital movement
            Vector3 trajectory = new Vector2(vector_to_player.y, -vector_to_player.x);
            Vector3 new_position = transform.position + trajectory*speed;

            // Set at fixed distance to player
            transform.position = player.transform.position + (new_position - player.transform.position).normalized * orbit_radius;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("Drone collision with " + other.gameObject.name);

        IDamageable damageable = other.gameObject.GetComponent((typeof(IDamageable))) as IDamageable;
        if (damageable != null)
        {
            damageable.TakeDamage(collision_damage, "Drone collision");
        }
    }

    void IDamageable.TakeDamage(float damage, string origin = "Unkown")
    {
        //Hack for fast collisions, no damageable if shield is up
        if (!invulnerable && GetComponentInChildren<Shield>() == null)
        {
            hp -= damage;

            if (hp <= 0) // If not =<, 0 is still alive!
            {
                Die();
            }
            if (damage > 0)
            {
                StartCoroutine(DamageExplosion(explosion_prefab, 2, 0.1f, 0.3f));
            }
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

    void Die()
    {
        if (!dead)
        {
            dead = true;
            StartCoroutine(DeathExplosion(explosion_prefab, 6, 0.05f, 0.3f));
        }

    }

    public IEnumerator DeathExplosion(GameObject explosion_prefab, int explosion_num, float explosion_interval, float explosion_radius)
    {
        for (int i = 0; i < explosion_num; i++)
        {
            GameObject bullet = (GameObject)Instantiate(explosion_prefab, transform.position + (Vector3)(Random.insideUnitCircle * explosion_radius), transform.rotation);

            yield return new WaitForSeconds(explosion_interval * i);
        }

        //GetComponent<SpriteRenderer>().enabled = false;
        //yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    [System.Serializable]
    public class GunUpgrades
    {
        public float damage;
        public float fire_cooldown;
        public int burst_size;
    }

}
