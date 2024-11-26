using System.Collections;
using System.Collections.Generic;
using Managers;
using MyBox;
using Shooting;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/Shield Buff", order = 6)]
public class ShieldBuff : EnemySkillEffect
{
    public int counter;
    public int shieldAmount;
    public float seekAOE;
    public override float propCounter => counter;
    public override void DoEffect(Enemy self)
    {
        List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, seekAOE));

        if (colliders.IsNullOrEmpty())
        {
            return;
        }

        float lessShieldHP = Mathf.Infinity;
        Enemy priorityEnemy = null;
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Collider2D collider2D in colliders)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Enemy enemy = collider2D.gameObject.GetComponent<Enemy>();

            if (lessShieldHP < enemy.health+enemy.shield) continue;

            lessShieldHP =
                enemy.health + enemy.shield;
            
            
            priorityEnemy = enemy;
        }
        
        if (priorityEnemy != null)
        {
            priorityEnemy.GainShield(shieldAmount);
            
        }
    }
}
