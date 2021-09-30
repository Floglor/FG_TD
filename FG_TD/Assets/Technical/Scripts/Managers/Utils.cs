using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyBox;
using Shooting;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;


namespace Managers
{

    public class StringNames
    {
        public const string DamageModifier = "Damage Modifier";
        public const string Damage = "Damage";
        public const string AttackSpeed = "Attack Speed";
        public const string Range = "Range";
        public const string AOE = "Area of effect";
        public const string BulletSpeed = "Projectile Speed";
        public const string TargetPriority = "TargetPriority";

        public const string ChainLength = "Target Count";
        public const string SplitshotTargetsCount = "Maximum Targets";
        public const string SlowingAuraPercentage = "Slowing Rate";
        public const string SnaringStrength = "Snare Strength";
        public const string DOTTickDamage = "Tick Damage";
        public const string DOTSlowPercentage = "Slow Rate";

        public const string AoeProjectileSpeed = "Wave Speed";
        public const string AoeProjectileTravelRange = "Wave Travel Range";
        public const string ChainSeekRadius = "Seek Radius";
        public const string AuraRadius = "Aura Radius";
        public const string DotDuration = "Duration";
        public const string DOTTickInterval = "Tick Interval";
        public const string DOTChangingIdentifier = "Changes Identifier";
        public const string EnemySuckAOE = "Essense Gather AOE";
    }


    [Serializable]
    public class Stats
    {
        public enum BoolStatName
        {
            None,
            IsRay,
            IsChainLightning,
            IsLinearAoe,
            IsSplitshot,
            IsSlowingAura,
            IsAura,
            IsSnaringAura,
            IsDot,
            IsDotSlowing,
            IsDOTMagical,
            IsRoundAttack,
            IsGroundType,
            IsMagical,
        }

        public enum IntStatName
        {
            None,
            Damage,
            PenetrateAmount,
            ChainLength,
            Disruption,
            SplitshotTargetsCount,
            SlowingAuraPercentage,
            SnaringStrength,
            DOTTickDamage,
            DOTSlowPercentage,
            DOTPenetrativeDamage,
            DamageModifier
        }

        public enum FloatStatName
        {
            None,
            AttackSpeed,
            Aoe,
            Range,
            AoeProjectileSpeed,
            AoeProjectileTravelRange,
            ChainSeekRadius,
            AuraRadius,
            DotDuration,
            DOTTickInterval,
            DOTChangingIdentifier,
            EnemySuckAOE
        }

        [ShowIf("@intStatName == Stats.IntStatName.Damage || floatStatName == Stats.FloatStatName.AttackSpeed || floatStatName == Stats.FloatStatName.Range")]
        public float statCost;
        
        public IntStatName intStatName;
        public FloatStatName floatStatName;
        public BoolStatName boolStatName;
        [HideIf("@intStatName == Stats.IntStatName.Damage || floatStatName == Stats.FloatStatName.AttackSpeed || floatStatName == Stats.FloatStatName.Range")]
        public float statValue;
        public List<FalseCrit> falseCrits;
       
    }

    [Serializable]
    public class ItemWheelSlotsCoords
    {
        public float y;
        public float x;
    }

    [Serializable]
    public class ItemWheeSlotCoordsList
    {
        public List<ItemWheelSlotsCoords> slotList;
    }

    public class SnareInstance
    {
        public readonly int instanceId;
        public readonly int snareStrength;

        public SnareInstance(int instanceId, int snareStrength)
        {
            this.instanceId = instanceId;
            this.snareStrength = snareStrength;
        }
    }

    public enum ShootingMode
    {
        Closest,
        ClosestToThrone
    }

    public enum TargetPriority
    {
        Small,
        Medium,
        Boss,
        Aura
    }

    public class DamageDotInstance
    {
        public int damage;
        public float tickRate;
        public float tickCountdown;
        public float dotDuration;
        public int instanceId;
        public bool isMagical;
        public int penetrationDamage;
        public string uniqueIdentifier;
        public bool isStackable;
        public bool updatesCooldown;
        public bool isFromGroundProjectile;
        public bool isSlowing;
        public int slowingPercentage;
        public int stacks;


