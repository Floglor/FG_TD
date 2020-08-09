using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [System.Serializable]
    public class MicroWave
    {
        public GameObject enemyType;
        public int enemyCount;
        public int nextDelay;
        public bool nextIsSimultaneous;
    }

    [CreateAssetMenu(fileName = "MacroWave", menuName = "ScriptableObjects/MacroWave", order = 2)]
    [System.Serializable]
    public class MacroWave : ScriptableObject
    {
        public List<MicroWave> microwaves;
        public MacroWave nextMacroWave;
        public int moneyGain;
        public int manaGain;
    }
}