using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public enum StatsNames { Damage, AttackSpeed, AOE, range }

    public StatsNames statName;
    public float stat;
}


[CreateAssetMenu(fileName = "Upgrade Data", menuName = "ScriptableObjects/UpgradeDataFile", order = 1)]
[System.Serializable]
[CanEditMultipleObjects]
public class UpgradeVariants : ScriptableObject
{

    public int cost;
    public List<Stats> statList;

    public List<UpgradeVariants> nextUpgrades;

    [Header("Tower Variant")]
    public GameObject towerUpgrade;
}
