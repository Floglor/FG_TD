using System;
using System.Linq;
using Managers;
using MyBox;
using Unity.UNetWeaver;
using UnityEngine;

namespace Shooting
{
    public class GroundProjectile : Projectile
    {
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision is CircleCollider2D) return;
            if (!collision.gameObject.CompareTag(Enemy.MyTag)) return;
            
            
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy.isFlyingNow)
                return;
            
            if (!enemy.isFlyingNow)
                Damage(collision.gameObject);

            Utils.ApplyColliderEntry(enemy,
                !dOTUniqueIdentifier.IsNullOrEmpty()
                    ? new ColliderContentTicket(GetInstanceID(), new DamageDotInstance(debuffTickFrequency, dOTDamage, debuffDuration,
                        GetInstanceID(), dOTMagical, dOTPenetration, dOTUniqueIdentifier, isStackable,
                        updatesAllSimilarDOTsCooldown, true, isSlowing, slowRate))
                    : new ColliderContentTicket( GetInstanceID(), null));
            
            if (isDamaging)
            {
                CheckAndApplyDot(enemy);
            }
            
            
        }

        /*private void OnTriggerStay2D(Collider2D other)
        {
            if (other is CircleCollider2D) return;
            if (!other.gameObject.CompareTag(Enemy.MyTag)) return;
            
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy.isFlyingNow) return;

            if (isDamaging)
            {
                CheckAndApplyDot(enemy);
            }
            
        }*/

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision is CircleCollider2D) return;
            if (!collision.gameObject.CompareTag(Enemy.MyTag)) return;
            
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Utils.DeleteColliderEntry(enemy, GetInstanceID());
        }


        private void Damage(GameObject enemy)
        {
            if (enemy == null) return;

            Enemy enemyObj = enemy.GetComponent<Enemy>();
            if (penetration > 0)
                enemyObj.TakeDamage(damage, penetration);
            else enemyObj.TakeDamage(damage, isMagical);
        }
    }
}