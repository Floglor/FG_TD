using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Shooting;
using UnityEngine;

public class LinearWaveScript : Projectile
{
    public Vector2 travelVector { get; set; }


    private float travelledDistance;

    private string nodeTag = "Node";


    private void Awake()
    {
        damagedEnemies = new List<Transform>();
        WaveSpawner.LinearWaves.Add(this);
    }

    private void DestroySelf()
    {
        WaveSpawner.LinearWaves.Remove(this);
        Destroy(gameObject);
    }
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float distanceThisFrame = travelSpeed * Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
        travelledDistance += distanceThisFrame;
        if (travelledDistance - travelDistance >= 0.04f)
        {
            DestroySelf();
            return;
        }

        transform.Translate(travelVector.normalized * distanceThisFrame, Space.World);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision is CircleCollider2D) return;
        if (collision.gameObject.CompareTag(enemyTag) && collision is BoxCollider2D)
            DamageWithoutDestroy(collision.gameObject, isMagical);

        if (collision.gameObject.CompareTag(nodeTag))
        {
            travelVector *= -1;
        }
    }

  
}