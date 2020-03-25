using System;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public float speed = 10f;

    public int health = 1;

    public int price = 1;

    private Transform target;
    private int waypointIndex = 0;

    public GameObject deathEffect;

    private void Start()
    {
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
       
        health -= damage;
     
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject effectInst = (GameObject)Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y, -100), transform.rotation);
       
        Destroy(effectInst, 2f);
        PlayerStats.Money += price;
    }

    void DamagePlayer()
    {
        Destroy(gameObject);
        PlayerStats.Lives--;
        return;
    }
}
