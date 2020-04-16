using System;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    public float speed;
    public int startHealth;
    public int worth;
    public int armor;
    public bool isFlying;
    [SerializeField]
    private int health;

    private Transform target;
    private int waypointIndex = 0;



    [Header("Unity Specific")]
    public GameObject deathEffect;
    public Image healthBar;

    private Rigidbody2D rb;
    public Vector2 movingDirection { get; set; }

    private void Start()
    {
        health = startHealth;
        target = Waypoints.points[0];
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movingDirection = target.position - transform.position;
        MoveWithTranslate(movingDirection);

        if (Vector2.Distance(transform.position, target.position) <= 0.04f)
        {
            GetNextWaypoint();
        }


    }

    void MoveWithPhysics(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;
    }

    void MoveWithTranslate(Vector2 dir)
    {
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
    }

    private void FixedUpdate()
    {
        //MoveWithPhysics(direction);
    }

    private void GetNextWaypoint()
    {
        if (waypointIndex >= Waypoints.points.Length - 1)
        {
            PlayerStats.Lives--;
            DamagePlayer();
            return;
        }
        waypointIndex++;
        target = Waypoints.points[waypointIndex];
    }

    public void TakeDamage(int damage)
    {
        if (damage - armor <= 0)
            return;

        if (armor < 0)
            health -= damage - (armor * 2);
        else
            health -= damage - armor;

        healthBar.fillAmount = (float)health / (float)startHealth;

        if (health <= 0)
            Die();
    }

    public void TakeDamage(int damage, bool isMagical)
    {
        if (isMagical)
        {
            health -= damage;

            healthBar.fillAmount = (float)health / (float)startHealth;

            if (health <= 0)
                Die();
        }
        else TakeDamage(damage);
    }

    public void TakeDamage(int damage, int penetrationDamage)
    {
        if (penetrationDamage - armor > 0)
        {
            health -= damage + ((penetrationDamage - armor)*2);

            healthBar.fillAmount = (float)health / (float)startHealth;

            if (health <= 0)
                Die();
        }
        else TakeDamage(damage+penetrationDamage);
    }



    private void Die()
    {
        Destroy(gameObject);
        GameObject effectInst = (GameObject)Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y, -100), transform.rotation);

        Destroy(effectInst, 2f);
        PlayerStats.Money += worth;
    }

    void DamagePlayer()
    {
        Destroy(gameObject);
        PlayerStats.Lives--;
        return;
    }
}
