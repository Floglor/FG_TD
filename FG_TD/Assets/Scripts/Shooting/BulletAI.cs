
using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletAI : MonoBehaviour
{
    public float speed { get; set; }
    public Transform target { get; set; }
    public Enemy targetEnemy { get; set; }
    private Vector3 lastTargetPosition;

    public static String RailTag = "Rail";

    public int damage { get; set; }
    public bool isMagical { get; set; }

    public bool isPenetrative { get; set; }
    public int penetration { get; set; }

    //LINEAR AOE====================
    public bool isLinearAOE { get; set; }
    private GameObject rail { get; set; }
    public GameObject linearAOEProjectile { get; set; }
    private List<LinearWaveScript> linearWaves;
    private LinearWaveScript secondLinearWave;
    public float travelDistance { get; set; }
    public float travelSpeed { get; set; }

    private Vector2 lastEnemyVector;

    public TowerAI motherTower;
    //==============================




    public bool shouldTurn;


    public float aoeRadius { get; set; }


    private void Awake()
    {
        lastTargetPosition = new Vector3();
        
        

    }

    private void Start()
    {
        if (target != null)
        {
            Seek(target.transform);
            lastTargetPosition = target.transform.position;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Seek(Transform _target)
    {
        target = _target;
        targetEnemy = target.gameObject.GetComponent<Enemy>();
    }

    private void Update()
    {

        if (aoeRadius <= 0)
        {
            MoveSingleTargetProjectile();
        }
        else
        {
            MoveAoeProjectile();
        }


    }

    private void MoveAoeProjectile()
    {
        if (target != null)
        {
            Vector2 dir = target.position - transform.position;
            float distanseThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanseThisFrame)
            {
                if (isLinearAOE)
                    LinearExlode();
                else
                if (aoeRadius > 0f)
                {
                    RoundExplode();
                }
                else
                    Damage(target.gameObject, magical: isMagical);
            }

            transform.Translate(dir.normalized * distanseThisFrame, Space.World);

            if (shouldTurn)
                transform.right = -dir.normalized;

            lastTargetPosition = target.transform.position;
            lastEnemyVector = targetEnemy.movingDirection;
        }
        else
        {

            Vector2 dir = lastTargetPosition - transform.position;
            float distanseThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanseThisFrame)
            {
                if (isLinearAOE)
                    LinearExlode();
                else
                    RoundExplode();
            }

            transform.Translate(dir.normalized * distanseThisFrame, Space.World);

            if (shouldTurn)
                transform.right = -dir.normalized;
        }
    }

    void MoveSingleTargetProjectile()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 dir = target.position - transform.position;
        float distanseThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanseThisFrame)
            Damage(target.gameObject, magical: isMagical);


        transform.Translate(dir.normalized * distanseThisFrame, Space.World);

        if (shouldTurn)
            transform.right = -dir.normalized;
    }

    private void RoundExplode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {

                Damage(collider.transform.gameObject, magical: isMagical);
            }
        }

    }

    private void LinearExlode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(RailTag))
            {
                rail = collider.gameObject;
                SpawnLinearWaves(transform);
                Destroy(gameObject);
                return;
            }
        }
    }

    private void SpawnLinearWaves(Transform transfom)
    {

        linearWaves = new List<LinearWaveScript>();

        if (rail != null)
        {
            RailsS railScript = rail.GetComponent<RailsS>();
            bool firstSpawned = false;
            for (int i = 0; i < 2; i++)
            {
                GameObject wave = (GameObject)Instantiate(linearAOEProjectile,
                    lastTargetPosition,
                    Quaternion.identity);

                LinearWaveScript linearWaveScript = wave.GetComponent<LinearWaveScript>();
                if (linearWaveScript == null) Debug.LogError("LWS IS NULL");
                linearWaveScript.damage = damage;
                linearWaveScript.travelSpeed = travelSpeed;
                linearWaveScript.travelDistance = travelDistance;
                linearWaveScript.isMagical = isMagical;
                if (isPenetrative)
                    linearWaveScript.penetration = penetration;


                if (!firstSpawned)
                {
                    switch (railScript.orientation)
                    {
                        case Orientation.Horizontal:
                            linearWaveScript.travelVector = Vector2.right;
                            linearWaveScript.gameObject.transform.position =
                                new Vector3(lastTargetPosition.x,
                               railScript.yAlignment,
                                lastTargetPosition.z);
                            linearWaveScript.gameObject.transform.Rotate(0, 0, 90);
                            break;
                        case Orientation.Vertical:
                            linearWaveScript.travelVector = Vector2.up;
                            linearWaveScript.gameObject.transform.position =
                            new Vector3(railScript.xAlignment,
                               lastTargetPosition.y,
                               lastTargetPosition.z);
                            break;
                    }
                    firstSpawned = true;
                }
                else
                {
                    switch (railScript.orientation)
                    {
                        case Orientation.Horizontal:
                            linearWaveScript.travelVector = Vector2.left;
                            linearWaveScript.gameObject.transform.position =
                            new Vector3(lastTargetPosition.x,
                              railScript.yAlignment,
                               lastTargetPosition.z);
                            linearWaveScript.gameObject.transform.Rotate(0, 0, 90);
                            break;
                        case Orientation.Vertical:
                            linearWaveScript.travelVector = Vector2.down;
                            linearWaveScript.gameObject.transform.position =
                            new Vector3(railScript.xAlignment,
                              lastTargetPosition.y,
                               lastTargetPosition.z);
                            break;
                    }
                }

                if (targetEnemy != null)
                    linearWaveScript.damagedEnemies.Add(targetEnemy);
                else
                    Debug.Log("Target Enemy is null");

                linearWaves.Add(linearWaveScript);

            }
        }
    }

    public void Damage(GameObject enemy, bool magical)
    {
        Destroy(gameObject);
        if (enemy != null)
        {
            Enemy EnemyObj = enemy.GetComponent<Enemy>();
            if (isPenetrative)
                EnemyObj.TakeDamage(damage, penetration);
            else EnemyObj.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }

}
