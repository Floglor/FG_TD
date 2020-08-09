using UnityEngine;

namespace Shooting
{
    [System.Serializable]
    public abstract class Effect : ScriptableObject
    {
        public bool isAffectingStats { get; set; } 

        protected Effect(bool isAffectingStats)
        {
            this.isAffectingStats = isAffectingStats;
        }

        public abstract void ChangeStats(Projectile projectile);
    }
}
