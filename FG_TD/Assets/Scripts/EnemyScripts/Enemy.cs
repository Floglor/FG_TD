using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public readonly struct ArmorBuffInstance
{
    public readonly int instanceId;
    public readonly int armorAmount;

    public ArmorBuffInstance(int instanceId, int armorAmount)
    {
        this.instanceId = instanceId;
        this.armorAmount = armorAmount;
    }
}

public class RegenBuffInstance
{
    public readonly int instanceId;
    public readonly float regenAmount;
    public readonly bool timed;
    public float counter;

    public RegenBuffInstance(int instanceId, float regenAmount, bool timed, float counter)
    {
        this.instanceId = instanceId;
        this.regenAmount = regenAmount;
        this.timed = timed;
        this.counter = counter;
    }
}

public class MoveSpeedBuffInstance
{
    public readonly int instanceId;
    public readonly int mvspAmountPercentage;
    public bool timed;
    public float counter;

    public MoveSpeedBuffInstance(int instanceId, int mvspAmountPercentage, bool timed = false, float counter = 0)
    {
        this.instanceId = instanceId;
        this.mvspAmountPercentage = mvspAmountPercentage;
        this.timed = timed;
        this.counter = counter;
    }
}

public class FlyBuffInstance
{
    public readonly int instanceId;
    public readonly int flyStrengthBuff;
    public bool timed;
    public float counter;

    public FlyBuffInstance(int instanceId, int flyStrengthBuff, bool timed)
    {
        this.instanceId = instanceId;
        this.flyStrengthBuff = flyStrengthBuff;
        this.timed = timed;
    }

    public FlyBuffInstance(int instanceId, int flyStrengthBuff, bool timed, float counter)
    {
        this.instanceId = instanceId;
        this.flyStrengthBuff = flyStrengthBuff;
        this.timed = timed;
        this.counter = counter;
    }
}


public enum AligningType
{
    Armor,
    Fly
}

public enum Type
{
    Small,
    Medium,
    Boss,
    Unique
}

public readonly struct AligningInstance
{
    public readonly AligningType aligningType;
    public readonly int instanceId;
    public readonly float targetMoveSpeed;

    public AligningInstance(int instanceId, float targetMoveSpeed, AligningType aligningType)
    {
        this.instanceId = instanceId;
        this.targetMoveSpeed = targetMoveSpeed;
        this.aligningType = aligningType;
    }
}

public class EssenceAOEInstance
{
    public int instanceID;
    public TowerAI tower;

    public EssenceAOEInstance(int instanceID, TowerAI tower)
    {
        this.instanceID = instanceID;
        this.tower = tower;
    }
}


public class Enemy : MonoBehaviour
{
    private float trueSpeed;

    public float startSpeed;

    public float speed;

    public int startHealth;
    [FormerlySerializedAs("armor")] public int startArmor;
    public int currentArmor;

    public bool isUnstoppable;
    
    public int damage;
    public int essences;
    
    public int startShield;
    public int shield;
    
    public bool ignoresNextAttack { get; set; }


    // ReSharper disable once MemberCanBePrivate.Global
    public const string MyTag = "Enemy";

    public int health { get; set; }
    [Header("Flying")] public bool isFlyingOnStart;
    public bool isFlyingNow;
    public int flyStrengthOnStart;
    public int flyStrength;
    private float flySpeedBuffFlat;
    private float flightSpeedBuffPercentage;


    public Transform target { get; set; }
    public int waypointIndex { get; set; }


    [Header("Unity Specific")] public GameObject deathEffect;
    public Image healthBar;
    public Image shieldBar;
    public Image shieldBackground;
    public GameObject hitEffect;

   // [FormerlySerializedAs("textMeshPro")] public TextMeshProUGUI healthText;
   // public TextMeshProUGUI shieldText;
    public int slowAmountPercentage { get; set; }

    public GameObject auraPrefab;
    private GameObject aura;
    public float currentMvspBuffFlat { get; set; }
    public List<SlowInstance> slowInstances { get; set; }
    public List<MoveSpeedBuffInstance> moveSpeedBuffInstances { get; set; }
    public List<SnareInstance> snareInstances { get; set; }
    private List<ArmorBuffInstance> armorInstances { get; set; }

