using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearWaveScript : MonoBehaviour
{

    public float travelDistance { get; set; }
    public TowerAI motherTower {get; set;}
    public float travelSpeed { get; set; }
    public int damage { get; set; }
    public Vector2 travelVector { get; set; }

    public List<Enemy> damagedEnemies { get; set; }

    private float travelledDistance;

    private string enemyTag = "Enemy";

    private void Awake()
    {
        damagedEnemies = new List<Enemy>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float distanseThisFrame = travelSpeed * Time.deltaTime;
        travelledDistance += distanseThisFrame;
        if (travelledDistance- travelDistance >= 0.04f)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(travelVector.normalized * distanseThisFrame, Space.World);
    }

   

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(enemyTag))
            Destroy(gameObject);
    }

    public void Damage(GameObject enemy, bool magical)
    { 
        if (enemy != null)
        {
            Enemy EnemyObj = enemy.GetComponent<Enemy>();
            EnemyObj.TakeDamage(damage);
        }
    }
}
