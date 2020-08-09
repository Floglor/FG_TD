using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyBox;
using UnityEngine;
using UnityEngine.Networking;

namespace Shooting
{
    public class Mine : Projectile
    {
        public float mineAOE { get; set; }
        public GameObject blowUpEffect { get; set; }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other is CircleCollider2D) return;
            if (!other.CompareTag(Enemy.MyTag)) return;

            //Debug.Log("Kaboom");

            List<Collider2D> enemies = Utils.RemoveEnemyOverlapRepetitions(Physics2D.OverlapCircleAll(transform.position, mineAOE));

            //List<Collider2D> distinctEnemyList = new List<Collider2D>();

            //distinctEnemyList = enemies.ToList();

            //enemies = Utils.RemoveEnemyOverlapRepetitions(enemies);


            foreach (Collider2D enemy in enemies)
            {
                DamageWithoutDestroy(enemy.gameObject, isMagical);
            }

            GoInvisible();

            if (isLinearAOE)
            {
                lastTargetPosition = transform.position;
                LinearExplode();
            }

            isMineBlownOff = true;

            if (!isChainLighting)
            {
                Destroy(gameObject);
            }

            if (blowUpEffect == null) return;
            GameObject effect = Instantiate(blowUpEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);

        }
        

        private void GoInvisible()
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            CircleCollider2D collider2D = gameObject.GetComponent<CircleCollider2D>();
            collider2D.enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, mineAOE);
        }
    }
}