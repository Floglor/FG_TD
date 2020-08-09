
using Shooting;
using UnityEngine;



namespace Prefaps.Consumables_and_traps
{
    public class PitTrapObject : MonoBehaviour
    {
        public int quantitiy { get; set; }
        private bool isClosed = false;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isClosed) return;
            
            if (other is CircleCollider2D) return;

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy == null) return;
            
            if (enemy.type == Type.Small)
            {
                enemy.Die();
                quantitiy--;
            }

            if (quantitiy != 0) return;
            
            isClosed = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
            //gameObject.GetComponent<BoxCollider2D>().enabled = false;

        }
    }
}
