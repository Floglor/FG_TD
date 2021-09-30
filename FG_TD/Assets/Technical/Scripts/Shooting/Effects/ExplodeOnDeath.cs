using System.Collections;
using System.Collections.Generic;
using Shooting;
using UnityEngine;

namespace Shooting.Effects
{
    public class ExplodeOnDeathObj : EffectObj
    {
        public int towerDamagePercent;
        public int enemyHPPercent;
        public int flatDamage;
        public float debuffTime;

        public float explodeRadius;
        public GameObject explodeVisualEffect;
        public float trueCounter { get; set; }

        public ExplodeOnDeathObj(ExplodeOnDeath onDeath)
        {
            this.towerDamagePercent = onDeath.towerDamagePercent;
            this.enemyHPPercent = onDeath.enemyHPPercent;
            this.flatDamage = onDeath.flatDamage;
            this.debuffTime = onDeath.debuffTime;
            this.explodeRadius = onDeath.explodeRadius;
            this.explodeVisualEffect = onDeath.explodeVisualEffect;
            this.trueCounter = onDeath.debuffTime;

            isAffectingStats = onDeath.isAffectingStats;
            appliesOnEnemy = onDeath.appliesOnEnemy;
            isTimedDebuff = onDeath.isTimedDebuff;
            description = onDeath.description;
        }
    }

    [CreateAssetMenu(fileName = "Effect", menuName = "Effects/Explode On Death", order = 2)]
    public class ExplodeOnDeath : Effect
    {
        public int towerDamagePercent;
        public int enemyHPPercent;
        public int flatDamage;
        public float debuffTime;

        public float explodeRadius;
        public GameObject explodeVisualEffect;
        public float trueCounter { get; set; }

        public ExplodeOnDeath() : base(false, true)
        {
            trueCounter = debuffTime;
        }

        public override void ChangeStats(Projectile projectile)
        {
        }
    }
}