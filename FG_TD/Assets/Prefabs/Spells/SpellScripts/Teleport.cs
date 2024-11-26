using UnityEngine;
using UnityEngine.UI;

namespace Prefaps.Spells.SpellScripts
{
    public class Teleport : Spell
    {
        public GameObject teleportObject;
        public float lifetime;
        
        
        public Teleport(int cost, Image spellImage) : base(cost, spellImage)
        {
        }

        public override void TakeEffect(GameObject rail, Vector2 clickCoordinates)
        {
            if (rail == null) return;
            if (!PlayerStats.instance.SpendMana(cost)) return;

            Rail railScript = rail.GetComponent<Rail>();
            
            GameObject newTeleport;
            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(clickCoordinates, 0.60f);

            bool waypointFound = false;
            Transform waypoint = null;
            
            foreach (Collider2D collider2D1 in colliders2D)
            {
                if (!collider2D1.CompareTag(WaypointGizmos.MyTag)) continue;
                
                waypointFound = true;
                waypoint = collider2D1.transform;
            }

            if (waypointFound)
            {
                Vector3 position = waypoint.position;
                newTeleport = Instantiate(teleportObject, new Vector3(position.x, position.y), Quaternion.identity);
                Destroy(newTeleport, lifetime);
                return;
            }

            newTeleport = Instantiate(teleportObject,
                railScript.orientation == Orientation.Horizontal
                    ? new Vector3(clickCoordinates.x, railScript.yAlignment)
                    : new Vector3(railScript.xAlignment, clickCoordinates.y), Quaternion.identity);

            Destroy(newTeleport, lifetime);
        }
    }
}
