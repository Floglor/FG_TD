using System.Collections;
using System.Collections.Generic;
using Managers;
using MyBox;
using Shooting;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/Speed Buff", order = 4)]
public class EnemySpeedBuff : EnemySkillEffect
{
    public float counter;
    public float seekAOE;

    [Range(0, 100)]
    public int percentageSpeedIncrease;
    public override float propCounter => counter;
    public override void DoEffect(Enemy self)
    {
        Debug.Log("rofl");
        buffID = GetInstanceID();
        List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, seekAOE));

        if (colliders.IsNullOrEmpty())
        {
          
            return;
        }
        
        float maxHPBrother = Mathf.NegativeInfinity;
        Enemy priorityEnemy = null;
        
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Collider2D collider2D in colliders)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Enemy enemy = collider2D.gameObject.GetComponent<Enemy>();

            if (!(maxHPBrother < enemy.startHealth)) continue;
            
            maxHPBrother =
                enemy.startHealth;
            
            
            priorityEnemy = enemy;
        }

        if (priorityEnemy != null)
        {
            //priorityEnemy.regenBuffInstances.Add(new RegenBuffInstance(buffID, regenAmount, true, regenDuration));
            //EnemySelfRegen regenInstance = CreateInstance<EnemySelfRegen>();
            //regenInstance.init(regenAmount, buffID);
            //priorityEnemy.skills.Add(regenInstance);
            
            priorityEnemy.moveSpeedBuffInstances.Add(new MoveSpeedBuffInstance(buffID, percentageSpeedIncrease, true, counter));
            priorityEnemy.ResetMVSP();
            
        }
    }
}