        public DamageDotInstance(float tickRate, int damage, float dotDuration, int instanceId, bool isMagical,
            int penetrationDamage, string uniqueIdentifier, bool isStackable, bool updatesCooldown,
            bool isFromGroundProjectile, bool isSlowing, int slowingPercentage, int stacks = 1)
        {
            this.tickRate = tickRate;
            this.damage = damage;
            this.dotDuration = dotDuration;
            this.instanceId = instanceId;
            tickCountdown = tickRate;
            this.isMagical = isMagical;
            this.penetrationDamage = penetrationDamage;
            this.uniqueIdentifier = uniqueIdentifier;
            this.isStackable = isStackable;
            this.updatesCooldown = updatesCooldown;
            this.isFromGroundProjectile = isFromGroundProjectile;
            this.isSlowing = isSlowing;
            this.slowingPercentage = slowingPercentage;
            this.stacks = stacks;
        }
    }

    public readonly struct ColliderContentTicket
    {
        public readonly int sourceId;
        public readonly DamageDotInstance dotInstance;

        public ColliderContentTicket(int sourceId, DamageDotInstance dotInstance)
        {
            this.sourceId = sourceId;
            this.dotInstance = dotInstance;
        }
    }


    public readonly struct RangeEnemyPair
    {
        public readonly float distanceToEnemy;
        public readonly float enemyDistanceToPriority;
        public readonly Enemy enemy;

        public RangeEnemyPair(float distanceToEnemy, Enemy enemy, float enemyDistanceToPriority)
        {
            this.distanceToEnemy = distanceToEnemy;
            this.enemy = enemy;
            this.enemyDistanceToPriority = enemyDistanceToPriority;
        }
    }

    public class SlowInstance
    {
        public readonly int instanceId;
        public readonly int slowAmount;
        public float duration;

        public SlowInstance(int instanceId, int slowAmount, float duration = -10)
        {
            this.instanceId = instanceId;
            this.slowAmount = slowAmount;
            this.duration = duration;
        }
    }

    public readonly struct AttackBuffInstance
    {
        public readonly int instanceId;
        public readonly int flatAttackBuff;
        public readonly int percentageAttackBuff;
        public readonly bool isFromAura;

        public AttackBuffInstance(int flatAttackBuff, int instanceId, int percentageAttackBuff, bool isFromAura)
        {
            this.flatAttackBuff = flatAttackBuff;
            this.instanceId = instanceId;
            this.percentageAttackBuff = percentageAttackBuff;
            this.isFromAura = isFromAura;
        }
    }


    public readonly struct AttackSpeedBuffInstance
    {
        public readonly int instanceId;
        public readonly float flatAttackSpeedBuff;
        public readonly int percentageAttackSpeedBuff;
        public readonly bool isFromAura;

        public AttackSpeedBuffInstance(float flatAttackSpeedBuff, int instanceId, int percentageAttackSpeedBuff,
            bool isFromAura)
        {
            this.flatAttackSpeedBuff = flatAttackSpeedBuff;
            this.instanceId = instanceId;
            this.percentageAttackSpeedBuff = percentageAttackSpeedBuff;
            this.isFromAura = isFromAura;
        }
    }

    public readonly struct RadiusBuffInstance
    {
        public readonly int instanceId;
        public readonly int percentageRadiusBuff;
        public readonly float flatRadiusBuff;
        public readonly bool isFromAura;

        public RadiusBuffInstance(int percentageRadiusBuff, int instanceId, bool isFromAura, float flatRadiusBuff)
        {
            this.percentageRadiusBuff = percentageRadiusBuff;
            this.instanceId = instanceId;
            this.isFromAura = isFromAura;
            this.flatRadiusBuff = flatRadiusBuff;
        }
    }

    public enum TowerTypeSmallStatList
    {
        ProjectileTower
    }

    public static class Utils
    {
        
        public const string FloatFormat = "#.##";
        private static void ApplySlow(Enemy enemy, int slowRate, int id)
        {
            List<SlowInstance> slowInstances = enemy.slowInstances;

            int biggestSlowInstance = slowRate;

            if (slowInstances.Count > 0)
                foreach (SlowInstance slowInstance in slowInstances.Where(slowInstance =>
                    slowInstance.slowAmount > slowRate))
                {
                    biggestSlowInstance = slowInstance.slowAmount;
                }

            slowInstances.Add(new SlowInstance(id, slowRate));

            if (biggestSlowInstance > slowRate) return;

            enemy.slowAmountPercentage = slowRate;
            enemy.ResetMVSP();
        }

        public static decimal RoundToTwoDecimals(decimal number)
        {
            return  Math.Round(number, 2);
        }
        
