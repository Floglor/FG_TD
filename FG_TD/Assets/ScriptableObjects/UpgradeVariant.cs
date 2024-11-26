using System.Collections.Generic;
using Managers;
using Shooting;
using Sirenix.OdinInspector;
using Technical.Scripts;
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
public class UpgradeVariant : ScriptableObject
{
    public TowerCostData towerCostData;
    
    public int cost;
   
    public List<Stats> statList;

    public List<UpgradeVariant> nextUpgrades;

    [Header("Tower Variant")] public List<TowerVariant> towerUpgrades;
}