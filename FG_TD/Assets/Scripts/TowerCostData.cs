using System;
using Sirenix.OdinInspector;

namespace Technical.Scripts
{
    [Serializable]
    public class TowerCostData : ICloneable
    {
        [BoxGroup("Damage")]
        [DisableInPrefabs] public int totalDamage;

        [BoxGroup("Damage")]
        public float totalDamageCost;
       // [BoxGroup("Penetrate")]
       // public int totalPenetrate;
       // [BoxGroup("Penetrate")]
       // public int totalPenetrateCost;
       // [BoxGroup("Disruption")]
       // public int totalDisruption;
       // [BoxGroup("Disruption")]
       // public int totalDisruptionCost;
       [DisableInPrefabs] [BoxGroup("Range")]
        public float totalRange;
        [BoxGroup("Range")]
        public float totalRangeCost;
        [DisableInPrefabs] [BoxGroup("FireRate")]
        public float totalFireRate;
        [BoxGroup("FireRate")]
        public float totalFireRateCost;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
