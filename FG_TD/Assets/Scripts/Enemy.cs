using System;
using UnityEngine;
using UnityEngine.UI;
public class Enemy: MonoBehaviour
{
    public float speed;
    public int startHealth;
    public int worth;
    public int armor;
    private int health;

    private Transform target;
    private int waypointIndex = 0;

    [Header("Unity Specific")]
    public GameObject deathEffect;
    public Image healthBar;

    private void Start()
    {
        health = startHealth; 
        target = Waypoints.points[0];
    }

    private void Update()
    {
        Vector2 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, target.position) <= 0.04f)
        {
            GetNextWaypoint();
        }


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

        health -= damage;

        healthBar.fillAmount = (float) health / (float) startHealth;
     
        if (health <= 0)
            Die();
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
