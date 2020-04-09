using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effects { Magical, LinearAOE, Penetrative, Jumping,  }

[System.Serializable]
public class Effect
{
    public Effects effectType;
    
}

[CreateAssetMenu(fileName = "Tower Effect", menuName = "ScriptableObjects/TowerEffectFile", order = 1)]
[System.Serializable]
public class TowerEffects : ScriptableObject
{
    public List<Effect> effects;
    public bool isMagical;
    public float waveSpeed;
    public float waveDistance;
}


