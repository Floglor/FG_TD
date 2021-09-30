using System.Collections;
using System.Collections.Generic;
using Shooting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkill", menuName = "EnemySkills/SelfRegen", order = 1)]
public class EnemySelfRegen : EnemySkillEffect
{
    public void init(float regenAmount, int id)
    {
         this.regenAmount = regenAmount;
         buffID = id;
    }

    public override float propCounter => 0.75f;

    [Header("X/0.75s")]
    public float regenAmount;

    

    private float regenPool { get; set; }
    public override void DoEffect(Enemy self)
    {
        regenPool += regenAmount;
        while (regenPool > 1)
        {
            self.Heal(1);
            regenPool--;
        }
    }

    
}
