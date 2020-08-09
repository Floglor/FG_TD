using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyBox;
using UnityEngine;

namespace Shooting
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Vector3 lastTargetPosition;
        public float speed { get; set; }
        public Transform target { get; set; }
        public Enemy targetEnemy { get; set; }

        public static String RailTag = "Rail";

        public int damage { get; set; }
        public int disruption { get; set; }
        public bool isMagical { get; set; }

        public bool isPenetrative { get; set; }
        public int penetration { get; set; }

        //LINEAR AOE====================
        public bool isLinearAOE { get; set; }
        protected GameObject rail { get; set; }
        public GameObject linearAOEProjectile { get; set; }
        protected List<LinearWaveScript> linearWaves;
        protected LinearWaveScript secondLinearWave;
        public float travelDistance { get; set; }
        public float travelSpeed { get; set; }

        public TowerAI motherTower;

        //==============================

        //CHAIN
        public bool isChainLighting { get; set; }
        public int chainLength { get; set; }
        public float chainTargetRange { get; set; }
        protected const float GraphicPauseTime = 0.2f;
        public Texture2D lightingTextures;
        public GameObject spritePrefab;
        protected List<Transform> damagedEnemies;

        //private bool chainLightingStarted;

        protected float pauseTimeCurrent;

        protected Sprite[] sprites;
        //==============================

        public bool shouldTurn;
        protected String enemyTag = "Enemy";


        public float aoeRadius { get; set; }

        public bool isDamaging { get; set; }

        public bool isMine { get; set; }

        public bool isMineBlownOff { get; set; }
        public int dOTDamage { get; set; }
        public int dOTPenetration { get; set; }
        public bool dOTMagical { get; set; }
        public string dOTUniqueIdentifier { get; set; }
        public float debuffDuration { get; set; }
        public float debuffTickFrequency { get; set; }

        public bool isStackable { get; set; }
        
        public bool updatesAllSimilarDOTsCooldown { get; set; }

        public bool isSlowing { get; set; }

        public int slowRate { get; set; }

        public int slowIdentifier { get; set; }

        public List<Effect> effects { get; set; }

        private void Awake()
        {
            slowIdentifier = GetInstanceID();
           
        }

        protected void ApplyEffectStatChanges()
        {
            if (effects.IsNullOrEmpty()) return;

            foreach (Effect effect in effects)
            {
                effect.ChangeStats(this);
            }
        }

        protected void DamageWithoutDestroy(GameObject enemy, bool magical)
        {
            if (enemy == null) return;

            Enemy enemyObj = enemy.GetComponent<Enemy>();

            if (enemyObj == null) return;

            if (isDamaging) CheckAndApplyDot(enemyObj);

            if (isPenetrative)
            {
                enemyObj.TakeDamage(damage, penetration, disruption);
            }
            else
            {
                enemyObj.TakeDamage(damage, magical, disruption);
            }
        }

        protected void CheckAndApplyDot(Enemy enemy)
        {
            ApplyDamageDot(enemy);
        }

        private void ApplySlow(Enemy enemy)
        {
         

            List<SlowInstance> slowInstances = enemy.slowInstances;

            int biggestSlowInstance = slowRate;

            if (slowInstances.Count > 0)
                foreach (SlowInstance slowInstance in slowInstances.Where(slowInstance =>
                    slowInstance.slowAmount > slowRate))
                {
                    biggestSlowInstance = slowInstance.slowAmount;
                }

            for (int i = 0; i < slowInstances.Count; i++)
            {
                SlowInstance slowInstance = slowInstances[i];

                if (slowInstance.instanceId != slowIdentifier) continue;

                slowInstance.duration = debuffDuration;
                slowInstances[i] = slowInstance;
             
                enemy.ResetMVSP();
                return;
            }

           

            slowInstances.Add(new SlowInstance(slowIdentifier, slowRate, debuffDuration));

            

            if (biggestSlowInstance > slowRate) return;

            enemy.slowAmountPercentage = slowRate;
            enemy.ResetMVSP();
        }

        protected void ApplyDamageDot(Enemy enemy)
        {
            bool isAlreadyOn = false;

            if (enemy.damageDotInstances.Count > 0)
                foreach (DamageDotInstance enemyDamageDotInstance in
                    enemy.damageDotInstances.Where(enemyDamageDotInstance =>
                        enemyDamageDotInstance.uniqueIdentifier == dOTUniqueIdentifier))
                {
                    //Debug.Log($"{dOTUniqueIdentifier} and {enemyDamageDotInstance.uniqueIdentifier}");
                    isAlreadyOn = true;
                    
                    if (!updatesAllSimilarDOTsCooldown) continue;
                    
                    if (debuffDuration > enemyDamageDotInstance.dotDuration)
                        enemyDamageDotInstance.dotDuration = debuffDuration;
                }

            if (isSlowing)
            {
                ApplySlow(enemy);
            }

            if (!isStackable && !(this is GroundProjectile))
                if (isAlreadyOn) return;

            //Debug.Log("Inst add");
            enemy.AddDotInstance(new DamageDotInstance(debuffTickFrequency, dOTDamage, debuffDuration,
                GetInstanceID(), dOTMagical, dOTPenetration, dOTUniqueIdentifier,
                isStackable, updatesAllSimilarDOTsCooldown, this is GroundProjectile, isSlowing, slowRate));

          
        }

        protected void LinearExplode()
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

        protected void SpawnLinearWaves(Transform transformPoint)
        {
            linearWaves = new List<LinearWaveScript>();

            if (rail != null)
            {
                Rail railScript = rail.GetComponent<Rail>();
                bool firstSpawned = false;
                for (int i = 0; i < 2; i++)
                {
                    GameObject wave = (GameObject) Instantiate(linearAOEProjectile,
                        lastTargetPosition,
                        Quaternion.identity);

                    LinearWaveScript linearWaveScript = wave.GetComponent<LinearWaveScript>();
                    if (linearWaveScript == null) Debug.LogError("LWS IS NULL");
                    linearWaveScript.damage = damage;
                    linearWaveScript.travelSpeed = travelSpeed;
                    linearWaveScript.travelDistance = travelDistance;
                    linearWaveScript.isMagical = isMagical;
                    linearWaveScript.disruption = disruption;
                    if (isPenetrative)
                        linearWaveScript.penetration = penetration;

                    if (isDamaging)
                    {
                        linearWaveScript.dOTDamage = dOTDamage;
                        linearWaveScript.dOTMagical = dOTMagical;
                        linearWaveScript.isDamaging = isDamaging;
                        linearWaveScript.dOTUniqueIdentifier = dOTUniqueIdentifier;
                        linearWaveScript.dOTPenetration = dOTPenetration;
                        linearWaveScript.debuffDuration = debuffDuration;
                        linearWaveScript.debuffTickFrequency = debuffTickFrequency;
                    }


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

                    linearWaves.Add(linearWaveScript);
                }
            }
        }
    }
}