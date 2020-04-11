using MyBox;
using System.Collections.Generic;
using UnityEngine;



public class TowerAI : MonoBehaviour
{
    private Transform target;
    private Enemy enemyTarget;
    [Header("Attributes")]
    public float range = 2.3f;
    public float fireRate = 1f;

    public int damage;
    public float bulletSpeed;

    

    [Header("UnitySetup")]
    public string enemyTag = "Enemy";
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float fireCountdown = 0f;

    [Header("Upgrades")]
    public List<UpgradeVariants> upgradeVariants;


    [Header("Linear AOE")]
    public bool isLinearAOE;
    [ConditionalField(nameof(isLinearAOE))] public float aOE;
    [ConditionalField(nameof(isLinearAOE))] public GameObject linearAOEProjectile;
    [ConditionalField(nameof(isLinearAOE))] public float linearTravelDistance;
    [ConditionalField(nameof(isLinearAOE))] public float linearTravelSpeed;

    [Header("Magical")]
    public bool isMagical;

    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.15f);
    }

    private void Update()
    {


        fireCountdown -= Time.deltaTime;

        if (target == null)
        {
            return;
        }

      if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

       
    }

    void UpdateTarget()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            enemyTarget = nearestEnemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        GameObject bulletGO = (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        BulletAI bullet = bulletGO.GetComponent<BulletAI>();
        bullet.damage = damage;
        bullet.speed = bulletSpeed;
        bullet.aoeRadius = aOE;
        bullet.isMagical = isMagical;
        bullet.targetEnemy = enemyTarget;
        if (isLinearAOE)
        {
            bullet.isLinearAOE = true;
            bullet.linearAOEProjectile = linearAOEProjectile;
            bullet.travelDistance = linearTravelDistance;
            
            bullet.travelSpeed = linearTravelSpeed;
            
        }
        if (bullet != null)
            bullet.Seek(target);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }

   
}
