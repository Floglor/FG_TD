using UnityEngine;
using UnityEngine.UI;

namespace Prefaps.Consumables_and_traps
{
    public abstract class Consumable : MonoBehaviour
    {
        public int quantity;
        public int cost;
        public Button buttonPrefab;

        public abstract void TakeEffect(GameObject rail, Vector2 clickCoordinates);

        protected Consumable(int quantity, int cost)
        {
            this.quantity = quantity;
            this.cost = cost;
        }
    }
    
}
