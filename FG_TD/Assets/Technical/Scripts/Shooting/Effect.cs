using UnityEngine;

namespace Shooting
{

    public abstract class EffectObj 
    {
        public bool isAffectingStats { get; set; }
        public bool appliesOnEnemy { get; set; }

        [Header("Да, если это временный эффект на врага")]
        public bool isTimedDebuff;

        public string description;
    }
    
    [System.Serializable]
    public abstract class Effect : ScriptableObject
    {
        public bool isAffectingStats { get; set; }
        public bool appliesOnEnemy { get; set; }

        [Header("Да, если это временный эффект на врага")]
        public bool isTimedDebuff;

        public string description;

        protected Effect(bool isAffectingStats, bool appliesOnEnemy = false)
        {
            this.isAffectingStats = isAffectingStats;
            this.appliesOnEnemy = appliesOnEnemy;
        }

        public abstract void ChangeStats(Projectile projectile);
    }
}
