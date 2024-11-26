using System.Collections;
using System.Collections.Generic;
using Shooting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/IgnoreAttackSelf", order = 7)]
public class EnemyIgnoreAttack : EnemySkillEffect
{
    [SerializeField] public float counter;
    public override float propCounter => counter;
    public override void DoEffect(Enemy self)
    {
        self.ignoresNextAttack = true;
    }
}

