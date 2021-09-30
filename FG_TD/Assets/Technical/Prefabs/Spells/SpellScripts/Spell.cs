using UnityEngine;
using UnityEngine.UI;

namespace Prefaps.Spells.SpellScripts
{
    public abstract class Spell : MonoBehaviour
    {
        [SerializeField] public int cost;
        public readonly Image spellImage;
        public Button buttonPrefab;
        public float aoe;

        protected Spell(int cost, Image spellImage)
        {
            this.cost = cost;
            this.spellImage = spellImage;
        }


        public abstract void TakeEffect(GameObject rail, Vector2 clickCoordinates);

        protected bool IsThereEnoughMana()
        {
            if (cost > PlayerStats.instance.Mana)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.Log("Not enough mana");
                return false;
            }

            PlayerStats.instance.Mana -= cost;
            return true;
        }

        public void ChargePlayerSpell()
        {
            PlayerStats.instance.ChargeSpell(this);
        }

        public void DischargePlayerSpell()
        {
            PlayerStats.instance.CancelSpell();
        }
    }
}