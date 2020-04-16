using MyBox;
using System;
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
    public string nodeTag = "Node";
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float fireCountdown = 0f;
    public Node motherNode { get; set; }

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

    [Header("Penetrative")]
    public bool isPenetrative;
    [ConditionalField(nameof(isPenetrative))] public int penetration;

    [Header("Ground")]
    public bool isGroundType;
    [ConditionalField(nameof(isGroundType))] private readonly float SPAWN_DISTANCE = 1.65f;
    [ConditionalField(nameof(isGroundType))] private List<GroundProjectile> bulletList;

    [Header("Ray")]
    public bool isRay;
    [ConditionalField(nameof(isRay))] public GameObject hitEffect;

    [Header("Splitshot")]
    public bool isSplitshot;
    [ConditionalField(nameof(isSplitshot))] public int targetCount;
    private List<Transform> splitshotTargets;
    private List<Enemy> splitshotEnemyTargets;

    private void Start()
    {
        if (isSplitshot)
        {
            splitshotTargets = new List<Transform>();
            splitshotEnemyTargets = new List<Enemy>();
        }

        if (isGroundType)
        {
            bulletList = new List<GroundProjectile>();
            SpawnGroundPoints();
        }
        else
        {
            InvokeRepeating("UpdateTarget", 0f, 0.15f);
        }
    }

    private void SpawnGroundPoints()
    {
        Vector2 pos = transform.position;
        float x = pos.x;
        float y = pos.y;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(x + SPAWN_DISTANCE, y), 0.5f);
        bool caseBreakFlag = false;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    colliders = Physics2D.OverlapCircleAll(new Vector2(x + SPAWN_DISTANCE, y), 0.05f);

                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].CompareTag(nodeTag))
                        {
                            caseBreakFlag = true;


                        }
                    }

                    if (caseBreakFlag)
                    {
                        caseBreakFlag = false;
                        break;
                    }

                    GameObject go = (GameObject)Instantiate(bulletPrefab,
                        new Vector3(x + SPAWN_DISTANCE, y, transform.position.z),
                        Quaternion.identity);
                    bulletList.Add(go.GetComponent<GroundProjectile>());

                    break;
                case 1:
                    colliders = Physics2D.OverlapCircleAll(new Vector2(x - SPAWN_DISTANCE, y), 0.05f);

                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].CompareTag(nodeTag))
                        {
                            caseBreakFlag = true;


                        }
                    }

                    if (caseBreakFlag)
                    {
                        caseBreakFlag = false;
                        break;
                    }

                    GameObject go1 = (GameObject)Instantiate(bulletPrefab,
                        new Vector3(x - SPAWN_DISTANCE, y, transform.position.z),
                        Quaternion.identity);
                    bulletList.Add(go1.GetComponent<GroundProjectile>());

                    break;
                case 2:
                    colliders = Physics2D.OverlapCircleAll(new Vector2(x, y + SPAWN_DISTANCE), 0.05f);

                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].CompareTag(nodeTag))
                        {
                            caseBreakFlag = true;


                        }
                    }

                    if (caseBreakFlag)
                    {
                        caseBreakFlag = false;
                        break;
                    }

                    GameObject go2 = (GameObject)Instantiate(bulletPrefab,
                        new Vector3(x, y + SPAWN_DISTANCE, transform.position.z),
                        Quaternion.identity);
                    bulletList.Add(go2.GetComponent<GroundProjectile>());

                    break;
                case 3:
                    colliders = Physics2D.OverlapCircleAll(new Vector2(x, y - SPAWN_DISTANCE), 0.05f);

                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].CompareTag(nodeTag))
                        {
                            caseBreakFlag = true;


                        }
                    }

                    if (caseBreakFlag)
                    {
                        caseBreakFlag = false;
                        break;
                    }

                    GameObject go3 = (GameObject)Instantiate(bulletPrefab,
                        new Vector3(x, y - SPAWN_DISTANCE, transform.position.z),
                        Quaternion.identity);

                    bulletList.Add(go3.GetComponent<GroundProjectile>());

                    break;
            }
        }

        foreach (GroundProjectile groundProjectile in bulletList)
        {
            groundProjectile.damage = damage;
            groundProjectile.penetration = penetration;
            groundProjectile.isMagical = isMagical;
        }
    }

    private void Update()
    {

        if (!isGroundType)
        {
            fireCountdown -= Time.deltaTime;

            if (!isSplitshot)
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
    }

    void UpdateTarget()
    {
        //TODO: OverlapCircle
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        if (!isSplitshot)
        {
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
        else
        {
            for (int i = 0; i < splitshotEnemyTargets.Count; i++)
            {
                if (splitshotEnemyTargets[i] == null)
                {
                    splitshotEnemyTargets.Remove(splitshotEnemyTargets[i]);
                }
            }


            foreach (var enemy in enemies)
            {

                if (!splitshotEnemyTargets.Contains(enemy.GetComponent<Enemy>()))
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    }

                    if (nearestEnemy != null && shortestDistance <= range && splitshotEnemyTargets.Count < targetCount)
                    {

                        splitshotEnemyTargets.Add(nearestEnemy.GetComponent<Enemy>());
                        shortestDistance = Mathf.Infinity;
                    }

                }
                else
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy > range)
                    {

                        splitshotEnemyTargets.Remove(enemy.GetComponent<Enemy>());
                        splitshotTargets.Remove(enemy.transform);
                    }

                }

            }


        }

    }

    void Shoot()
    {


        if (!isSplitshot)
        {
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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
            if (isPenetrative)
            {
                bullet.isPenetrative = isPenetrative;
                bullet.penetration = penetration;
            }
            if (bullet != null)
                bullet.Seek(target);

            if (isRay)
            {

                bullet.target = enemyTarget.transform;
                bullet.transform.position = enemyTarget.transform.position;

                if (isLinearAOE)
                {
                    Debug.Log(bullet.transform.position);
                }
                if (hitEffect != null)
                {
                    GameObject effect = (GameObject)Instantiate(hitEffect,
                        new Vector3(enemyTarget.transform.position.x,
                        enemyTarget.transform.position.y,
                        -100),
                        Quaternion.identity);

                    Destroy(effect, 0.5f);
                }
            }

        }
        else
        {
            foreach (Enemy enemy in splitshotEnemyTargets)
            {

                GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                BulletAI bullet = bulletGO.GetComponent<BulletAI>();
                bullet.damage = damage;
                bullet.speed = bulletSpeed;
                bullet.aoeRadius = aOE;
                bullet.isMagical = isMagical;
                bullet.targetEnemy = enemy;
                if (isLinearAOE)
                {
                    bullet.isLinearAOE = true;
                    bullet.linearAOEProjectile = linearAOEProjectile;
                    bullet.travelDistance = linearTravelDistance;

                    bullet.travelSpeed = linearTravelSpeed;

                }
                if (isPenetrative)
                {
                    bullet.isPenetrative = isPenetrative;
                    bullet.penetration = penetration;
                }
                if (bullet != null && enemy != null)
                    bullet.Seek(enemy.transform);

                if (isRay)
                {

                    bullet.target = enemy.transform;
                    bullet.transform.position = enemy.transform.position;

                    if (isLinearAOE)
                    {
                        Debug.Log(bullet.transform.position);
                    }
                    if (hitEffect != null)
                    {
                        GameObject effect = (GameObject)Instantiate(hitEffect,
                            new Vector3(enemy.transform.position.x,
                            enemy.transform.position.y,
                            -100),
                            Quaternion.identity);

                        Destroy(effect, 0.5f);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }


}
