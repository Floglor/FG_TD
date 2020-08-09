using System.Collections;
using System.Collections.Generic;
using Managers;
using MyBox;
using UI;
using UnityEngine;

[System.Serializable]


public class BoolStats
{
   
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

    [Header("Tower Variant")] public List<TowerVariant> towerUpgrades;
}