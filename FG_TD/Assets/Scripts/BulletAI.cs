
using System;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private Vector3 lastTargetPosition;

    public int damage = 1;
    public bool shouldTurn;

    [Header("For AOE")]
    public float aoeRadius = 0;


    private void Awake()
    {
        lastTargetPosition = new Vector3();
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }

    private void Update()
    {
        if (target == null && aoeRadius <= 0)
        {
            Destroy(gameObject);
            return;
        }
        else if (target == null != aoeRadius > 0)
        {
            MoveAoeProjectile();
            return;
        }

        MoveSingleTargetProjectile();
    }

    private void MoveAoeProjectile()
    {
        if (target != null)
        {
            Vector2 dir = target.position - transform.position;
            float distanseThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanseThisFrame)
            {
                if (aoeRadius > 0f)
                {
                    Explode();
                }
                else
                    Damage(target.gameObject);
            }

            transform.Translate(dir.normalized * distanseThisFrame, Space.World);

            if (shouldTurn)
                transform.right = -dir.normalized;

            lastTargetPosition = target.transform.position;
        }
        else
        {
            Vector2 dir = target.position - lastTargetPosition;
            float distanseThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanseThisFrame)
            {
               
                    Explode();
                
            }

            transform.Translate(dir.normalized * distanseThisFrame, Space.World);

            if (shouldTurn)
                transform.right = -dir.normalized;
        }
    }

    void MoveSingleTargetProjectile()
    {

        Vector2 dir = target.position - transform.position;
        float distanseThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanseThisFrame)
        {
            if (aoeRadius > 0f)
            {
                Explode();
            }
            else
                Damage(target.gameObject);
        }

        transform.Translate(dir.normalized * distanseThisFrame, Space.World);

        if (shouldTurn)
            transform.right = -dir.normalized;
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform.gameObject);
            }
        }
       
    }

    void Damage(GameObject enemy)
    {
        Destroy(gameObject);
        if (enemy != null)
        {
            Enemy EnemyObj = enemy.GetComponent<Enemy>();
            EnemyObj.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }

}
