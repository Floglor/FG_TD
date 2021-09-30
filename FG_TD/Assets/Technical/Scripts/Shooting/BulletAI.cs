using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Managers;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Shooting
{
    public class BulletAI : Projectile
    {
        private Vector2 lastEnemyVector;
        private Transform previousTarget;
        private Vector3 prevTargetPosition;

        public bool isSpawnedWithSkill;
        public Vector3 spawnCoordinatesChainLightning;

        private GameObject lightningGO;
        private GameObject firstLightningPoint;
        private GameObject secondLightningPoint;

        private byte lightningOrder;


        //private bool chainLightingStarted;


        protected void Awake()
        {
            lastTargetPosition = new Vector3();
            effects = new List<Effect>();
        }

        private void Start()
        {
            //Debug.Log(effects.Count);
            ApplyEffectStatChanges();

            isMine = false;
            isMineBlownOff = false;

            if (isChainLightning)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
                damagedEnemies = new List<Transform>();

                //sprites = Resources.LoadAll<Sprite>("Graphics/" + lightningTextures.name);

                pauseTimeCurrent = GraphicPauseTime;
                previousTarget = target;
                if (target == null)
                {
                    Destroy(gameObject);
                    return;
                }

                Vector3 position = previousTarget.position;
                prevTargetPosition = new Vector3(position.x, position.y, position.z);
                SpawnLightingTexture(
                    isSpawnedWithSkill ? spawnCoordinatesChainLightning : motherTower.firePoint.position,
                    target.position, true);
                DamageWithoutDestroy(target.gameObject, isMagical);
                damagedEnemies.Add(target);
            }

            if (target != null)
            {
                Seek(target.transform);
                lastTargetPosition = target.transform.position;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void Update()
        {
            if (isMine)
            {
                if (isMineBlownOff)
                {
                    if (isChainLightning)
                    {
                        if (ChainUpdate()) return;
                        return;
                    }
                }
            }
            else
            {
                if (isChainLightning)
                {
                    if (ChainUpdate()) return;
                    return;
                }
            }


            if (isMine) return;


            if (aoeRadius <= 0)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                MoveSingleTargetProjectile();
            }
            else
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                MoveAoeProjectile();
            }
        }

        private bool ChainUpdate()
        {
            if (chainLength <= 0)
            {
                Destroy(gameObject);
                return true;
            }

            if (pauseTimeCurrent <= 0)
            {
                ChainSeekAndShoot();
            }
            else
            {
                pauseTimeCurrent -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
            }

            return false;
        }

        private bool ChainCheckAndChangeDamage()
        {
            if (!chainChangesDamage) return false;

            if (damageChangePercentage > 0)
            {
                if (damage > 0)
                {
                    damage -= damage * damageChangePercentage / 100;
                    if (damage < 0) damage = 0;
                }

                if (penetration > 0)
                {
                    penetration -= penetration * damageChangePercentage / 100;
                    if (penetration < 0) penetration = 0;
                }

                if (disruption > 0)
                {
                    disruption -= disruption * damageChangePercentage / 100;
                    if (disruption < 0) disruption = 0;
                }
            }

            if (damageChangeFlat > 0)
            {
                if (damage > 0)
                {
                    damage -= damageChangeFlat;
                    if (damage < 0) damage = 0;
                }

                if (penetration > 0)
                {
                    damage -= damageChangeFlat;
                    if (penetration < 0) penetration = 0;
                }

                if (disruption > 0)
                {
                    disruption -= damageChangeFlat;
                    if (disruption < 0) disruption = 0;
                }
            }


            return damage != 0 || penetration != 0 || disruption != 0;
        }

        private void ChainSeekAndShoot()
        {
            //if damage = 0 - destroy and return
            if (!ChainCheckAndChangeDamage())
            {
                Destroy(lightningGO);
                Destroy(gameObject);
                return;
            }

            Collider2D[] colliders;

            colliders = Physics2D.OverlapCircleAll(
                previousTarget != null ? previousTarget.position : prevTargetPosition, chainTargetRange);

            List<Collider2D> fixedColliders = Utils.RemoveEnemyOverlapRepetitions(colliders);

            float shortestDistance = Mathf.Infinity;
            GameObject closestEnemy = null;


            foreach (Collider2D colliderTwoDee in fixedColliders)
            {
                if (ReferenceEquals(colliderTwoDee, null)) break;

                if (!colliderTwoDee.CompareTag(Enemy.MyTag)) continue;

                bool alreadyDamaged = false;

                foreach (Transform transform in damagedEnemies.Where(transform =>
                    colliderTwoDee.gameObject.transform == transform))
                {
                    alreadyDamaged = true;
                }

                if (alreadyDamaged) continue;
                // if (!ReferenceEquals(previousTarget, null))
                if (previousTarget != null)
                {
                    if (colliderTwoDee.gameObject == previousTarget.gameObject) continue;
                }
                else
                {
                    Destroy(lightningGO);
                }

                float distanceToEnemy = Vector2.Distance(prevTargetPosition,
                    colliderTwoDee.transform.position);

                if (!(distanceToEnemy < shortestDistance)) continue;

                shortestDistance = distanceToEnemy;
                closestEnemy = colliderTwoDee.gameObject;
            }

            if (closestEnemy != null)
            {
                Vector3 closestEnemyPosition1 = closestEnemy.transform.position;

                Vector3 closestEnemyPosition = closestEnemyPosition1;
                if (previousTarget != null)
                {
                    SpawnLightingTexture(previousTarget.transform, closestEnemy.transform);
                }
                else
                {
                    //SpawnLightingTexture(prevTargetPosition, closestEnemyPosition);
                    MoveLightningTexture(closestEnemyPosition);
                }

                previousTarget = closestEnemy.transform;
                Vector3 position = closestEnemyPosition;
                prevTargetPosition = new Vector3(position.x, position.y, position.z);
                DamageWithoutDestroy(closestEnemy, isMagical);
                damagedEnemies.Add(closestEnemy.transform);
                chainLength--;
                pauseTimeCurrent = GraphicPauseTime;
            }
            else
            {
                Destroy(lightningGO);
                Destroy(gameObject);
            }
        }

        private void SpawnLightingTexture(Transform previousTargetTransform, Transform closestEnemyTransform)
        {
            Vector3 firstPoint = previousTargetTransform.position;
            Vector3 secondPoint = closestEnemyTransform.position;

            lightningOrder = 1;

            if (isChainLightning)
                lightning = PlayerStats.instance.lightning;

            if (lightningGO != null)
            {
                Destroy(lightningGO);
            }

            lightningGO = Instantiate(
                lightning,
                new Vector3(0, 0, -100),
                Quaternion.identity);

            List<GameObject> lightObjects = lightningGO.GetAllChildren();


            firstLightningPoint = lightObjects[0];
            secondLightningPoint = lightObjects[1];

            firstLightningPoint.transform.position = firstPoint;
            firstLightningPoint.transform.parent = previousTargetTransform;
            previousTargetTransform.GetComponent<Enemy>().lightningPoints.Add(firstLightningPoint);
            secondLightningPoint.transform.position = secondPoint;
            closestEnemyTransform.GetComponent<Enemy>().lightningPoints.Add(secondLightningPoint);
            secondLightningPoint.transform.parent = closestEnemyTransform;

            GameObject beam1 = Instantiate(PlayerStats.instance.lightningBeam, firstLightningPoint.transform.position,
                Quaternion.identity,
                previousTargetTransform);
            GameObject beam2 = Instantiate(PlayerStats.instance.lightningBeam, secondLightningPoint.transform.position,
                Quaternion.identity,
                closestEnemyTransform);

            Destroy(beam1, 1f);
            Destroy(beam2, 1f);
            Destroy(lightningGO, GraphicPauseTime);
        }


        private void MoveLightningTexture(Vector3 closestEnemyPosition)
        {
            if (firstLightningPoint == null || secondLightningPoint == null)
                return;

            if (lightningOrder == 1)
            {
                firstLightningPoint.transform.position = closestEnemyPosition;
                lightningOrder = 2;
            }
            else
            {
                secondLightningPoint.transform.position = closestEnemyPosition;
                lightningOrder = 1;
            }
        }

        private void SpawnLightingTexture(Vector3 firstPoint, Vector3 secondPoint, bool firstTime = false)
        {
            /* firstPoint.z = 0;
             secondPoint.z = 0;
 
             // Debug.Log($@"firstPoint = {firstPoint}, secondPoint = {secondPoint}");
             float firstToSecondVectorDistance = Vector3.Distance(firstPoint, secondPoint);
             Vector2 dir = secondPoint - firstPoint;
 
             Vector3 spawnPoint = (firstPoint + secondPoint) / 2;
 
             spawnPoint.z = -100;*/

            lightningOrder = 1;

            if (isChainLightning)
                lightning = PlayerStats.instance.lightning;

            if (lightningGO != null)
            {
                Destroy(lightningGO);
            }

            lightningGO = Instantiate(
                lightning,
                new Vector3(0, 0, -100),
                Quaternion.identity);

            List<GameObject> lightObjects = lightningGO.GetAllChildren();


            firstLightningPoint = lightObjects[0];
            secondLightningPoint = lightObjects[1];

            firstLightningPoint.transform.position = firstPoint;
            secondLightningPoint.transform.position = secondPoint;


            /* go.transform.up = dir.normalized;
 
             SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
             if (spriteRenderer == null)
             {
                 Debug.LogError("GO = NULL");
             }
 
             spriteRenderer.sprite = sprites[(int) Random.Range(1f, 8f)];
 
             Vector3 transformLocalScale = go.transform.localScale;
 
 
             transformLocalScale.x = 1f;
             transformLocalScale.y = firstToSecondVectorDistance;
 
 
             go.transform.localScale = transformLocalScale;
 
 
             //Debug.Log(firstToSecondVectorDistance);
 
             //if (firstTime) firstToSecondVectorDistance /= 2.95f;
             //ConvertDistanceToScale(go, firstToSecondVectorDistance);
             Destroy(go, 0.2f);
             */
        }

        public void ConvertDistanceToScale(GameObject go, float distance)
        {
            Vector3 lossyScale = go.transform.lossyScale;
            Vector2 size = lossyScale;
            size.y = distance;
            Vector3 localScale = go.transform.localScale;
            size.y /= lossyScale.y;
            size.x = localScale.x;
            localScale = size;
            go.transform.localScale = localScale;
        }

        public void Seek(Transform _target)
        {
            target = _target;
            targetEnemy = target.gameObject.GetComponent<Enemy>();
        }


        private void MoveAoeProjectile()
        {
            if (target != null)
            {
                Vector2 dir = target.position - transform.position;
                float distanceThisFrame = speed * Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

                if (dir.magnitude <= distanceThisFrame)
                {
                    if (isLinearAOE)
                        LinearExplode();
                    else if (aoeRadius > 0f)
                    {
                        RoundExplode();
                    }
                    else
                        Damage(target.gameObject, magical: isMagical);
                }

                transform.Translate(dir.normalized * distanceThisFrame, Space.World);

                if (shouldTurn)
                    transform.right = -dir.normalized;

                lastTargetPosition = target.transform.position;
                lastEnemyVector = targetEnemy.movingDirection;
            }
            else
            {
                Vector2 dir = lastTargetPosition - transform.position;
                float distanceThisFrame = speed * Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

                if (dir.magnitude <= distanceThisFrame)
                {
                    if (isLinearAOE)
                        LinearExplode();
                    else
                        RoundExplode();
                }

                transform.Translate(dir.normalized * distanceThisFrame, Space.World);

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
            float distanceThisFrame = speed * Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

            if (dir.magnitude <= distanceThisFrame)
                Damage(target.gameObject, magical: isMagical);


            transform.Translate(dir.normalized * distanceThisFrame, Space.World);

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


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}