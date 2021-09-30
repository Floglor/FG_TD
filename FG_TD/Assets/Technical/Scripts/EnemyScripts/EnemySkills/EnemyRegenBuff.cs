using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyBox;
using Shooting;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/Regen Buff", order = 3)]
public class EnemyRegenBuff : EnemySkillEffect
{
    // Start is called before the first frame update
    [SerializeField] public float regenDuration;
    public float regenBuffFrequency;
    public override float propCounter => regenBuffFrequency;
    [Header("X/0.75s")]
    public float regenAmount;
    public float seekAOE;
    public bool affectsAll;
    
    public override void DoEffect(Enemy self)
    {
        if (!affectsAll)
        {
            buffID = GetInstanceID();
            List<Collider2D> colliders =
                Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, seekAOE));

            if (colliders.IsNullOrEmpty())
            {

                return;
            }

            float halfHPBrother = Mathf.NegativeInfinity;
            Enemy priorityEnemy = null;


            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Collider2D collider2D in colliders)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Enemy enemy = collider2D.gameObject.GetComponent<Enemy>();

                if (halfHPBrother > enemy.startHealth - enemy.health) continue;

                halfHPBrother =
                    enemy.startHealth - enemy.health;


                priorityEnemy = enemy;
            }

            if (priorityEnemy != null)
            {
                priorityEnemy.regenBuffInstances.Add(new RegenBuffInstance(buffID, regenAmount, true, regenDuration));
                EnemySelfRegen regenInstance = CreateInstance<EnemySelfRegen>();
                regenInstance.init(regenAmount, buffID);
                priorityEnemy.skills.Add(regenInstance);

            }
        }
        else
        {
            buffID = GetInstanceID();
            List<Collider2D> colliders =
                Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, seekAOE));
            
            foreach (Enemy enemy in colliders.Select(collider2D => collider2D.gameObject.GetComponent<Enemy>()))
            {
                enemy.regenBuffInstances.Add(new RegenBuffInstance(buffID, regenAmount, true, regenDuration));
                EnemySelfRegen regenInstance = CreateInstance<EnemySelfRegen>();
                regenInstance.init(regenAmount, buffID);
                enemy.skills.Add(regenInstance);
            }
        }
       
    }
}