        public static float RoundToTwoDecimals(float number)
        {
            return  (float) Math.Round(number, 2);
        }
        public static string MakeFalseCritDescription(List<FalseCrit> falseCrits)
        {
            if (falseCrits.IsNullOrEmpty())
                return null;

            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (FalseCrit falseCrit in falseCrits)
            {
                 stringBuilder.Append(falseCrit.effect.description);
                //stringBuilder.Append(".");

                if (falseCrit.attackCounter > 1)
                {
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.AppendLine($"Once every {falseCrit.attackCounter} attacks.");
                }
            }
            
            
            return stringBuilder.ToString();
        }
        
        public static void InitiateSmallStatList(TowerAI towerAI, GameObject smallStatList)
        {
            List<GameObject> smallStatChildren = smallStatList.gameObject.GetAllChildren();

            GameObject statNames = smallStatChildren[0];
            GameObject statValues = smallStatChildren[1];

            List<GameObject> statNamesList = statNames.GetAllChildren();
            List<GameObject> statValuesList = statValues.GetAllChildren();

            if (towerAI.typeSmallStatList == TowerTypeSmallStatList.ProjectileTower)
            {
                statNamesList[0].GetComponent<TextMeshProUGUI>().text = "Damage";
                statNamesList[1].GetComponent<TextMeshProUGUI>().text = "Attack Speed";
                statNamesList[2].GetComponent<TextMeshProUGUI>().text = "Range";
                statNamesList[3].GetComponent<TextMeshProUGUI>().text = "Target";

                statValuesList[0].GetComponent<TextMeshProUGUI>().text =
                    $"{towerAI.damage}/<color=red>{towerAI.penetration}</color>/<color=blue>{towerAI.disruption}</color>";
                statValuesList[1].GetComponent<TextMeshProUGUI>().text = $"{towerAI.fireRate:#.##}";
                statValuesList[2].GetComponent<TextMeshProUGUI>().text = $"{towerAI.range:#.##}";
                statValuesList[3].GetComponent<TextMeshProUGUI>().text =
                    $"{Enum.GetName(typeof(TargetPriority), towerAI.targetPriority)}";
            }
        }
        
        
        public static void ApplyDamageDot(Enemy enemy, DamageDotInstance dotInstance)
        {
            bool isAlreadyOn = false;

            if (enemy.damageDotInstances.Count > 0)
                foreach (DamageDotInstance enemyDamageDotInstance in
                    enemy.damageDotInstances.Where(enemyDamageDotInstance =>
                        enemyDamageDotInstance.uniqueIdentifier == dotInstance.uniqueIdentifier))
                {
                    //Debug.Log($"{dOTUniqueIdentifier} and {enemyDamageDotInstance.uniqueIdentifier}");
                    isAlreadyOn = true;

                    if (!dotInstance.updatesCooldown) continue;

                    if (dotInstance.dotDuration > enemyDamageDotInstance.dotDuration)
                        enemyDamageDotInstance.dotDuration = dotInstance.dotDuration;
                }

            if (dotInstance.isSlowing)
            {
                ApplySlow(enemy, dotInstance.slowingPercentage, dotInstance.instanceId);
            }

            if (!dotInstance.isStackable && !dotInstance.isFromGroundProjectile)
                if (isAlreadyOn)
                    return;

            //Debug.Log("Inst add");
            //TODO: maybe should use new instance
            enemy.AddDotInstance(dotInstance);
        }

        public static List<Collider2D> RemoveEnemyOverlapRepetitions(Collider2D[] enemies)
        {
            return enemies.Where(t => t.CompareTag(Enemy.MyTag) && !(t is CircleCollider2D)).ToList();
        }


        public static void ApplyColliderEntry(Enemy enemy, ColliderContentTicket colliderContentTicket)
        {
            enemy.contentTickets.Add(colliderContentTicket);
            //Debug.Log("adding ticket");
        }


        public static void DeleteColliderEntry(Enemy enemy, int id)
        {
            if (enemy.contentTickets.IsNullOrEmpty()) return;

            //Debug.Log("removing ticket");

            for (int i = 0; i < enemy.contentTickets.Count; i++)
            {
                if (enemy.contentTickets[i].sourceId == id)
                {
                    enemy.contentTickets.RemoveAt(i);
                }
            }
        }
        
        

        public static List<GameObject> GetAllChildren(this GameObject go)
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < go.transform.childCount; i++)
            {
                list.Add(go.transform.GetChild(i).gameObject);
            }

            return list;
        }
    }
}