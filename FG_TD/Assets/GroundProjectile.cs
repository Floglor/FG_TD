using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundProjectile : MonoBehaviour
{
    public int damage { get; set; }
    public float lifetime { get; set; }
    public bool isMagical { get; set; }
    public bool isPenetrative { get; set; }

    private string enemyTag = "Enemy";
    public int penetration { get; set; }

    private void Start()
    {
     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
            if (!collision.gameObject.GetComponent<Enemy>().isFlying)
            Damage(collision.gameObject);

       
    }

    public void Damage(GameObject enemy)
    {
        if (enemy != null)
        {
            Enemy EnemyObj = enemy.GetComponent<Enemy>();
            if (penetration > 0)
                EnemyObj.TakeDamage(damage, penetration);
            else EnemyObj.TakeDamage(damage, isMagical);
        }
    }
}
