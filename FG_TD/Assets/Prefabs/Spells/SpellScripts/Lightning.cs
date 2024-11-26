using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Managers;
using Prefaps.Spells.SpellScripts;
using Shooting;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Spells.SpellScripts
{
    public class Lightning : Spell
    {
        public int damage;
        public GameObject spellEffect;
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public override void TakeEffect(GameObject rail, Vector2 clickCoordinates)
        {
            if (!PlayerStats.instance.SpendMana(cost)) return;
            
            List<Collider2D> colliders = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(clickCoordinates, aoe));
            
        
            foreach (Collider2D collider2D1 in colliders)
            {
                //Debug.Log(colliders.Count);
                if (collider2D1.CompareTag(Enemy.MyTag))
                    collider2D1.gameObject.GetComponent<Enemy>().TakeDamage(damage, true);
            }

            GameObject effect = Instantiate(spellEffect, clickCoordinates, Quaternion.identity);
        
            Destroy(effect, 5f);
        }

        public Lightning(int cost, Image spellImage) : base(cost, spellImage)
        {
        }
        
        
    
    }
}