    public List<FlyBuffInstance> flyBuffInstances { get; set; }
    public List<AligningInstance> aligningInstances { get; set; }

    public List<DamageDotInstance> damageDotInstances { get; set; }

    public List<ColliderContentTicket> contentTickets { get; set; }

    public List<RegenBuffInstance> regenBuffInstances { get; set; }

    public List<EnemySkillEffect> skills;

    public List<EssenceAOEInstance> essenceAOEInstances;


    public Type type;

    private Rigidbody2D rb;
    public Vector2 movingDirection { get; set; }

    [Header("Auras")] public bool isAura;


    [ConditionalField(nameof(isAura))] public bool isArmorAuraBuff;

    [ConditionalField(nameof(isArmorAuraBuff))]
    public int armorAuraAmount;

    [ConditionalField(nameof(isAura))] public bool isMoveSpeedAuraBuff;

    [ConditionalField(nameof(isMoveSpeedAuraBuff))]
    public int mvspBuffPercentage;

    [ConditionalField(nameof(isMoveSpeedAuraBuff))]
    private float moveSpeedAuraRadius;

    [ConditionalField(nameof(isAura))] public bool isFlyBuff;

    [ConditionalField(nameof(isFlyBuff))] public int flyBuffStrength;


    [ConditionalField(nameof(isAura))] public float auraRadius;

    private CircleCollider2D auraCollider;
    private bool alreadyDead;
    private bool hitEffectIsNull;
    private GameObject shieldHitEffect;
    private GameObject shieldBreakEffect;
    private GameObject shieldTexture;
    private GameObject currentShieldTexture;

    public List<GameObject> lightningPoints { get; set; }
    private void Start()
    {
        flightSpeedBuffPercentage = PlayerStats.instance.flightSpeedBuffPercentage;
        flySpeedBuffFlat = PlayerStats.instance.flightSpeedBuffFlat;
        shieldTexture = PlayerStats.instance.shield;
        shieldHitEffect = PlayerStats.instance.shieldHitEffect;
        shieldBreakEffect = PlayerStats.instance.shieldBreakEffect;
        alreadyDead = false;
        trueSpeed = startSpeed;
        speed = startSpeed;
        armorInstances = new List<ArmorBuffInstance>();
        snareInstances = new List<SnareInstance>();
        slowInstances = new List<SlowInstance>();
        aligningInstances = new List<AligningInstance>();
        moveSpeedBuffInstances = new List<MoveSpeedBuffInstance>();
        flyBuffInstances = new List<FlyBuffInstance>();
        damageDotInstances = new List<DamageDotInstance>();
        contentTickets = new List<ColliderContentTicket>();
        regenBuffInstances = new List<RegenBuffInstance>();
        lightningPoints = new List<GameObject>();
        essenceAOEInstances = new List<EssenceAOEInstance>();
        if (skills.IsNullOrEmpty())
            skills = new List<EnemySkillEffect>();
        else
            foreach (EnemySkillEffect enemySkillEffect in skills)
            {
                enemySkillEffect.trueCounter = enemySkillEffect.propCounter;
            }

        currentArmor = startArmor;

        
        if (isAura)
        {
            auraCollider = GetComponent<CircleCollider2D>();
            auraCollider.radius = auraRadius;

            if (isArmorAuraBuff)
            {
                armorInstances.Add(new ArmorBuffInstance(GetInstanceID(), armorAuraAmount));
                ResetArmor();
            }

            if (isMoveSpeedAuraBuff)
            {
                moveSpeedBuffInstances.Add(new MoveSpeedBuffInstance(GetInstanceID(), mvspBuffPercentage));
            }

            if (isFlyBuff)
            {
                flyBuffInstances.Add(new FlyBuffInstance(GetInstanceID(), flyBuffStrength, false));
            }

            aura = Instantiate(auraPrefab, gameObject.transform, false);

            Vector3 transformLocalScale = aura.transform.localScale;

            transformLocalScale.x = auraRadius * 2;
            transformLocalScale.y = auraRadius * 2;

            aura.transform.localScale = transformLocalScale;
        }

        if (isFlyingOnStart)
            isFlyingNow = isFlyingOnStart;

        if (isFlyingNow)
        {
            startSpeed += flySpeedBuffFlat;
            startSpeed = (startSpeed / 100f) * (100 + flightSpeedBuffPercentage);
        }


        health = startHealth;
        //healthText.text = $"{health}/{startHealth}";

        shield = startShield;

        UpdateShieldBar();

        if (target == null)
            target = Waypoints.points[1];

        rb = this.GetComponent<Rigidbody2D>();

        ResetFlight();
        ResetMVSP();
    }

