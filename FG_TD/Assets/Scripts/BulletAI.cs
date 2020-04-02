
using System;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;

    public int damage = 1;
    public bool shouldTurn;

    [Header("For AOE")]
    public float aoeRadius = 0;

   

    public void Seek(Transform _target)
    {
        target = _target;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

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
