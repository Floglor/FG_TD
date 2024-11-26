using UnityEngine;

namespace Shooting
{
    
    [CreateAssetMenu(fileName = "Effect", menuName = "Effects/Critical Strike", order = 1)]
    public class CriticalStrikeEffect : Effect
    {
        public int damageMultiplier;
      

        public CriticalStrikeEffect() : base(true)
        {
        }
        
        public override void ChangeStats(Projectile projectile)
        {
            projectile.damage = damageMultiplier * projectile.damage;
            
            if (projectile.isPenetrative)
                projectile.penetration = damageMultiplier * projectile.penetration;
            
            
        }
    }
}