    public void IgnoreAttackProc()
    {
        ignoresNextAttack = false;
    }
    private void FindShit()
    {
        shieldBackground = gameObject.transform.Find("ShieldBackground").gameObject.GetComponent<Image>();
        shieldBar = gameObject.transform.Find("ShieldImage").gameObject.GetComponent<Image>();
       // shieldText = gameObject.transform.Find("ShieldText").gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void UpdateShieldBar()
    {
        if (shield <= 0)
        {
            BreakShield();
            return;
        }

        if (currentShieldTexture == null)
            currentShieldTexture = Instantiate(shieldTexture, transform.position, Quaternion.identity, transform);
        
       // shieldText.enabled = true;
        shieldBar.enabled = true;
        shieldBackground.enabled = true;


      //  shieldText.text = $"{shield}/{startShield}";
        shieldBar.fillAmount = (float) shield / startShield;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other is CircleCollider2D) return;
        if (!isAura) return;
        if (!other.CompareTag(MyTag)) return;

        Enemy brother = other.gameObject.GetComponent<Enemy>();

        if (isArmorAuraBuff)
        {
            if (brother.armorInstances.All(brotherArmorInstance =>
                brotherArmorInstance.instanceId != GetInstanceID()))
            {
                ApplyArmorBuff(brother);
            }
        }

        if (isMoveSpeedAuraBuff)
        {
            if (brother.moveSpeedBuffInstances.All(moveSpeedBuffInstance =>
                moveSpeedBuffInstance.instanceId != GetInstanceID()))
            {
                ApplyMoveSpeedBuff(brother);
            }
        }

        if (isFlyBuff)
        {
            if (brother.flyBuffInstances.All(flyBuffInstance =>
                flyBuffInstance.instanceId != GetInstanceID()))
            {
                ApplyFlyBuff(brother);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is CircleCollider2D) return;
        if (!isAura) return;
        if (!other.CompareTag(MyTag)) return;

        Enemy brother = other.gameObject.GetComponent<Enemy>();

        for (int i = 0; i < brother.armorInstances.Count; i++)
        {
            if (brother.armorInstances[i].instanceId == GetInstanceID())
                brother.armorInstances.Remove(brother.armorInstances[i]);
        }

        for (int i = 0; i < brother.aligningInstances.Count; i++)
        {
            if (brother.aligningInstances[i].instanceId == GetInstanceID())
                brother.aligningInstances.Remove(brother.aligningInstances[i]);
        }

        for (int i = 0; i < brother.moveSpeedBuffInstances.Count; i++)
        {
            if (brother.moveSpeedBuffInstances[i].instanceId == GetInstanceID())
                brother.moveSpeedBuffInstances.Remove(brother.moveSpeedBuffInstances[i]);
        }

        for (int i = 0; i < brother.flyBuffInstances.Count; i++)
        {
            if (brother.flyBuffInstances[i].instanceId == GetInstanceID())
                brother.flyBuffInstances.Remove(brother.flyBuffInstances[i]);
        }

        brother.ResetFlight();
        brother.ResetArmor();
        brother.ResetMVSP();
    }

    private void ApplyFlyBuff(Enemy brother)
    {
        if (brother.flyStrength >= flyBuffStrength) return;

        brother.flyBuffInstances.Add(new FlyBuffInstance(GetInstanceID(), flyBuffStrength, false));
        brother.aligningInstances.Add(new AligningInstance(GetInstanceID(), speed, AligningType.Fly));
        brother.ResetFlight();
        brother.ResetMVSP();
    }


    private void ApplyMoveSpeedBuff(Enemy brother)
    {
        brother.moveSpeedBuffInstances.Add(new MoveSpeedBuffInstance(GetInstanceID(), mvspBuffPercentage));
        brother.ResetMVSP();
    }

    private void ApplyArmorBuff(Enemy brother)
    {
        brother.armorInstances.Add(new ArmorBuffInstance(GetInstanceID(), armorAuraAmount));
        brother.aligningInstances.Add(new AligningInstance(GetInstanceID(), speed, AligningType.Armor));
        brother.ResetMVSP();
        brother.ResetArmor();
    }

    public void ResetFlight()
    {
        bool isBuffed = !flyBuffInstances.IsNullOrEmpty();

        if (shield > 0)
        {
            snareInstances.Clear();
        }

        if (!snareInstances.IsNullOrEmpty())
        {
            int biggestSnareStrength = snareInstances.Select(snareInstance => snareInstance.snareStrength)
                .Concat(new[] {0}).Max();


            int biggestFlyStrength = FindBiggestFlyStrength();

            if (biggestSnareStrength > biggestFlyStrength)
            {
                isFlyingNow = false;
                flyStrength = 0;
                return;
            }
        }
        else
        {
            if (isFlyingOnStart || isBuffed)
            {
                isFlyingNow = true;

                int biggestFlyStrength = FindBiggestFlyStrength();

                flyStrength = biggestFlyStrength;
                return;
            }
        }

        if (isBuffed || isFlyingOnStart) return;

        isFlyingNow = false;
        flyStrength = 0;
    }

    private int FindBiggestFlyStrength()
    {
        return flyBuffInstances.Select(flyBuffInstance => flyBuffInstance.flyStrengthBuff)
            .Concat(new[] {flyStrengthOnStart}).Max();
    }

    private void ResetArmor()
    {
        currentArmor = startArmor;

        if (armorInstances.IsNullOrEmpty()) return;

        int biggestArmorBuff = armorInstances.Select(armorInstance => armorInstance.armorAmount).Concat(new[] {0})
            .Max();

        currentArmor = startArmor + biggestArmorBuff;
    }

    public void ResetMVSP()
    {
        //Debug.Log($"mvsp reset, slowinstancescount = {slowInstances.Count}");
        if (slowInstances.IsNullOrEmpty())
        {
            slowAmountPercentage = 0;
        }

        if (isFlyingNow)
        {
            startSpeed = trueSpeed + flySpeedBuffFlat;
            startSpeed = startSpeed / 100f * (100f + flightSpeedBuffPercentage);
        }

        speed = startSpeed;
        
        if (isUnstoppable) return;

        if (!moveSpeedBuffInstances.IsNullOrEmpty())
        {
            int biggestSpeedBuff = moveSpeedBuffInstances
                .Select(moveSpeedBuffInstance => moveSpeedBuffInstance.mvspAmountPercentage).Concat(new[] {0})
                .Max();

            currentMvspBuffFlat = startSpeed / 100 * biggestSpeedBuff;

            speed = (startSpeed + currentMvspBuffFlat) / 100 * (100 - slowAmountPercentage);
        }
        else if (!aligningInstances.IsNullOrEmpty())
        {
            MakeAlignments();
        }
        else
        {
            speed = startSpeed / 100 * (100 - slowAmountPercentage);
        }
    }

    private void MakeAlignments()
    {
        float lowestMoveSpeedAlignment = aligningInstances
            .Select(aligningInstance => aligningInstance.targetMoveSpeed).Concat(new[] {Mathf.Infinity}).Min();

        if (speed < lowestMoveSpeedAlignment) return;
        
        PlayerStats playerStats = PlayerStats.instance;
        switch (type)
        {
            case Type.Small:
                if (playerStats.smallMVSPDifferenceBarrier <= speed-lowestMoveSpeedAlignment) return;
                speed -= (speed - lowestMoveSpeedAlignment)/ 100 * (100 - playerStats.smallMVSPChangePercent);
                break;
            case Type.Medium:
                if (playerStats.mediumMVSPDifferenceBarrier <= speed-lowestMoveSpeedAlignment) return;
                speed -= (speed - lowestMoveSpeedAlignment)/ 100 * (100 - playerStats.mediumMVSPChangePercent);
                break;
            case Type.Boss:
                if (playerStats.bossMVSPDifferenceBarrier <= speed-lowestMoveSpeedAlignment) return;
                speed -= (speed - lowestMoveSpeedAlignment)/ 100 * (100 - playerStats.bossMVSPChangePercent);
                break;
            case Type.Unique:
                if (playerStats.uniqueMVSPDifferenceBarrier <= speed-lowestMoveSpeedAlignment) return;
                speed -= (speed - lowestMoveSpeedAlignment)/ 100 * (100 - playerStats.uniqueMVSPChangePercent);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }


    private void FixedUpdate()
    {
       // if (alreadyDead) return;
        
        try
        {
            movingDirection = target.position - transform.position;
        }
        catch (Exception e)
        {
            Debug.Log($"already dead = {alreadyDead}");
            if (target == null) Debug.Log("target = null!");
            if (transform == null) Debug.Log("transform = null!");
            
            Console.WriteLine(e);
            throw;
        }
        MoveWithTranslate(movingDirection);

        if (Vector2.Distance(transform.position, target.position) <=
            0.04f * PlayerStats.instance.gameSpeedMultiplier)
        {
            GetNextWaypoint();
        }

        EffectsUpdate();
    }

    private void EffectsUpdate()
    {
        //int slowInstancesCount = slowInstances.Count;

        SlowEffectsUpdate();

        DOTEffectsUpdate();

        SkillsUpdate();

        TimedBuffsUpdate();
    }

    private void TimedBuffsUpdate()
    {
        for (int i = 0; i < flyBuffInstances.Count; i++)
        {
            if (!flyBuffInstances[i].timed) return;

            flyBuffInstances[i].counter -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

            if (flyBuffInstances[i].counter <= 0)
            {
                flyBuffInstances.RemoveAt(i);
            }
        }

        for (int i = 0; i < regenBuffInstances.Count; i++)
        {
            if (!regenBuffInstances[i].timed) return;

            regenBuffInstances[i].counter -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

            if (!(regenBuffInstances[i].counter <= 0)) continue;


            if (skills.IsNullOrEmpty()) continue;

            for (int j = 0; j < skills.Count; j++)
            {
                if (skills[j].buffID == regenBuffInstances[i].instanceId)
                {
                    skills.RemoveAt(j);
                }
            }

            regenBuffInstances.RemoveAt(i);
        }

        for (int i = 0; i < moveSpeedBuffInstances.Count; i++)
        {
            if (!moveSpeedBuffInstances[i].timed) return;

            moveSpeedBuffInstances[i].counter -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

            if (!(moveSpeedBuffInstances[i].counter <= 0)) continue;

            moveSpeedBuffInstances.RemoveAt(i);
        }
    }

    private void SkillsUpdate()
    {
        if (skills.IsNullOrEmpty()) return;

        foreach (EnemySkillEffect skillEffect in skills)
        {
            skillEffect.trueCounter -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;

            if (Mathf.Round(skillEffect.trueCounter * 10) / 10 <= 0)
            {
                skillEffect.trueCounter = skillEffect.propCounter;
                skillEffect.DoEffect(this);
            }
        }
    }

    private void DOTEffectsUpdate()
    {
        if (damageDotInstances.IsNullOrEmpty()) return;

        if (shield > 0)
        {
            damageDotInstances.Clear();
            return;
        }

        //int dotInstancesCount = damageDotInstances.Count;

        for (int i = 0; i < damageDotInstances.Count; i++)
        {
            //Debug.Log(damageDotInstances[i].dotDuration);
            damageDotInstances[i].tickCountdown -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
            damageDotInstances[i].dotDuration -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;


            if (Mathf.Round(damageDotInstances[i].dotDuration * 10) / 10 <= 0)
            {
                damageDotInstances.Remove(damageDotInstances[i]);
                continue;
            }

            if ((Mathf.Round(damageDotInstances[i].tickCountdown * 10) / 10 <= 0))
            {
                if (damageDotInstances[i].isMagical)
                    for (int j = 0; j < damageDotInstances[i].stacks; j++)
                    {
                        TakeDamage(damageDotInstances[i].damage, damageDotInstances[i].isMagical);
                    }

                else
                    for (int j = 0; j < damageDotInstances[i].stacks; j++)
                    {
                        TakeDamage(damageDotInstances[i].damage, damageDotInstances[i].penetrationDamage);
                    }

                damageDotInstances[i].tickCountdown = damageDotInstances[i].tickRate;

                DOTApplyCheck(damageDotInstances[i]);
            }
        }
    }

    private void DOTApplyCheck(DamageDotInstance damageDotInstance)
    {
        //Debug.Log(contentTickets.Count);

        if (contentTickets.IsNullOrEmpty()) return;


        foreach (ColliderContentTicket colliderContentTicket in contentTickets.Where(colliderContentTicket =>
            colliderContentTicket.dotInstance.uniqueIdentifier == damageDotInstance.uniqueIdentifier))
        {
            damageDotInstance.dotDuration = colliderContentTicket.dotInstance.dotDuration;
            //Debug.Log("reapplying dot");

            if (damageDotInstance.isStackable)
            {
                damageDotInstance.stacks++;
            }

            //Debug.Log($"slow {slowInstances[0].instanceId} vs {damageDotInstance.instanceId}");

            foreach (SlowInstance slowInstance in slowInstances.Where(slowInstance =>
                slowInstance.instanceId == damageDotInstance.instanceId))
            {
                slowInstance.duration = damageDotInstance.dotDuration;
            }
        }
    }

    private void SlowEffectsUpdate()
    {
        if (shield > 0)
        {
            slowInstances.Clear();
            ResetMVSP();
            return;
        }

        for (int i = 0; i < slowInstances.Count; i++)
        {
            SlowInstance slowInstance = slowInstances[i];

            if (slowInstance.duration <= -9f) continue;

            slowInstance.duration -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
            //Debug.Log(slowInstance.duration);

            if (slowInstance.duration <= 0.1f)
            {
                slowInstances.Remove(slowInstances[i]);
                ResetMVSP();
            }
            else
            {
                slowInstances[i] = slowInstance;
                //Debug.Log(slowInstance.duration);
            }
        }
    }

    private void CheckRepeatingDot()
    {
        int instancesCount = damageDotInstances.Count;
        for (int i = 0; i < instancesCount; i++)
        {
            for (int j = 0; j < instancesCount; j++)
            {
                if (damageDotInstances[i].isStackable) continue;

                if (damageDotInstances[i] == damageDotInstances[j]) continue;
                //Debug.Log($"{damageDotInstances[i].uniqueIdentifier} vs {damageDotInstances[j].uniqueIdentifier} ({damageDotInstances[i].instanceId} vs {damageDotInstances[j].instanceId})");
                if (string.CompareOrdinal(damageDotInstances[i].uniqueIdentifier,
                    damageDotInstances[j].uniqueIdentifier) != 0) continue;


                damageDotInstances.Remove(damageDotInstances[i].dotDuration <= damageDotInstances[j].dotDuration
                    ? damageDotInstances[i]
                    : damageDotInstances[j]);

                instancesCount--;
            }
        }
    }

    public void AddDotInstance(DamageDotInstance dotInstance)
    {
        dotInstance.dotDuration += 0.1f;
        damageDotInstances.Add(dotInstance);
        CheckRepeatingDot();
    }

    private void MoveWithPhysics(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;
    }

    void MoveWithTranslate(Vector2 dir)
    {
        transform.Translate(dir.normalized * (PlayerStats.instance.gameSpeedMultiplier * speed * Time.deltaTime),
            Space.World);
    }


    private void GetNextWaypoint()
    {
        if (waypointIndex >= Waypoints.points.Length - 1)
        {
            DamagePlayer();
            return;
        }

        waypointIndex++;
        target = Waypoints.points[waypointIndex];
    }

    private void SpawnHitEffect()
    {
        if (hitEffectIsNull || hitEffect == null)
        {
            hitEffectIsNull = true;
            return;
        }
        GameObject spawnedHitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(spawnedHitEffect, 2f);
    }
    
    private void SpawnShieldHitEffect()
    {
        GameObject spawnedHitEffect = Instantiate(shieldHitEffect, transform.position, Quaternion.identity);
        Destroy(spawnedHitEffect, 2f);
    }

    public void TakeDamage(int incomingDamage, bool isMagical = false, int disruptionDamage = 0)
    {

        if (disruptionDamage > 0)
        {
            shield -= PlayerStats.instance.disruptionMultiplier * disruptionDamage;
            SpawnShieldHitEffect();
            
            if (shield <= 0)
            {
                BreakShield();
            }
        }
        
        if (isMagical)
        {
            if (shield > 0)
            {
                if (incomingDamage > shield)
                {
                    BreakShield();
                    int residualDamage = incomingDamage - shield;
                    shield = 0;
                    TakeDamage(residualDamage, true);
                    return;
                }

                shield -= incomingDamage;
                UpdateShieldBar();
                if (shield == 0) BreakShield();
                return;
            }


            health -= incomingDamage;

            healthBar.fillAmount = (float) health / (float) startHealth;
            SpawnHitEffect();

            //healthText.text = $"{health}/{startHealth}";

            if (health <= 0)
                Die();
        }
        else
        {
            if (shield > 0)
            {
                if (incomingDamage > shield)
                {
                    int residualDamage = incomingDamage - shield;
                    shield = 0;
                    BreakShield();
                    TakeDamage(residualDamage);
                    return;
                }

                shield -= incomingDamage;
                SpawnShieldHitEffect();
                UpdateShieldBar();
                if (shield == 0) BreakShield();
                return;
            }

            if (incomingDamage - currentArmor <= 0)
                return;

            if (currentArmor < 0)
                health -= incomingDamage - (currentArmor * 2);
            else
                health -= incomingDamage - currentArmor;

            healthBar.fillAmount = (float) health / (float) startHealth;
        //    healthText.text = $"{health}/{startHealth}";
            
            SpawnHitEffect();

            if (health <= 0)
                Die();
        }
    }

    
    
    private void BreakShield()
    {
        Destroy(currentShieldTexture);
        
        shieldBackground.enabled = false;
        shieldBar.enabled = false;
        //shieldText.enabled = false;
        shield = 0;

        if (startShield == 0) return;
        GameObject visualEffect = Instantiate(shieldBreakEffect, transform.position, Quaternion.identity);
        Destroy(visualEffect, 2f);
    }

    public void GainShield(int amount)
    {
        shield += amount;
        UpdateShieldBar();
    }

    public void TakeDamage(int incomingDamage, int penetrationDamage, int disruptionDamage = 0)
    {
        if (disruptionDamage > 0)
        {
            shield -=  PlayerStats.instance.disruptionMultiplier * disruptionDamage;
            if (shield <= 0)
            {
                BreakShield();
            }
        }
        
        if (shield > 0)
        {
            if (incomingDamage + penetrationDamage > shield)
            {
                int residualDamage = incomingDamage + penetrationDamage - shield;
                shield = 0;
                BreakShield();
                TakeDamage(residualDamage);
                return;
            }

            shield -= incomingDamage;
            if (shield == 0) BreakShield();
            return;
        }

        if (penetrationDamage - currentArmor > 0)
        {
            health -= incomingDamage + (penetrationDamage - currentArmor) * 2;

            healthBar.fillAmount = (float) health / (float) startHealth;
            //healthText.text = $"{health}/{startHealth}";

            if (health <= 0)
                Die();
        }
        else TakeDamage(incomingDamage + penetrationDamage);
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > startHealth)
            health = startHealth;

        healthBar.fillAmount = (float) health / (float) startHealth;
        //healthText.text = $"{health}/{startHealth}";
    }


    public void Die()
    {
        if (alreadyDead) return;
        if (!essenceAOEInstances.IsNullOrEmpty())
        {
            foreach (EssenceAOEInstance instance 
                in essenceAOEInstances.Where(instance => instance.tower.GainEssences(essences)))
            {
                break;
            }
        }
        UntieLightningPoints();
        PlayerStats.enemiesAlive--;
        Destroy(gameObject);
        GameObject effectInst = (GameObject) Instantiate(deathEffect,
            new Vector3(transform.position.x, transform.position.y, -100), transform.rotation);
        

        Destroy(effectInst, 10f);

        alreadyDead = true;
    }

    private void DamagePlayer()
    {
        UntieLightningPoints();
        Destroy(gameObject);
        PlayerStats.enemiesAlive--;
        PlayerStats.Lives -= damage;
    }

    private void UntieLightningPoints()
    {
        foreach (GameObject lightningPoint in lightningPoints)
        {
            lightningPoint.transform.parent = null;
            lightningPoint.transform.position = transform.position;
        }
    }
}