using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using MyBox;
using Prefaps.Spells.SpellScripts;
using Shooting;
using UnityEngine;
using UnityEngine.UI;

public class ChainLightning : Spell
{
    public GameObject bulletPrefab;
    public int damage;
    [Header("Magical")] public bool isMagical;
    
    [Header("Lightning")] public bool isChainLighting;

    [ConditionalField(nameof(isChainLighting))]
    public int chainLength;

    [ConditionalField(nameof(isChainLighting))]
    public float chainSeekRadius;

    [Header("Penetrative")] public bool isPenetrative;
    [ConditionalField(nameof(isPenetrative))]
    public int penetration;
    
    
    [Header("DOT")] public bool isDOT;
    [ConditionalField(nameof(isDOT))] public int dOTDamage;
    [ConditionalField(nameof(isDOT))] public int dOTPenetration;
    [ConditionalField(nameof(isDOT))] public bool dOTMagical;
    [ConditionalField(nameof(isDOT))] public string dOTUniqueIdentifier;
    [ConditionalField(nameof(isDOT))] public float debuffDuration;
    [ConditionalField(nameof(isDOT))] public float debuffTickFrequency;
    
    public ChainLightning(int cost, Image spellImage) : base(cost, spellImage)
    {
    }

    public override void TakeEffect(GameObject rail, Vector2 clickCoordinates)
    {
        if (!PlayerStats.instance.SpendMana(cost)) return;
        
        List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(clickCoordinates, aoe));
        
        Enemy nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        
        
        foreach (Collider2D collider2D1 in colliders)
        {
            float distanceToEnemy = Vector3.Distance(clickCoordinates, collider2D1.transform.position);

            if (!(distanceToEnemy < shortestDistance)) continue;
            
            shortestDistance = distanceToEnemy;
            nearestEnemy = collider2D1.GetComponent<Enemy>();
        }
        
        SpawnBulletForEnemy(nearestEnemy, clickCoordinates);
    }
    
    private void SpawnBulletForEnemy(Enemy enemy, Vector2 clickCoordinates)
        {
            
            GameObject bulletGO =
                (GameObject) Instantiate(bulletPrefab, clickCoordinates, Quaternion.identity);
            BulletAI bullet = bulletGO.GetComponent<BulletAI>();
            //bullet.motherTower = this;
            bullet.damage = damage;
            bullet.isMagical = isMagical;
            bullet.targetEnemy = enemy;
            

            if (isPenetrative)
            {
                bullet.isPenetrative = isPenetrative;
                bullet.penetration = penetration;
            }

            if (bullet != null && enemy != null)
                bullet.Seek(enemy.transform);

            
            

            bullet.isChainLighting = isChainLighting;
            bullet.chainLength = chainLength;
            bullet.chainTargetRange = chainSeekRadius;
            

            if (isDOT)
            {
                bullet.dOTDamage = dOTDamage;
                bullet.dOTMagical = dOTMagical;
                bullet.isDamaging = isDOT;
                bullet.dOTUniqueIdentifier = dOTUniqueIdentifier;
                bullet.dOTPenetration = dOTPenetration;
                bullet.debuffDuration = debuffDuration;
                bullet.debuffTickFrequency = debuffTickFrequency;
            }

            bullet.isSpawnedWithSkill = true;
            bullet.spawnCoordinatesChainLightning = clickCoordinates;
        }
}