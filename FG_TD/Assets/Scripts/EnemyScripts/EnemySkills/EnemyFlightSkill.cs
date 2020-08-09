using System.Collections.Generic;
using Managers;
using MyBox;
using Shooting;
using UnityEngine;

namespace EnemyScripts.EnemySkills
{
    [CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/Flight Buff", order = 2)]
    public class EnemyFlightSkill : EnemySkillEffect
    {
        [SerializeField] public float counter;
        [SerializeField] public float flightSeekAOE;
        [SerializeField] public float buffDuration;
        [SerializeField] public int flightStrength;
        public override float propCounter => counter;

        public override void DoEffect(Enemy self)
        {
            List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(self.transform.position, flightSeekAOE));

            if (colliders.IsNullOrEmpty()) return;
        
            float shortestDistanceToNextCheckPoint = Mathf.Infinity;
            Enemy priorityEnemy = null;
        
        
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Collider2D collider2D in colliders)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Enemy enemy = collider2D.gameObject.GetComponent<Enemy>();

                if (!(shortestDistanceToNextCheckPoint <
                      Vector3.Distance(enemy.transform.position, enemy.target.transform.position))) continue;
            
                shortestDistanceToNextCheckPoint =
                    Vector3.Distance(enemy.transform.position, enemy.target.transform.position);
                priorityEnemy = enemy;
            }
        
            priorityEnemy.flyBuffInstances.Add(new FlyBuffInstance(GetInstanceID(), flightStrength, true, buffDuration));
            priorityEnemy.ResetMVSP();
        }
    }
}
