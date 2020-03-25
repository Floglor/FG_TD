
using System;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;

    public int damage = 1;

   

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
            Damage(target.gameObject);
        }

        transform.Translate(dir.normalized * distanseThisFrame, Space.World);

        transform.right = -dir.normalized;


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

   
}
