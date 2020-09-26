using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Managers;
using MyBox;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Shooting
{
    [Serializable]
    public abstract class Modifier
    {
        public int instanceId { get; set; }
        public bool isTimed { get; set; }
        public bool isPercentage;
        public bool isFromAura { get; set; }    

        protected Modifier(int instanceId, bool isTimed, bool isPercentage, bool isFromAura)
        {
            this.instanceId = instanceId;
            this.isTimed = isTimed;
            this.isPercentage = isPercentage;
            this.isFromAura = isFromAura;
        }
    }

    [Serializable]
    public class IntegerTowerBuff : Modifier
    {
        public Stats.IntStatName intStat;
        public int amount;

        public IntegerTowerBuff(int instanceId, bool isTimed, bool isPercentage, bool isFromAura,
            Stats.IntStatName intStat, int amount) : base(instanceId, isTimed, isPercentage, isFromAura)
        {
            this.intStat = intStat;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class IntegerTowerDebuff : Modifier
    {
        public Stats.IntStatName intStat;
        public int amount;

        public IntegerTowerDebuff(int instanceId, bool isTimed, bool isPercentage, bool isFromAura,
            Stats.IntStatName intStat, int amount) : base(instanceId, isTimed, isPercentage, isFromAura)
        {
            this.intStat = intStat;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class FloatTowerBuff : Modifier
    {
        public Stats.FloatStatName floatStat;
        public float amount;

        public FloatTowerBuff(int instanceId, bool isTimed, bool isPercentage, bool isFromAura,
            Stats.FloatStatName floatStat, float amount) : base(instanceId, isTimed, isPercentage, isFromAura)
        {
            this.floatStat = floatStat;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class FloatTowerDebuff : Modifier
    {
        public Stats.FloatStatName floatStat;
        public float amount;

        public FloatTowerDebuff(int instanceId, bool isTimed, bool isPercentage, bool isFromAura,
            Stats.FloatStatName floatStat, float amount) : base(instanceId, isTimed, isPercentage, isFromAura)
        {
            this.floatStat = floatStat;
            this.amount = amount;
        }
    }


    [SuppressMessage("ReSharper", "InvertIf")]
    public class TowerAI : MonoBehaviour
    {
        private const string MyTag = "Tower";

        private Transform target;
        private Enemy enemyTarget;

        [FormerlySerializedAs("range")] [Header("Attributes")]
        public float startRange = 2.3f;

        public float range { get; set; }
        [FormerlySerializedAs("fireRate")] public float startingFireRate = 1f;
        public float fireRate { get; set; }


        [FormerlySerializedAs("aOE")] public float startAOE;
        public float aoe { get; set; }
        [FormerlySerializedAs("damage")] public int startDamage;
        public int damage;

        public int startDisruption;
        public int disruption;

        public float bulletSpeed;


        [Header("UnitySetup")] public string enemyTag = "Enemy";
        public string nodeTag = "Node";
        public GameObject bulletPrefab;
        public Transform firePoint;
        private float fireCountdown = 0f;
        public Node motherNode { get; set; }

        [Header("Upgrades")] public List<UpgradeVariants> upgradeVariants;


        [Header("Linear AOE")] public bool isLinearAOE;


        [ConditionalField(nameof(isLinearAOE))]
        public GameObject linearAOEProjectile;


        [FormerlySerializedAs("linearTravelDistance")] [ConditionalField(nameof(isLinearAOE))]
        public float startLinearTravelDistance;

        public float linearTravelDistance;

        [FormerlySerializedAs("linearTravelSpeed")] [ConditionalField(nameof(isLinearAOE))]
        public float startingLinearTravelSpeed;

        public float linearTravelSpeed { get; set; }

        [Header("Magical")] public bool isMagical;

        [Header("Penetrative")] public bool isPenetrative;

        [FormerlySerializedAs("penetration")] [ConditionalField(nameof(isPenetrative))]
        public int startPenetration;

        [ConditionalField(nameof(isPenetrative))]
        public int penetration;

        [Header("Ground")] public bool isGroundType;

        [ConditionalField(nameof(isGroundType))]
        private const float SpawnDistance = 1.65f;

        public List<GroundProjectile> groundProjectiles { get; set; }

        [Header("Ray")] public bool isRay;
        [ConditionalField(nameof(isRay))] public GameObject hitEffect;

        [Header("Splitshot")] public bool isSplitshot;

        [FormerlySerializedAs("targetCount")] [ConditionalField(nameof(isSplitshot))]
        public int startSplitshotTargetCount;

       public int splitshotTargetCount { get; set; }

        private List<Transform> splitshotTargets;
        private List<Enemy> splitshotEnemyTargets;

        [Header("Lightning")] public bool isChainLighting;

        [FormerlySerializedAs("chainLength")] [ConditionalField(nameof(isChainLighting))]
        public int startChainLength;

        public int chainLength { get; set; }

        [ConditionalField(nameof(isChainLighting))]
        public float startChainSeekRadius;

        public float chainSeekRadius { get; set; }

        [Header("Aura")] public bool isAura;

        [FormerlySerializedAs("auraRadius")] [ConditionalField(nameof(isAura))]
        public float startAuraRadius;

        public float auraRadius;

        [ConditionalField(nameof(isAura))] public bool isSlowing;

        [FormerlySerializedAs("towerSlowingPercentage")] [ConditionalField(nameof(isSlowing))]
        public int startSlowingAuraPercentage;

        [ConditionalField(nameof(isSlowing))] [Range(0, 100)]
        public int slowingAuraPercentage;

        [ConditionalField(nameof(isAura))] public bool isSnaring;

        [FormerlySerializedAs("snaringStrength")] [ConditionalField(nameof(isSnaring))]
        public int startSnaringStrength;

        [ConditionalField(nameof(isSnaring))] public int snaringStrength;

        [ConditionalField(nameof(isAura))] public bool isAffectingTowers;

        [ConditionalField(nameof(isAffectingTowers))]
        public bool isAttackBuffAura;

        [ConditionalField(nameof(isAttackBuffAura))]
        public int flatAttackBuff;

        [ConditionalField(nameof(isAttackBuffAura))]
        public bool auraAffectsPenDamage;
        
        [ConditionalField(nameof(auraAffectsPenDamage))]
        public bool auraAffectsOnlyPen;
        
        [ConditionalField(nameof(isAttackBuffAura))]
        public bool auraAffectsDisruptionDamage;
        
        [ConditionalField(nameof(auraAffectsDisruptionDamage))]
        public bool auraAffectsOnlyDisruption;

        [ConditionalField(nameof(isAttackBuffAura))]
        public int percentageAttackBuff;

        [ConditionalField(nameof(isAffectingTowers))]
        public bool isAttackSpeedBuffAura;

        [ConditionalField(nameof(isAttackSpeedBuffAura))]
        public float flatAttackSpeedBuff;

        [ConditionalField(nameof(isAttackSpeedBuffAura))]
        public int percentageAttackSpeedBuff;


        [ConditionalField(nameof(isAffectingTowers))]
        public bool isRadiusBuff;

        [ConditionalField(nameof(isRadiusBuff))]
        public int radiusBuffFlat;
        [ConditionalField(nameof(isRadiusBuff))]
        public int radiusBuffPercentage;


        [Header("MineTrap")] public bool isMineTrap;
        [ConditionalField(nameof(isMineTrap))] public int mineCountOneSide;
        [ConditionalField(nameof(isMineTrap))] public float mineAOE;
        [ConditionalField(nameof(isMineTrap))] public int mineSpawnPerWave;
        [ConditionalField(nameof(isMineTrap))] public int waveCounterMineSpawn;
        public int waveCounterForMines { get; set; }
        [ConditionalField(nameof(isMineTrap))] public GameObject mineGameObject;
        [ConditionalField(nameof(isMineTrap))] public GameObject blowEffect;

        [Header("EssenceGain")]
        public bool isFreeEssenceGain;

        [ConditionalField(nameof(isFreeEssenceGain))]
        public int essenceGainAmount;
        
        public bool isEnemySuckGain;

        [ConditionalField(nameof(isEnemySuckGain))] public int maxSuckAmount;

        [ConditionalField(nameof(isEnemySuckGain))] public bool isFullOfEssence;
        [ConditionalField(nameof(isEnemySuckGain))] public float suckAOE;
        [ConditionalField(nameof(isEnemySuckGain))] public int suckAmount;

        private List<Vector2> mineSpots;
        private List<Rail> railListForMines;

        private Vector3 priorityMineSpot;
        private List<Mine> priorityMines;
        private Vector3 notSoPriorityMineSpot;
        private List<Mine> notSoPriorityMines;

        [Header("DOT")] public bool isDOT;

        [FormerlySerializedAs("dOTDamage")] [ConditionalField(nameof(isDOT))]
        public int startDOTDamage;

        [HideInInspector] [ConditionalField(nameof(isDOT))]
        public int dOTDamage;

        [FormerlySerializedAs("dOTPenetration")] [ConditionalField(nameof(isDOT))]
        public int startingDOTPenetration;

        [ConditionalField(nameof(isDOT))] public int dOTPenetration;
        [ConditionalField(nameof(isDOT))] public bool dOTMagical;

        [FormerlySerializedAs("dOTUniqueIdentifier")] [ConditionalField(nameof(isDOT))]
        public string startingDOTUniqueIdentifier;

        [HideInInspector] [ConditionalField(nameof(isDOT))]
        public string dOTUniqueIdentifier;

        [FormerlySerializedAs("debuffDuration")] [ConditionalField(nameof(isDOT))]
        public float startDotDuration;

        [HideInInspector] [ConditionalField(nameof(isDOT))]
        public float dotDuration;

        [FormerlySerializedAs("debuffTickFrequency")] [ConditionalField(nameof(isDOT))]
        public float startDebuffTickFrequency;

        [ConditionalField(nameof(isDOT))] [HideInInspector]
        public float debuffTickFrequency;

        [ConditionalField(nameof(isDOT))] public bool isDOTSlowing;
        [ConditionalField(nameof(isDOT))] public bool isStackable;
        [ConditionalField(nameof(isDOT))] public bool updatesCooldown;

        [FormerlySerializedAs("dotSlowRate")] [ConditionalField(nameof(isDOTSlowing))]
        public int startDotSlowRate;

        public int dotSlowRate;


        private CircleCollider2D auraCollider;

        [Header("Round Attack")] public bool isRoundAttacking;

        [Header("Target Priority")] public bool oneShotTargetFist;
        public Type targetPriority;
        private int targetPriorityCount;
        public ShootingMode shootingMode;
        public bool isHoldingTarget;
        public GameObject tree;
        private Vector3 treeTransformPosition;


        [FormerlySerializedAs("effects")] [Header("Effects")]
        public List<FalseCrit> falseCrits;

        public int damageTotal { get; set; }
        public int totalCost;

        [Header("Buff Relation Logic (NOT USED)")] public bool notAffectedByAttackAuras;
        public bool notAffectedByAttackSpeedAuras;
        public bool notAffectedByRadiusAuras;


        public List<IntegerTowerBuff> integerTowerBuffs { get; set; }
        public List<IntegerTowerDebuff> integerTowerDebuffs { get; set; }
        public List<FloatTowerBuff> floatTowerBuffs { get; set; }
        public List<FloatTowerDebuff> floatTowerDebuffs { get; set; }

        

        private int id;

        [Header("External Node Buffs")] public bool editorShowExternalBuffs;

        public List<FloatTowerBuff> externalStatsBoostFloat;
        public List<IntegerTowerBuff> externalStatsBoostInt;

        [Header("Graphics")] public GameObject shootEffect;


        private void Awake()
        {
            id = GetInstanceID();


            //Debug.Log(PlayerStats.instance.towers.Count);
        }

        private void Start()
        {
            integerTowerBuffs = new List<IntegerTowerBuff>();
            integerTowerDebuffs = new List<IntegerTowerDebuff>();
            floatTowerBuffs = new List<FloatTowerBuff>();
            floatTowerDebuffs = new List<FloatTowerDebuff>();


            PlayerStats.instance.towers.Add(this);

            //short
            waveCounterForMines = 1;
            disruption = startDisruption;
            fireRate = startingFireRate;
            penetration = startPenetration;
            range = startRange;
            damage = startDamage;
            aoe = startAOE;
            linearTravelSpeed = startingLinearTravelSpeed;
            linearTravelDistance = startLinearTravelDistance;
            chainLength = startChainLength;
            chainSeekRadius = startChainSeekRadius;
            splitshotTargetCount = startSplitshotTargetCount;
            auraRadius = startAuraRadius;
            slowingAuraPercentage = startSlowingAuraPercentage;
            snaringStrength = startSnaringStrength;
            dotDuration = startDotDuration;
            debuffTickFrequency = startDebuffTickFrequency;
            dOTDamage = startDOTDamage;
            dOTPenetration = startingDOTPenetration;
            dOTUniqueIdentifier = startingDOTUniqueIdentifier;
            

            if (motherNode != null)
                if (motherNode.isExternal)
                {
                    if (!externalStatsBoostFloat.IsNullOrEmpty())
                    {
                        foreach (FloatTowerBuff stat in externalStatsBoostFloat)
                        {
                            Debug.Log("debug");
                            FloatStatRecalculate(stat.floatStat);
                        }

                        foreach (IntegerTowerBuff stat in externalStatsBoostInt)
                        {
                            IntegerStatRecalculate(stat.intStat);
                        }
                    }
                }

            foreach (FalseCrit falseCrit in falseCrits)
            {
                InitializeEffect(falseCrit);
            }

            if (isRoundAttacking)
            {
                isRay = true;
                isSplitshot = false;
                InvokeRoundAttacking();
                return;
            }

            if (isMineTrap)
            {
                mineSpots = new List<Vector2>();
                LocateMineSpots();
                WaveSpawner.instance.mineTowerList.Add(this);
            }

            if (isFreeEssenceGain || isEnemySuckGain)
            {
                WaveSpawner.instance.essenceTowerList.Add(this);
            }

            railListForMines = new List<Rail>();
            treeTransformPosition = tree.transform.position;
            
            if (!isEnemySuckGain && !(suckAOE > 0))
            {
                if (isAura)
                {
                    auraCollider = GetComponent<CircleCollider2D>();
                    auraCollider.radius = auraRadius; //TODO: AuraradiusBuffChange
                    return;
                }
            }
            else
            {
                auraCollider = GetComponent<CircleCollider2D>();
                auraCollider.radius = suckAOE; //TODO: AuraradiusBuffChange
                return;
            }

            if (isSplitshot)
            {
                splitshotTargets = new List<Transform>();
                splitshotEnemyTargets = new List<Enemy>();
            }

            if (isGroundType)
            {
                groundProjectiles = new List<GroundProjectile>();
                SpawnGroundPoints();
            }
            else if (isMineTrap)
            {
            }
            else if (!isRoundAttacking)
            {
                InvokeRepeating("UpdateTarget", 0f, 0.15f);
            }
        }

        public void FloatStatRecalculate(Stats.FloatStatName floatStatName)
        {
            float totalStatFlatBuff = 0;
            int totalStatPercentageBuff = 0;

            float totalStatFlatDebuff = 0;
            int totalStatPercentageDebuff = 0;

            float biggestBuffFlatFromAura = 0;
            int biggestBuffPercentageFromAura = 0;

            float biggestDebuffFlatFromAura = 0;
            int biggestDebuffPercentageFromAura = 0;

            biggestBuffFlatFromAura =
                FindBiggestBuffFromAuraFloat(biggestBuffFlatFromAura, ref biggestBuffPercentageFromAura, floatStatName);

            biggestDebuffFlatFromAura =
                FindBiggestDebuffFromAuraFloat(biggestDebuffFlatFromAura, ref biggestDebuffPercentageFromAura,
                    floatStatName);

            totalStatFlatBuff =
                FindTotalBuffFloat(totalStatFlatBuff, ref totalStatPercentageBuff, floatStatName);

            totalStatFlatDebuff =
                FindTotalDebuffFloat(totalStatFlatDebuff, ref totalStatPercentageDebuff, floatStatName);


            CalculateFloatStat(floatStatName, totalStatFlatBuff, totalStatPercentageBuff,
                biggestBuffFlatFromAura,
                biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff);
        }


        private void CalculateFloatStat(Stats.FloatStatName floatStatName, float totalStatFlatBuff,
            int totalStatPercentageBuff,
            float biggestBuffFlatFromAura, int biggestBuffPercentageFromAura, float biggestDebuffFlatFromAura,
            int biggestDebuffPercentageFromAura, float totalStatFlatDebuff, int totalStatPercentageDebuff)
        {
            switch (floatStatName)
            {
                case Stats.FloatStatName.AttackSpeed:
                    fireRate = startingFireRate;
                    fireRate = FloatStatFormula(fireRate, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.AttackSpeed));
                    break;
                case Stats.FloatStatName.Aoe:
                    aoe = startAOE;
                    aoe = FloatStatFormula(aoe, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.Aoe));
                    break;
                case Stats.FloatStatName.Range:
                    range = startRange;
                    range = FloatStatFormula(range, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.Range));
                    break;
                case Stats.FloatStatName.AoeProjectileSpeed:
                    linearTravelSpeed = startingLinearTravelSpeed;
                    linearTravelSpeed = FloatStatFormula(linearTravelSpeed, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.AoeProjectileSpeed));
                    break;
                case Stats.FloatStatName.AoeProjectileTravelRange:
                    linearTravelDistance = startLinearTravelDistance;
                    linearTravelDistance = FloatStatFormula(linearTravelDistance, totalStatFlatBuff,
                        totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.AoeProjectileTravelRange));
                    break;
                case Stats.FloatStatName.ChainSeekRadius:
                    chainSeekRadius = startChainSeekRadius;
                    chainSeekRadius = FloatStatFormula(chainSeekRadius, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.ChainSeekRadius));
                    break;
                case Stats.FloatStatName.AuraRadius:
                    auraRadius = startAuraRadius;
                    auraRadius = FloatStatFormula(auraRadius, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.AuraRadius));
                    break;
                case Stats.FloatStatName.DotDuration:
                    dotDuration = startDotDuration;
                    dotDuration = FloatStatFormula(dotDuration, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.DotDuration));
                    break;
                case Stats.FloatStatName.DOTTickInterval:
                    debuffTickFrequency = startDebuffTickFrequency;
                    debuffTickFrequency = FloatStatFormula(debuffTickFrequency, totalStatFlatBuff,
                        totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                        biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff,
                        findExternalBuff(Stats.FloatStatName.DOTTickInterval));
                    break;
                case Stats.FloatStatName.DOTChangingIdentifier:
                    dOTUniqueIdentifier = startingDOTUniqueIdentifier;
                    string.Concat(dOTUniqueIdentifier, totalStatFlatBuff);
                    break;
                case Stats.FloatStatName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(floatStatName), floatStatName,
                        "Float stat recalculation out of range");
            }
        }

        private int findExternalBuff(Stats.FloatStatName floatStatName)
        {
            return (int) (from stat in externalStatsBoostFloat
                where stat.floatStat == floatStatName && stat.isPercentage
                select stat.amount).FirstOrDefault();
        }

        private int findExternalBuff(Stats.IntStatName intStatName)
        {
            return (from stat in externalStatsBoostInt
                where stat.intStat == intStatName && stat.isPercentage
                select stat.amount).FirstOrDefault();
        }


        private float FloatStatFormula(float stat, float totalStatFlatBuff, int totalStatPercentageBuff,
            float biggestBuffFlatFromAura, int biggestBuffPercentageFromAura,
            float biggestDebuffFlatFromAura, int biggestDebuffPercentageFromAura, float totalStatFlatDebuff,
            int totalStatPercentageDebuff, int externalBuff = 0)
        {
            return ( //
                    ((stat + totalStatFlatBuff + biggestBuffFlatFromAura - biggestDebuffFlatFromAura -
                      totalStatFlatDebuff) / 100 *
                     (100 + totalStatPercentageBuff - totalStatPercentageDebuff)
                    ) //
                    / 100 * (100 + biggestBuffPercentageFromAura - biggestDebuffPercentageFromAura))
                / 100 * (100 + externalBuff);
        }

        private float FindTotalDebuffFloat(float totalStatFlatDebuff, ref int totalStatPercentageDebuff,
            Stats.FloatStatName floatStatName)
        {
            foreach (FloatTowerDebuff debuff in floatTowerDebuffs.Where(debuff =>
                !debuff.isFromAura && debuff.floatStat == floatStatName))
            {
                if (!debuff.isPercentage)
                {
                    if (debuff.amount > 0)
                    {
                        totalStatFlatDebuff += debuff.amount;
                    }
                }
                else
                {
                    if (debuff.amount > 0)
                    {
                        totalStatPercentageDebuff += (int) debuff.amount;
                    }
                }
            }

            return totalStatFlatDebuff;
        }

        private float FindTotalBuffFloat(float totalStatFlatBuff, ref int totalStatPercentageBuff,
            Stats.FloatStatName floatStatName)
        {
            foreach (FloatTowerBuff floatTowerBuff in floatTowerBuffs.Where(floatTowerBuff =>
                !floatTowerBuff.isFromAura && floatTowerBuff.floatStat == floatStatName))
            {
                if (!floatTowerBuff.isPercentage)
                {
                    if (floatTowerBuff.amount > 0)
                    {
                        totalStatFlatBuff += floatTowerBuff.amount;
                    }
                }
                else
                {
                    if (floatTowerBuff.amount > 0)
                    {
                        totalStatPercentageBuff += (int) floatTowerBuff.amount;
                    }
                }
            }

            return totalStatFlatBuff;
        }

        private float FindBiggestDebuffFromAuraFloat(float biggestDebuffFlatFromAura,
            ref int biggestDebuffPercentageFromAura, Stats.FloatStatName floatStatName)
        {
            foreach (FloatTowerDebuff debuff in floatTowerDebuffs.Where(debuff =>
                debuff.isFromAura && debuff.floatStat == floatStatName))
            {
                if (!debuff.isPercentage)
                {
                    if (biggestDebuffFlatFromAura < debuff.amount)
                        biggestDebuffFlatFromAura = debuff.amount;
                }
                else if (biggestDebuffPercentageFromAura < debuff.amount)
                    biggestDebuffPercentageFromAura = (int) debuff.amount;
            }

            return biggestDebuffFlatFromAura;
        }

        private float FindBiggestBuffFromAuraFloat(float biggestBuffFlatFromAura, ref int biggestBuffPercentageFromAura,
            Stats.FloatStatName floatStatName)
        {
            foreach (FloatTowerBuff buffInstance in floatTowerBuffs.Where(buffInstance =>
                buffInstance.isFromAura && buffInstance.floatStat == floatStatName))
            {
                if (!buffInstance.isPercentage)
                {
                    if (biggestBuffFlatFromAura < buffInstance.amount)
                        biggestBuffFlatFromAura = buffInstance.amount;
                }
                else if (biggestBuffPercentageFromAura < buffInstance.amount)
                    biggestBuffPercentageFromAura = (int) buffInstance.amount;
            }

            return biggestBuffFlatFromAura;
        }

        public void IntegerStatRecalculate(Stats.IntStatName intStatName)
        {
            int totalStatFlatBuff = 0;
            int totalStatPercentageBuff = 0;

            int totalStatFlatDebuff = 0;
            int totalStatPercentageDebuff = 0;

            int biggestBuffFlatFromAura = 0;
            int biggestBuffPercentageFromAura = 0;

            int biggestDebuffFlatFromAura = 0;
            int biggestDebuffPercentageFromAura = 0;


            biggestBuffFlatFromAura =
                FindBiggestBuffFromAuraInt(biggestBuffFlatFromAura, ref biggestBuffPercentageFromAura, intStatName);

            biggestDebuffFlatFromAura =
                FindBiggestDebuffFromAuraInt(biggestDebuffFlatFromAura, ref biggestDebuffPercentageFromAura,
                    intStatName);

            totalStatFlatBuff =
                FindTotalBuffInt(totalStatFlatBuff, ref totalStatPercentageBuff, intStatName);


            totalStatFlatDebuff = FindTotalDebuffInt(totalStatFlatDebuff, ref totalStatPercentageDebuff, intStatName);


            CalculateIntStat(intStatName, totalStatFlatBuff, totalStatPercentageBuff,
                biggestBuffFlatFromAura,
                biggestBuffPercentageFromAura, biggestDebuffFlatFromAura,
                biggestDebuffPercentageFromAura, totalStatFlatDebuff, totalStatPercentageDebuff);
        }

        private void CalculateIntStat(Stats.IntStatName intStatName, int totalStatFlatBuff,
            int totalStatPercentageBuff, int biggestBuffFlatFromAura, int biggestBuffPercentageFromAura,
            int biggestDebuffFlatFromAura, int biggestDebuffPercentageFromAura, int totalStatFlatDebuff,
            int totalStatPercentageDebuff)
        {
            //(damage + totalStatFlatBuffValue + biggestBuffFlatFromAura + //- biggestDebuffFromAura - totalDebuffFlat DO NOT USE
            switch (intStatName)
            {
                case Stats.IntStatName.Damage:
                    damage = startDamage;
                    damage = IntStatFormula(damage, totalStatFlatBuff, totalStatPercentageBuff, biggestBuffFlatFromAura,
                        biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.Damage));
                    break;
                case Stats.IntStatName.PenetrateAmount:
                    penetration = startPenetration;
                    penetration = IntStatFormula(penetration, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.PenetrateAmount));
                    break;
                case Stats.IntStatName.ChainLength:
                    chainLength = startChainLength;
                    chainLength = IntStatFormula(chainLength, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.ChainLength));
                    break;
                case Stats.IntStatName.SplitshotTargetsCount:
                    splitshotTargetCount = startSplitshotTargetCount;
                    splitshotTargetCount = IntStatFormula(splitshotTargetCount, totalStatFlatBuff,
                        totalStatPercentageBuff, biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.SplitshotTargetsCount));
                    break;
                case Stats.IntStatName.SlowingAuraPercentage:
                    slowingAuraPercentage = startSlowingAuraPercentage;
                    slowingAuraPercentage = IntStatFormula(slowingAuraPercentage, totalStatFlatBuff,
                        totalStatPercentageBuff, biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.SlowingAuraPercentage));
                    break;
                case Stats.IntStatName.SnaringStrength:
                    snaringStrength = startSnaringStrength;
                    snaringStrength = IntStatFormula(snaringStrength, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.SnaringStrength));
                    break;
                case Stats.IntStatName.DOTTickDamage:
                    dOTDamage = startDOTDamage;
                    dOTDamage = IntStatFormula(dOTDamage, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.DOTTickDamage));
                    break;
                case Stats.IntStatName.DOTSlowPercentage:
                    dotSlowRate = startDotSlowRate;
                    dotSlowRate = IntStatFormula(dotSlowRate, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.DOTSlowPercentage));
                    break;
                case Stats.IntStatName.DOTPenetrativeDamage:
                    dOTPenetration = startingDOTPenetration;
                    dOTPenetration = IntStatFormula(dOTPenetration, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.DOTPenetrativeDamage));
                    break;

                case Stats.IntStatName.Disruption:
                    disruption = startDisruption;
                    disruption = IntStatFormula(disruption, totalStatFlatBuff, totalStatPercentageBuff,
                        biggestBuffFlatFromAura, biggestBuffPercentageFromAura,
                        biggestDebuffFlatFromAura, biggestDebuffPercentageFromAura, totalStatFlatDebuff,
                        totalStatPercentageDebuff, findExternalBuff(Stats.IntStatName.Disruption));
                    break;
                case Stats.IntStatName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(intStatName), intStatName,
                        "Int stat recalculate out of range");
            }
        }

        private static int IntStatFormula(int stat, int totalStatFlatBuff, int totalStatPercentageBuff,
            int biggestBuffFlatFromAura, int biggestBuffPercentageFromAura,
            int biggestDebuffFlatFromAura, int biggestDebuffPercentageFromAura, int totalStatFlatDebuff,
            int totalStatPercentageDebuff, int externalBuff = 0)
        {
            return (int) Math.Round((int) Math.Round( //
                    (stat + totalStatFlatBuff + biggestBuffFlatFromAura - biggestDebuffFlatFromAura -
                     totalStatFlatDebuff) / 100.0f *
                    (100.0f + totalStatPercentageBuff - totalStatPercentageDebuff)
                    //
                    / 100.0f * (100.0f + biggestBuffPercentageFromAura - biggestDebuffPercentageFromAura))
                / 100.0f * (100.0f + externalBuff));
        }

        private int FindTotalBuffInt(int statFlatBuffValue, ref int statPercentageBuffValue,
            Stats.IntStatName intStatName)
        {
            foreach (IntegerTowerBuff integerTowerBuff in integerTowerBuffs.Where(integerTowerBuff =>
                !integerTowerBuff.isFromAura && integerTowerBuff.intStat == intStatName))
            {
                if (!integerTowerBuff.isPercentage)
                {
                    if (integerTowerBuff.amount > 0)
                    {
                        statFlatBuffValue += integerTowerBuff.amount;
                    }
                }
                else
                {
                    if (integerTowerBuff.amount > 0)
                    {
                        statPercentageBuffValue += integerTowerBuff.amount;
                    }
                }
            }

            return statFlatBuffValue;
        }

        private int FindTotalDebuffInt(int statFlatDebuffValue, ref int statPercentageDebuffValue,
            Stats.IntStatName intStatName)
        {
            foreach (IntegerTowerDebuff integerTowerDebuff in integerTowerDebuffs.Where(integerTowerDebuff =>
                !integerTowerDebuff.isFromAura && integerTowerDebuff.intStat == intStatName))
            {
                if (!integerTowerDebuff.isPercentage)
                {
                    if (integerTowerDebuff.amount > 0)
                    {
                        statFlatDebuffValue += integerTowerDebuff.amount;
                    }
                }
                else
                {
                    if (integerTowerDebuff.amount > 0)
                    {
                        statPercentageDebuffValue += integerTowerDebuff.amount;
                    }
                }
            }

            return statFlatDebuffValue;
        }

        private int FindBiggestBuffFromAuraInt(int biggestBuffFlatFromAura,
            ref int biggestBuffPercentageFromAura, Stats.IntStatName intStatName)
        {
            // if (biggestBuffFlatFromAura <= 0) throw new ArgumentOutOfRangeException(nameof(biggestBuffFlatFromAura));
            foreach (IntegerTowerBuff buffInstance in integerTowerBuffs.Where(buffInstance =>
                buffInstance.isFromAura && buffInstance.intStat == intStatName))
            {
                if (!buffInstance.isPercentage)
                {
                    if (biggestBuffFlatFromAura < buffInstance.amount)
                        biggestBuffFlatFromAura = buffInstance.amount;
                }
                else if (biggestBuffPercentageFromAura < buffInstance.amount)
                    biggestBuffPercentageFromAura = buffInstance.amount;
            }

            return biggestBuffFlatFromAura;
        }


        private int FindBiggestDebuffFromAuraInt(int biggestDebuffFlatFromAura,
            ref int biggestDebuffPercentageFromAura, Stats.IntStatName intStatName)
        {
            foreach (IntegerTowerDebuff integerTowerDebuff in integerTowerDebuffs.Where(integerTowerDebuff =>
                integerTowerDebuff.isFromAura && integerTowerDebuff.intStat == intStatName))
            {
                if (!integerTowerDebuff.isPercentage)
                {
                    if (biggestDebuffFlatFromAura < integerTowerDebuff.amount)
                        biggestDebuffFlatFromAura = integerTowerDebuff.amount;
                }
                else if (biggestDebuffPercentageFromAura < integerTowerDebuff.amount)
                    biggestDebuffPercentageFromAura = integerTowerDebuff.amount;
            }

            return biggestDebuffFlatFromAura;
        }
        

        public void InvokeRoundAttacking()
        {
            CancelInvoke();
            InvokeRepeating(nameof(UpdateTarget), 0f,
                (1f / PlayerStats.instance.gameSpeedMultiplier) /
                (notAffectedByAttackSpeedAuras ? startingFireRate : fireRate));

            // Debug.Log($"invoking attack. Repeatrate = {(1f / PlayerStats.instance.gameSpeedMultiplier) / (notAffectedByAttackSpeedAuras ? startingFireRate : fireRate)}");
        }

        private static void InitializeEffect(FalseCrit falseCrit)
        {
            falseCrit.trueCounter = falseCrit.attackCounter;
        }

        
        


        private void OnTriggerEnter2D(Collider2D other)
        {
           
            if (!isAura && !isEnemySuckGain) return;
           
            if (!isAffectingTowers)
            {
                if (other is CircleCollider2D) return;

                if (!other.CompareTag(enemyTag)) return;

                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                if (isSnaring)
                {
                    ApplySnare(enemy);
                }

                if (isSlowing)
                {
                    ApplySlow(enemy);
                }

                if (isDOT)
                {
                    CheckAndApplyDot(enemy);
                }

                if (isEnemySuckGain)
                {
                    AddEssenceInstance(enemy);
                }

                Utils.ApplyColliderEntry(enemy,
                    isDOT
                        ? new ColliderContentTicket(id, new DamageDotInstance(debuffTickFrequency, dOTDamage,
                            dotDuration,
                            id, dOTMagical, dOTPenetration, dOTUniqueIdentifier, isStackable, updatesCooldown, false,
                            isDOTSlowing, dotSlowRate))
                        : new ColliderContentTicket(id, null));
            }
            else
            {
                //Debug.Log("blurp");
                if (!other.CompareTag(MyTag)) return;

                TowerAI towerAi = other.gameObject.GetComponent<TowerAI>();

                if (isAttackSpeedBuffAura)
                {
                    ApplyTowerAttackSpeedBuff(towerAi);
                }

                if (isAttackBuffAura)
                {
                    ApplyTowerDamageBuff(towerAi);
                }

                if (isRadiusBuff)
                {
                    ApplyRadiusBuff(towerAi);
                }
            }
        }

        private void AddEssenceInstance(Enemy enemy)
        {
            enemy.essenceAOEInstances.Add(new EssenceAOEInstance(GetInstanceID(), this));
        }

        public bool GainEssences(int amount)
        {
            if (!isEnemySuckGain) return false;
            if (isFullOfEssence) return false;

            suckAmount += amount;

            if (suckAmount > maxSuckAmount)
            {
                isFullOfEssence = true;
                suckAmount = maxSuckAmount;
                return true;
            } 
            
            return true;
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isAura && !isEnemySuckGain) return;

            if (!isAffectingTowers)
            {
                if (other is CircleCollider2D) return;


                if (!other.CompareTag(enemyTag)) return;

                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (isSlowing)
                {
                    RemoveSlowInstance(enemy);
                }

                if (isSnaring)
                {
                    RemoveSnareInstance(enemy);
                }

                /*if (isDOT)
                {
                    RemoveDOTInstance(enemy);
                }*/

                if (isEnemySuckGain)
                {
                    RemoveEssenceInstance(enemy);
                }

                Utils.DeleteColliderEntry(enemy, id);
            }
            else
            {
                if (!other.CompareTag(MyTag)) return;

                TowerAI towerAi = other.gameObject.GetComponent<TowerAI>();

                if (isAttackBuffAura)
                {
                    RemoveBuff(id, Stats.IntStatName.Damage);
                }

                if (isAttackSpeedBuffAura)
                {
                    RemoveBuff(id, Stats.FloatStatName.AttackSpeed);
                }

                if (isRadiusBuff)
                {
                    RemoveBuff(id, Stats.FloatStatName.Range);
                }
                
                
            }
        }

        private void RemoveEssenceInstance(Enemy enemy)
        {
            for (int i = 0; i < enemy.essenceAOEInstances.Count; i++)
            {
                if (enemy.essenceAOEInstances[i].instanceID == GetInstanceID())
                {
                    enemy.essenceAOEInstances.RemoveAt(i);
                }
            }
        }
        
        public void RemoveBuff(int id, Stats.FloatStatName floatStatName)
        {
            for (int i = 0; i < floatTowerBuffs.Count; i++)
            {
                if (floatTowerBuffs[i].instanceId != id) continue;

                floatTowerBuffs.Remove(floatTowerBuffs[i]);
            }

            FloatStatRecalculate(floatStatName);;
        }
        
        public void RemoveBuff(int id, Stats.IntStatName intStatName)
        {
            for (int i = 0; i < integerTowerBuffs.Count; i++)
            {
                if (integerTowerBuffs[i].instanceId != id) continue;

                integerTowerBuffs.Remove(integerTowerBuffs[i]);
            }

            IntegerStatRecalculate(intStatName);;
        }
        
        

        private void ApplyRadiusBuff(TowerAI towerAi)
        {
            
            if (radiusBuffPercentage != 0)
                towerAi.floatTowerBuffs.Add(new FloatTowerBuff(id, false, true, true, Stats.FloatStatName.Range, radiusBuffPercentage ));
            
            if (radiusBuffFlat != 0)
                towerAi.floatTowerBuffs.Add(new FloatTowerBuff(id, false, false, true, Stats.FloatStatName.Range, radiusBuffFlat ));

            towerAi.FloatStatRecalculate(Stats.FloatStatName.Range);
        }

        private void ApplyTowerDamageBuff(TowerAI towerAi)
        {
            if (auraAffectsOnlyPen == false && auraAffectsOnlyDisruption == false )
            {
                AuraBuffDamage(towerAi);

                if (auraAffectsPenDamage)
                {
                    AuraBuffPenetration(towerAi);
                }
                
                if (auraAffectsDisruptionDamage)
                {
                    AuraBuffDisruption(towerAi);
                }
                
            }
            else
            {
                if (auraAffectsOnlyPen)
                {
                    AuraBuffPenetration(towerAi);
                }
                
                if (auraAffectsOnlyDisruption)
                {
                    AuraBuffDisruption(towerAi);
                }
            }
        }

        private void AuraBuffDamage(TowerAI towerAi)
        {
            if (percentageAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, true, true, Stats.IntStatName.Damage,
                    percentageAttackBuff));

            if (flatAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, false, true, Stats.IntStatName.Damage,
                    flatAttackBuff));
            towerAi.IntegerStatRecalculate(Stats.IntStatName.Damage);
        }

        private void AuraBuffPenetration(TowerAI towerAi)
        {
            if (percentageAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, true, true, Stats.IntStatName.PenetrateAmount,
                    percentageAttackBuff));

            if (flatAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, false, true, Stats.IntStatName.PenetrateAmount,
                    flatAttackBuff));
            towerAi.IntegerStatRecalculate(Stats.IntStatName.PenetrateAmount);
        }

        private void AuraBuffDisruption(TowerAI towerAi)
        {
            if (percentageAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, true, true, Stats.IntStatName.Disruption,
                    percentageAttackBuff));

            if (flatAttackBuff != 0)
                towerAi.integerTowerBuffs.Add(new IntegerTowerBuff(id, false, false, true, Stats.IntStatName.Disruption,
                    flatAttackBuff));
            towerAi.IntegerStatRecalculate(Stats.IntStatName.Disruption);
        }

        private void ApplyTowerAttackSpeedBuff(TowerAI towerAi)
        {

            if (percentageAttackSpeedBuff != 0)
                towerAi.floatTowerBuffs.Add(new FloatTowerBuff(GetInstanceID(), false, true, true,
                    Stats.FloatStatName.AttackSpeed, percentageAttackSpeedBuff));

            if (flatAttackSpeedBuff != 0)
            {
                towerAi.floatTowerBuffs.Add(new FloatTowerBuff(GetInstanceID(), false, false, true,
                    Stats.FloatStatName.AttackSpeed, percentageAttackSpeedBuff));
            }
            
            towerAi.FloatStatRecalculate(Stats.FloatStatName.AttackSpeed);
        }


        /*private void OnTriggerStay2D(Collider2D other)
        {
            if (other is CircleCollider2D) return;
            if (!isAura) return;

            if (!other.CompareTag(enemyTag)) return;

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            CheckAndApplyDot(enemy);
        }*/

        private void CheckAndApplyDot(Enemy enemy)
        {
            bool shouldReturn = false;
            foreach (DamageDotInstance enemyDamageDotInstance in enemy.damageDotInstances.Where(
                enemyDamageDotInstance => enemyDamageDotInstance.instanceId == id))
            {
                //Debug.Log($"{enemyDamageDotInstance.instanceId} vs {id}");
                enemyDamageDotInstance.dotDuration = dotDuration;
                if (isStackable)
                {
                    enemyDamageDotInstance.stacks++;
                    Debug.Log("omegaLuul");
                }

                shouldReturn = true;
            }

            if (shouldReturn) return;

            ApplyDamageDot(enemy);
        }

        private void ApplyDamageDot(Enemy enemy)
        {
            bool isAlreadyOn = false;
            foreach (DamageDotInstance enemyDamageDotInstance in
                enemy.damageDotInstances.Where(enemyDamageDotInstance =>
                    enemyDamageDotInstance.uniqueIdentifier == dOTUniqueIdentifier))
            {
                //Debug.Log($"{dOTUniqueIdentifier} and {enemyDamageDotInstance.uniqueIdentifier}");
                isAlreadyOn = true;
                if (dotDuration > enemyDamageDotInstance.dotDuration)
                    enemyDamageDotInstance.dotDuration = dotDuration;
            }

            if (isAlreadyOn) return;

            //Debug.Log("Inst add");
            enemy.AddDotInstance(new DamageDotInstance(debuffTickFrequency, dOTDamage, dotDuration,
                id, dOTMagical, dOTPenetration, dOTUniqueIdentifier, isStackable, updatesCooldown,
                false, isDOTSlowing, dotSlowRate));
        }

        private void ApplySnare(Enemy enemy)
        {
            List<SnareInstance> snareInstances = enemy.snareInstances;
            snareInstances.Add(new SnareInstance(id, snaringStrength));

            enemy.ResetFlight();
            enemy.ResetMVSP();
        }

        private void ApplySlow(Enemy enemy)
        {
            List<SlowInstance> slowInstances = enemy.slowInstances;

            int biggestSlowInstance = slowingAuraPercentage;

            if (slowInstances.Count > 0)
                foreach (SlowInstance slowInstance in slowInstances.Where(slowInstance =>
                    slowInstance.slowAmount > slowingAuraPercentage))
                {
                    biggestSlowInstance = slowInstance.slowAmount;
                }

            slowInstances.Add(new SlowInstance(id, slowingAuraPercentage));

            if (biggestSlowInstance > slowingAuraPercentage) return;

            enemy.slowAmountPercentage = slowingAuraPercentage;
            enemy.ResetMVSP();
        }


        private void RemoveDOTInstance(Enemy enemy)
        {
            List<DamageDotInstance> damageDotInstances = enemy.damageDotInstances;

            for (int i = 0; i < damageDotInstances.Count; i++)
            {
                if (damageDotInstances[i].instanceId != id) continue;

                damageDotInstances.Remove(damageDotInstances[i]);
            }
        }

        private void RemoveSnareInstance(Enemy enemy)
        {
            List<SnareInstance> snareInstances = enemy.snareInstances;

            for (int i = 0; i < snareInstances.Count; i++)
            {
                if (snareInstances[i].instanceId != id) continue;

                snareInstances.Remove(snareInstances[i]);
                enemy.ResetFlight();
                enemy.ResetMVSP();
            }
        }

        private void RemoveSlowInstance(Enemy enemy)
        {
            List<SlowInstance> slowInstances = enemy.slowInstances;

            for (int i = 0; i < slowInstances.Count; i++)
            {
                if (slowInstances[i].instanceId != id) continue;

                slowInstances.Remove(slowInstances[i]);
                enemy.ResetMVSP();
            }

            if (slowInstances.Count > 0)
            {
                int biggestSlowInstance = slowInstances.Select(slowInstance => slowInstance.slowAmount)
                    .Concat(new[] {0}).Max();

                enemy.slowAmountPercentage = biggestSlowInstance;
                enemy.ResetMVSP();
            }
            else
            {
                enemy.slowAmountPercentage = 0;
                enemy.ResetMVSP();
            }
        }


        private void SpawnGroundPoints()
        {
            List<Vector3> spots = FindSpots();

            if (spots.IsNullOrEmpty()) return;

            foreach (GameObject go in spots.Select(spot => (GameObject) Instantiate(bulletPrefab,
                spot,
                Quaternion.identity)))
            {
                groundProjectiles.Add(go.GetComponent<GroundProjectile>());
            }


            foreach (GroundProjectile groundProjectile in groundProjectiles)
            {
                groundProjectile.damage = startDamage;
                groundProjectile.penetration = penetration;
                groundProjectile.isMagical = isMagical;
                AssignDOTs(groundProjectile);
            }
        }

        public List<Vector3> FindSpots()
        {
            List<Vector3> spotList = new List<Vector3>();

            Vector2 pos = transform.position;
            float x = pos.x;
            float y = pos.y;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(x + SpawnDistance, y), 0.5f);
            bool caseBreakFlag = false;

            List<Rail> rails = new List<Rail>();


            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        colliders = Physics2D.OverlapCircleAll(new Vector2(x + SpawnDistance, y), 0.05f);

                        foreach (Collider2D t in colliders)
                        {
                            if (t.CompareTag(nodeTag))
                            {
                                caseBreakFlag = true;
                            }

                            if (t.CompareTag(Rail.myTag))
                            {
                                rails.Add(t.gameObject.GetComponent<Rail>());
                            }
                        }

                        if (caseBreakFlag)
                        {
                            caseBreakFlag = false;
                            break;
                        }

                        spotList.Add(new Vector3(x + SpawnDistance, y, transform.position.z));


                        break;
                    case 1:
                        colliders = Physics2D.OverlapCircleAll(new Vector2(x - SpawnDistance, y), 0.05f);

                        foreach (Collider2D t in colliders)
                        {
                            if (t.CompareTag(nodeTag))
                            {
                                caseBreakFlag = true;
                            }

                            if (t.CompareTag(Rail.myTag))
                            {
                                rails.Add(t.gameObject.GetComponent<Rail>());
                            }
                        }

                        if (caseBreakFlag)
                        {
                            caseBreakFlag = false;
                            break;
                        }


                        spotList.Add(new Vector3(x - SpawnDistance, y, transform.position.z));


                        break;
                    case 2:
                        colliders = Physics2D.OverlapCircleAll(new Vector2(x, y + SpawnDistance), 0.05f);

                        foreach (Collider2D t in colliders)
                        {
                            if (t.CompareTag(nodeTag))
                            {
                                caseBreakFlag = true;
                            }

                            if (t.CompareTag(Rail.myTag))
                            {
                                rails.Add(t.gameObject.GetComponent<Rail>());
                            }
                        }

                        if (caseBreakFlag)
                        {
                            caseBreakFlag = false;
                            break;
                        }

                        spotList.Add(new Vector3(x, y + SpawnDistance, transform.position.z));

                        break;
                    case 3:
                        colliders = Physics2D.OverlapCircleAll(new Vector2(x, y - SpawnDistance), 0.05f);

                        foreach (Collider2D t in colliders)
                        {
                            if (t.CompareTag(nodeTag))
                            {
                                caseBreakFlag = true;
                            }

                            if (t.CompareTag(Rail.myTag))
                            {
                                rails.Add(t.gameObject.GetComponent<Rail>());
                            }
                        }

                        if (caseBreakFlag)
                        {
                            caseBreakFlag = false;
                            break;
                        }


                        spotList.Add(new Vector3(x, y - SpawnDistance, transform.position.z));

                        break;
                }
            }

            railListForMines = rails;
            return spotList;
        }

        private void Update()
        {
            if (isRoundAttacking) return;

            if (!isGroundType)
            {
                fireCountdown -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

                if (!isSplitshot)
                {
                    if (target == null)
                    {
                        return;
                    }
                }
                else if (splitshotEnemyTargets.IsNullOrEmpty()) return;


                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / (notAffectedByAttackSpeedAuras ? startingFireRate : fireRate);
                }
            }
        }

        private void SingleTargetOneShotEnemyUpdateTarget()
        {
            List<GameObject> enemies = FindEnemies();

            int estimatedHPClosestToZero = int.MaxValue;
            Transform maxPriorityEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                Enemy enemyAI = enemy.GetComponent<Enemy>();

                int enemyHealth = enemyAI.health + enemyAI.shield;
                int enemyArmor = enemyAI.currentArmor;

                int estimatedPenetrationDamage = 0;

                int armorMinusPenetration = enemyArmor - penetration;

                if (armorMinusPenetration < 0)
                {
                    estimatedPenetrationDamage = armorMinusPenetration * 2;
                }
                else
                {
                    estimatedPenetrationDamage = armorMinusPenetration;
                }

                int estimatedHPLeft = enemyHealth - (damage + estimatedPenetrationDamage);


                if (Math.Abs(estimatedHPClosestToZero) > Math.Abs(estimatedHPLeft))
                {
                    estimatedHPClosestToZero = estimatedHPLeft;
                    maxPriorityEnemy = enemy.transform;
                }
                else if (Math.Abs(estimatedHPLeft) > Math.Abs(estimatedHPClosestToZero))
                {
                }
                else
                {
                    if (estimatedHPLeft >= estimatedHPClosestToZero) continue;

                    estimatedHPClosestToZero = estimatedHPLeft;
                    maxPriorityEnemy = enemy.transform;
                }
            }

            target = maxPriorityEnemy;
        }

        private List<GameObject> FindEnemies()
        {
            Collider2D[] collides = Physics2D.OverlapCircleAll(transform.position, range);

            List<Collider2D> fixedCollides = Utils.RemoveEnemyOverlapRepetitions(collides);
            //Debug.Log($"colliders found {collides.Length}");

            List<GameObject> enemies = (from collision in fixedCollides
                where collision.CompareTag(Enemy.MyTag)
                select collision.gameObject).ToList();
            return enemies;
        }

        private void UpdateTarget()
        {
            // Debug.Log("boom");
            if (oneShotTargetFist)
            {
                SingleTargetOneShotEnemyUpdateTarget();
            }

            else
            {
                NotOneshotTargetSearch();
            }
        }

        private void NotOneshotTargetSearch()
        {
            List<GameObject> enemies = FindEnemies();

            //Debug.Log(enemies.Count);

            if (isRoundAttacking)
            {
                foreach (GameObject enemy in enemies)
                {
                    SpawnBulletForEnemy(enemy.GetComponent<Enemy>());
                    //Debug.Log("spawning bullet");
                }

                return;
            }

            if (target != null)
                if (Vector2.Distance(transform.position, target.transform.position) > range)
                {
                    target = null;
                }

            float shortestDistance = Mathf.Infinity;
            float shortestDistanceToPriority = Mathf.Infinity;

            GameObject nearestEnemy = null;
            GameObject nearestEnemyToPriority = null;

            if (!isSplitshot)
            {
                bool priorityTargetInside = false;

                foreach (GameObject enemy in enemies.Where(enemy =>
                    priorityTargetInside == false && enemy.GetComponent<Enemy>().type == targetPriority))
                {
                    priorityTargetInside = true;
                }

                foreach (GameObject enemyObj in enemies)
                {
                    if (enemyObj == null) continue;

                    Vector3 enemyPosition = enemyObj.transform.position;
                    Enemy enemy = enemyObj.GetComponent<Enemy>();

                    float distanceToEnemyPriority = Mathf.Infinity;

                    if (enemy.target != null)
                        distanceToEnemyPriority = Vector2.Distance(enemy.target.position, enemyPosition);


                    float distanceToEnemyFromTower = Vector2.Distance(transform.position, enemyPosition);

                    for (int i = enemy.waypointIndex; i < Waypoints.points.Length - 1; i++)
                    {
                        distanceToEnemyPriority += Vector2.Distance(Waypoints.points[i].position,
                            Waypoints.points[i + 1].position);
                    }

                    if (priorityTargetInside)
                    {
                        if (distanceToEnemyFromTower < shortestDistance
                            && enemy.type == targetPriority)
                        {
                            shortestDistance = distanceToEnemyFromTower;
                            nearestEnemy = enemyObj;
                        }
                    }
                    else if (distanceToEnemyFromTower < shortestDistance)
                    {
                        shortestDistance = distanceToEnemyFromTower;
                        nearestEnemy = enemyObj;
                    }

                    if (priorityTargetInside)
                    {
                        if (distanceToEnemyPriority < shortestDistanceToPriority
                            && enemy.type == targetPriority)
                        {
                            shortestDistanceToPriority = distanceToEnemyPriority;
                            nearestEnemyToPriority = enemyObj;
                        }
                    }
                    else if (distanceToEnemyPriority < shortestDistanceToPriority)
                    {
                        shortestDistanceToPriority = distanceToEnemyPriority;
                        nearestEnemyToPriority = enemyObj;
                    }
                }

                if (nearestEnemy != null && shortestDistance <= range)
                {
                    if (isHoldingTarget)
                    {
                        if (target == null)
                        {
                            ChooseEnemyBasedByPriority(nearestEnemy, nearestEnemyToPriority);
                        }
                    }
                    else
                    {
                        ChooseEnemyBasedByPriority(nearestEnemy, nearestEnemyToPriority);
                    }
                }
                else
                {
                    target = null;
                }
            }
            else
            {
                SplitShotUpdate(enemies, shortestDistance);
            }
        }

        private void ChooseEnemyBasedByPriority(GameObject nearestEnemy, GameObject nearestEnemyToPriority)
        {
            if (shootingMode == ShootingMode.Closest)
            {
                target = nearestEnemy.transform;
                enemyTarget = nearestEnemy.GetComponent<Enemy>();
            }
            else
            {
                target = nearestEnemyToPriority.transform;
                enemyTarget = nearestEnemyToPriority.GetComponent<Enemy>();
            }
        }

        private void SplitShotUpdate(List<GameObject> enemies, float shortestDistance)
        {
            List<RangeEnemyPair> rangeList = new List<RangeEnemyPair>();
            if (!isHoldingTarget)
            {
                splitshotEnemyTargets.Clear();
                splitshotTargets.Clear();
            }


            //clear dead
            for (int i = 0; i < splitshotEnemyTargets.Count; i++)
            {
                if (splitshotEnemyTargets[i] == null)
                {
                    splitshotEnemyTargets.Remove(splitshotEnemyTargets[i]);
                }
            }


            //find ranges & clear out of range
            foreach (GameObject enemy in enemies)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (!splitshotEnemyTargets.Contains(enemyScript))
                {
                    Vector2 enemyPosition = enemy.transform.position;

                    float distanceToEnemyFromTower = Vector2.Distance(transform.position, enemyPosition);
                    float distanceToEnemyPriority = Vector2.Distance(enemyScript.target.position, enemyPosition);

                    for (int i = enemyScript.waypointIndex; i < Waypoints.points.Length - 1; i++)
                    {
                        distanceToEnemyPriority += Vector2.Distance(Waypoints.points[i].position,
                            Waypoints.points[i + 1].position);
                    }


                    rangeList.Add(new RangeEnemyPair(distanceToEnemyFromTower,
                        enemyScript,
                        distanceToEnemyPriority));
                }
                else
                {
                    //clear out of range
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy > range)
                    {
                        splitshotEnemyTargets.Remove(enemy.GetComponent<Enemy>());
                        splitshotTargets.Remove(enemy.transform);
                    }
                }
            }

            if (rangeList.Count == splitshotEnemyTargets.Count) return;

            targetPriorityCount = 0;
            List<Enemy> enemyPriorityFilterList = new List<Enemy>();

            foreach (RangeEnemyPair rangeEnemyPair in rangeList.Where(rangeEnemyPair =>
                rangeEnemyPair.enemy.type == targetPriority
                && !enemyPriorityFilterList.Contains(rangeEnemyPair.enemy)))
            {
                targetPriorityCount++;
                enemyPriorityFilterList.Add(rangeEnemyPair.enemy);
            }

            int cycleCount = splitshotTargetCount - splitshotEnemyTargets.Count;

            if (splitshotEnemyTargets.Count < splitshotTargetCount)
            {
                for (int i = 0; i < cycleCount; i++)
                {
                    Enemy nearestEnemy = null;
                    shortestDistance = Mathf.Infinity;

                    if (shootingMode == ShootingMode.Closest)
                    {
                        if (targetPriorityCount > 0)
                        {
                            foreach (RangeEnemyPair rangeEnemyPair in rangeList.Where(rangeEnemyPair =>
                                rangeEnemyPair.distanceToEnemy < shortestDistance &&
                                !splitshotEnemyTargets.Contains(rangeEnemyPair.enemy)
                                && rangeEnemyPair.enemy.type == targetPriority))
                            {
                                shortestDistance = rangeEnemyPair.distanceToEnemy;
                                nearestEnemy = rangeEnemyPair.enemy;
                            }
                        }
                        else
                            foreach (RangeEnemyPair rangeEnemyPair in rangeList.Where(rangeEnemyPair =>
                                rangeEnemyPair.distanceToEnemy < shortestDistance &&
                                !splitshotEnemyTargets.Contains(rangeEnemyPair.enemy)))
                            {
                                shortestDistance = rangeEnemyPair.distanceToEnemy;
                                nearestEnemy = rangeEnemyPair.enemy;
                            }
                    }
                    else
                    {
                        if (targetPriorityCount > 0)
                        {
                            foreach (RangeEnemyPair rangeEnemyPair in rangeList.Where(rangeEnemyPair =>
                                rangeEnemyPair.enemyDistanceToPriority < shortestDistance &&
                                !splitshotEnemyTargets.Contains(rangeEnemyPair.enemy)
                                && rangeEnemyPair.enemy.type == targetPriority))
                            {
                                shortestDistance = rangeEnemyPair.enemyDistanceToPriority;
                                nearestEnemy = rangeEnemyPair.enemy;
                            }
                        }
                        else
                            foreach (RangeEnemyPair rangeEnemyPair in rangeList.Where(rangeEnemyPair =>
                                rangeEnemyPair.enemyDistanceToPriority < shortestDistance &&
                                !splitshotEnemyTargets.Contains(rangeEnemyPair.enemy)))
                            {
                                shortestDistance = rangeEnemyPair.enemyDistanceToPriority;
                                nearestEnemy = rangeEnemyPair.enemy;
                            }
                    }

                    if (nearestEnemy != null)
                    {
                        splitshotEnemyTargets.Add(nearestEnemy);
                        if (nearestEnemy.type == targetPriority) targetPriorityCount--;
                    }
                }
            }
        }

        private void Shoot()
        {
            CountEffects();

            if (!isSplitshot)
            {
                SpawnBulletForEnemy(target.GetComponent<Enemy>());
            }
            else
            {
                foreach (Enemy enemy in splitshotEnemyTargets)
                {
                    SpawnBulletForEnemy(enemy);
                }
            }

            CheckEffectsForZeroes();
        }

        private void CheckEffectsForZeroes()
        {
            foreach (FalseCrit falseCrit in falseCrits.Where(falseCrit => falseCrit.trueCounter == 0))
            {
                falseCrit.trueCounter = falseCrit.attackCounter;
            }
        }

        private void CountEffects()
        {
            if (falseCrits.IsNullOrEmpty()) return;

            foreach (FalseCrit falseCrit in falseCrits)
            {
                falseCrit.trueCounter--;
                //Debug.Log(falseCrit.trueCounter);
            }
        }

        private void SpawnBulletForEnemy(Enemy enemy)
        {
            if (shootEffect != null)
            {
                GameObject effect = Instantiate(shootEffect, firePoint.position, firePoint.rotation);
                Destroy(effect, 3);
            }
            GameObject bulletGO =
                (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            BulletAI bullet = bulletGO.GetComponent<BulletAI>();

            AssignMainStats(bullet);

            AssignEffects(bullet);

            bullet.targetEnemy = enemy;
            if (isLinearAOE)
            {
                AssignLinearAOE(bullet);
            }

            if (isPenetrative)
            {
                AssignPenetration(bullet);
            }

            if (bullet != null && enemy != null)
                bullet.Seek(enemy.transform);


            if (isChainLighting)
            {
                if (enemy != null)
                {
                    bullet.target = enemy.transform;
                }

                AssignChain(bullet);
            }

            AssignDOTs(bullet);

            if (isRay)
            {
                AssignRay(bullet, enemy);
            }
        }

        private void AssignEffects(BulletAI bullet)
        {
            if (falseCrits.IsNullOrEmpty()) return;

            foreach (FalseCrit falseCrit in falseCrits.Where(falseCrit => falseCrit.trueCounter == 0))
            {
                //Debug.Log($"assigning {falseCrit.effect.name} effect");
                bullet.effects.Add(falseCrit.effect);
            }
        }


        private void AssignChain(Projectile bullet)
        {
            bullet.isChainLightning = isChainLighting;
            bullet.chainLength = startChainLength;
            bullet.chainTargetRange = chainSeekRadius;
        }

        private void AssignPenetration(Projectile bullet)
        {
            bullet.isPenetrative = isPenetrative;
            bullet.penetration = notAffectedByAttackAuras ? startPenetration : penetration;
        }

        private void AssignLinearAOE(Projectile bullet)
        {
            bullet.isLinearAOE = true;
            bullet.linearAOEProjectile = linearAOEProjectile;
            bullet.travelDistance = linearTravelDistance;
            bullet.travelSpeed = linearTravelSpeed;
        }

        private void AssignMainStats(Projectile bullet)
        {
            bullet.motherTower = this;
            bullet.damage = notAffectedByAttackAuras ? startDamage : damage;
            bullet.speed = bulletSpeed;
            bullet.aoeRadius = aoe;
            bullet.isMagical = isMagical;
            bullet.disruption = disruption;
        }

        private void AssignRay(Projectile bullet, Enemy enemy)
        {
            enemyTarget = enemy;

            if (target != null)
                bullet.target = enemyTarget.transform;

            bullet.transform.position = enemyTarget.transform.position;

            if (hitEffect == null) return;

            GameObject effect = (GameObject) Instantiate(hitEffect,
                new Vector3(enemyTarget.transform.position.x,
                    enemyTarget.transform.position.y,
                    -100),
                Quaternion.identity);

            Destroy(effect, 0.5f);
        }

        // ReSharper disable once InconsistentNaming
        private void AssignDOTs(Projectile bullet)
        {
            if (!isDOT) return;

            bullet.dOTDamage = dOTDamage;
            bullet.dOTMagical = dOTMagical;
            bullet.isDamaging = isDOT;

            bullet.dOTUniqueIdentifier =
                !dOTUniqueIdentifier.IsNullOrEmpty() ? dOTUniqueIdentifier : id.ToString();

            bullet.dOTPenetration = dOTPenetration;
            bullet.debuffDuration = dotDuration;
            bullet.debuffTickFrequency = debuffTickFrequency;
            bullet.isStackable = isStackable;
            bullet.updatesAllSimilarDOTsCooldown = updatesCooldown;

            if (!isDOTSlowing) return;

            bullet.isSlowing = true;
            bullet.slowRate = dotSlowRate;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, range);
        }

        private void LocateMineSpots()
        {
            List<Vector3> spots = FindSpots();
            priorityMines = new List<Mine>();
            notSoPriorityMines = new List<Mine>();

            if (Vector3.Distance(spots[0], treeTransformPosition) < Vector3.Distance(spots[1], treeTransformPosition))
            {
                priorityMineSpot = spots[0];
                notSoPriorityMineSpot = spots[1];
            }
            else
            {
                priorityMineSpot = spots[1];
                notSoPriorityMineSpot = spots[0];
            }
        }

        public void SpawnMine(int count)
        {
            if (waveCounterMineSpawn > 1)
                if (waveCounterForMines != 1)
                {
                    waveCounterForMines--;
                    return;
                }
                else
                {
                    waveCounterForMines = waveCounterMineSpawn;
                }


            for (int i = 0; i < count; i++)
            {
                if (priorityMines.Count < mineCountOneSide)
                {
                    Vector3 position = new Vector3();

                    float newX = priorityMineSpot.x + Random.Range(-0.6f, 0.6f);
                    float newY = priorityMineSpot.y + Random.Range(-0.6f, 0.6f);

                    position.x = newX;
                    position.y = newY;
                    position.z = 0;

                    priorityMines.Add(CreateMineInstance(position));
                }
                else if (notSoPriorityMines.Count < mineCountOneSide)
                {
                    Vector3 position = new Vector3();

                    float newX = notSoPriorityMineSpot.x + Random.Range(-0.6f, 0.6f);
                    float newY = notSoPriorityMineSpot.y + Random.Range(-0.6f, 0.6f);

                    position.x = newX;
                    position.y = newY;
                    position.z = 0;

                    notSoPriorityMines.Add(CreateMineInstance(position));
                }
            }
        }


        private Mine CreateMineInstance(Vector3 position)
        {
            GameObject mineObject = Instantiate(mineGameObject, position, Quaternion.identity);
            return AssignStatsToMine(mineObject.GetComponent<Mine>());
        }

        private Mine AssignStatsToMine(Mine mine)
        {
            mine.motherTower = this;
            mine.mineAOE = mineAOE;
            mine.blowUpEffect = blowEffect;
            mine.damage = damage;
            mine.speed = bulletSpeed;
            mine.aoeRadius = aoe;
            mine.isMagical = isMagical;
            mine.targetEnemy = enemyTarget;

            AssignLinearAOE(mine);

            if (isPenetrative)
            {
                mine.isPenetrative = isPenetrative;
                mine.penetration = penetration;
            }

            //  if (bullet != null)
            //      bullet.Seek(target);

            if (isRay)
            {
                mine.target = enemyTarget.transform;
                mine.transform.position = enemyTarget.transform.position;

                if (isLinearAOE)
                {
                }

                if (hitEffect != null)
                {
                    GameObject effect = (GameObject) Instantiate(hitEffect,
                        new Vector3(enemyTarget.transform.position.x,
                            enemyTarget.transform.position.y,
                            -100),
                        Quaternion.identity);

                    Destroy(effect, 0.5f);
                }
            }

            AssignChain(mine);

            AssignDOTs(mine);

            return mine;
        }


        public void showTotalDamage()
        {
            Debug.Log($"tower {this.name} total damage is {damageTotal}");
        }
    }
}