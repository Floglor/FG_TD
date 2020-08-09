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
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float distanceThisFrame = travelSpeed * Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
        travelledDistance += distanceThisFrame;
        if (travelledDistance- travelDistance >= 0.04f)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(travelVector.normalized * distanceThisFrame, Space.World);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is CircleCollider2D) return;
        if (collision.gameObject.CompareTag(enemyTag))
                Damage(collision.gameObject);

        if (collision.gameObject.CompareTag(nodeTag))
        {
            travelVector *= -1;
        }
    }

    private void Damage(GameObject enemy)
    {
        if (enemy == null) return;
        Enemy enemyObj = enemy.GetComponent<Enemy>();
            
        if (isDamaging) CheckAndApplyDot(enemyObj);
            
        if (penetration > 0)
            enemyObj.TakeDamage(damage, penetration);
        else enemyObj.TakeDamage(damage, isMagical);
    }
}
