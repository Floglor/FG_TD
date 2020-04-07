using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public enum StatsNames { Damage, AttackSpeed, AOE, range }

    public StatsNames statName;
    public float statValue;
}


[CreateAssetMenu(fileName = "Upgrade Data", menuName = "ScriptableObjects/UpgradeDataFile", order = 1)]
[System.Serializable]
public class UpgradeVariants : ScriptableObject
{

    public int cost;
    public List<Stats> statList;

    public List<UpgradeVariants> nextUpgrades;

    [Header("Tower Variant")]
    public List<GameObject> towerUpgrades;
   
}
