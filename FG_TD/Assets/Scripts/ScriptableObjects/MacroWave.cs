using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MicroWave
{
    public GameObject enemyType;
    public int enemyCount;
    public int nextDelay;
}

[CreateAssetMenu(fileName = "MacroWave", menuName = "ScriptableObjects/MacroWaves", order = 2)]
[System.Serializable]
public class MacroWave : ScriptableObject
{
    public List<MicroWave> microwaves;
    public MacroWave nextMacroWave;
}
