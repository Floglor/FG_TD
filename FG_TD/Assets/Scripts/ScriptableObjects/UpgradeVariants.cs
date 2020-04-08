using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public enum StatsNames { Damage, AttackSpeed, AOE, Range, Magical}

    public StatsNames statName;
    public float statValue;
}
[System.Serializable]
public class TowerVariant
{
    public int cost;
    public GameObject tower;
}


[CreateAssetMenu(fileName = "Upgrade Data", menuName = "ScriptableObjects/UpgradeDataFile", order = 1)]
[System.Serializable]
public class UpgradeVariants : ScriptableObject
{

    public int cost;
    public List<Stats> statList;

    public List<UpgradeVariants> nextUpgrades;

    [Header("Tower Variant")]
    public List<TowerVariant> towerUpgrades;
   
}
