using Shooting;
using UnityEngine;

namespace UI
{
    [System.Serializable]
    public class FalseCrit
    {
        public int attackCounter;
        public int trueCounter { get; set; }
        public Effect effect;
        public int itemID { get; set; }
    }
}