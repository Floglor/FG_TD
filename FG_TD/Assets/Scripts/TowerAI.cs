using UnityEngine;

public class TowerAI : MonoBehaviour
{
    private Transform target;
    [Header("Attributes")]
    public float range = 2.3f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("UnitySetup")]
    public string enemyTag = "Enemy";

    public GameObject bulletPrefab;
    public Transform firePoint;
 

    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
    }

    private void Update()
    {
      if(target == null)
        {
            return;
        }

      if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
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
        if (bullet != null)
            bullet.Seek(target);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
