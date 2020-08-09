using System.Collections;
using System.Collections.Generic;
using Managers;
using MyBox;
using Shooting;
using UnityEngine;

public class EnemyHeal : EnemySkillEffect
{
    public float counter;
    public float seekAOE;
    public int heal;
    public override float propCounter => counter;
    public override void DoEffect(Enemy self)
    {
        List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, seekAOE));

        if (colliders.IsNullOrEmpty())
        {
          
            return;
        }
        
        float effectiveHealBrother = Mathf.NegativeInfinity;
        Enemy priorityEnemy = null;
        
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Collider2D collider2D in colliders)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Enemy enemy = collider2D.gameObject.GetComponent<Enemy>();

            if (effectiveHealBrother > enemy.startHealth-enemy.health) continue;
            
            effectiveHealBrother =
                enemy.startHealth-enemy.health;
            
            
            priorityEnemy = enemy;
        }

        if (priorityEnemy != null)
        {
            priorityEnemy.Heal(heal);
            
        }
    }
}
