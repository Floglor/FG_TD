using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using MyBox;
using Shooting;
using Sirenix.OdinInspector;
using Technical.Scripts;
using UnityEditor;
using UnityEngine;

internal struct LockPackage
{
    public float lockedRange;
    public float lockedAttackSpeed;
    public int lockedDamage;

    public LockPackage(float lockedRange, float lockedAttackSpeed, int lockedDamage)
    {
        this.lockedRange = lockedRange;
        this.lockedAttackSpeed = lockedAttackSpeed;
        this.lockedDamage = lockedDamage;
    }
}

public class CostToStatTable : SerializedMonoBehaviour
{
    [BoxGroup("Range")] public float startRangeToken;
    [BoxGroup("Range")] public float startRangeCost;
    [BoxGroup("Range")] public float rangeProgression;
    [BoxGroup("Range")] public float rangeStepDistance;

    private Dictionary<int, float> _rangeMap;
    private Dictionary<int, float> _rangeTotalCostMap;

    [BoxGroup("Damage")] public float startCost;
    [BoxGroup("Damage")] [ShowInInspector] public const int DamageStep = 1;
    [BoxGroup("Damage")] public float damageProgression;

    private Dictionary<int, float> _damageMap;
    private Dictionary<int, float> _damageTotalCostMap;

    [BoxGroup("Fire Rate")] public float startFireRateToken;
    [BoxGroup("Fire Rate")] public float startFireRateCost;
    [BoxGroup("Fire Rate")] public float fireRateStepDistance;

    public Shop shop;

    private Dictionary<int, float> _fireRateMap;
    private Dictionary<int, float> _fireRateTotalCostMap;

    private TowerCostData _towerCostDataEvolution;
    [InfoBox(
        "0: рейнж; 1: стоимость рейнжа за шаг; 2: общая сумма костов. \n3: урон; 4: стоимость урона за шаг; 5: общая стоимость за текущий урон")]
    [TableMatrix(HorizontalTitle = "Table")]
    public float[,] CostMatrix = new float[9, 40];

    [Button("Recalculate Game Stats", ButtonSizes.Gigantic)]
    public void RecalculateGameStats()
    {
        RecalculateDamageStat();

        RecalculateRangeStat();

        _fireRateMap = new Dictionary<int, float>(300);
        _fireRateTotalCostMap = new Dictionary<int, float>(300);

        // CostMatrix[5, 0] =  startFireRateToken;
        // CostMatrix[6, 0] =  _fireRateMap[0];

        for (int i = 0; i < 300; i++)
        {
            _fireRateMap.Add(i, startFireRateCost);
            _fireRateTotalCostMap.Add(i, Utils.RoundToTwoDecimals(startFireRateCost * (i + 1)));

            if (i < 40)
            {
                CostMatrix[6, i] = Utils.RoundToTwoDecimals(fireRateStepDistance * i + startFireRateToken);
                CostMatrix[7, i] = _fireRateMap[i];
                CostMatrix[8, i] = _fireRateTotalCostMap[i];
            }
        }
    }

    private void RecalculateRangeStat()
    {
        _rangeMap = new Dictionary<int, float>(300) {{1, startRangeCost}};
        _rangeTotalCostMap = new Dictionary<int, float>(300) {{1, startRangeCost}};
        CostMatrix[0, 0] = (float) startRangeToken;
        CostMatrix[1, 0] = (float) _rangeMap[1];

        for (int i = 1; i < 299; i++)
        {
            _rangeMap.Add(i + 1, Utils.RoundToTwoDecimals(_rangeMap[i] * rangeProgression + _rangeMap[i]));
            _rangeTotalCostMap.Add(i + 1, Utils.RoundToTwoDecimals(_rangeTotalCostMap[i] + _rangeMap[i + 1]));

            if (i < 41)
            {
                if (i > 1)
                    CostMatrix[0, i - 1] =
                        (float) Utils.RoundToTwoDecimals((rangeStepDistance * i) + startRangeToken - rangeStepDistance);

                CostMatrix[1, i - 1] = (float) _rangeMap[i];
                CostMatrix[2, i - 1] = (float) _rangeTotalCostMap[i];
            }
        }

        for (int i = 1; i < _rangeTotalCostMap.Count; i++)
        {
            float range = i * rangeStepDistance + startRangeToken;
            Debug.Log($"{_rangeTotalCostMap[i]}, range = {range}");
            if (range >= 16) break;
        }
    }

    private void RecalculateDamageStat()
    {
        //Key = damage; Value = Cost
        _damageMap = new Dictionary<int, float>(300) {{1, startCost}};
        _damageTotalCostMap = new Dictionary<int, float>(300) {{1, startCost}};
        CostMatrix[3, 0] = 1;
        CostMatrix[4, 0] = _damageMap[1];


        for (int i = 1; i < 299; i++)
        {
            _damageMap.Add(i + 1, Utils.RoundToTwoDecimals(_damageMap[i] * damageProgression + _damageMap[i]));
            _damageTotalCostMap.Add(i + 1, Utils.RoundToTwoDecimals(_damageTotalCostMap[i] + _damageMap[i + 1]));

            if (i < 41)
            {
                CostMatrix[3, i - 1] = i;
                CostMatrix[4, i - 1] = _damageMap[i];
                CostMatrix[5, i - 1] = _damageTotalCostMap[i];
            }
        }
    }

    [Button("Change Tower Stats", ButtonSizes.Large), GUIColor(1f, 0f, 0f)]
    public void ChangeAllTowerStats()
    {
        #if UNITY_EDITOR
        if (EditorUtility.DisplayDialog("Confirm", "Change every tower stat? This action is irreversible", "Yes",
            "No"))
        {
                 RecalculateGameStats();
                
                if (shop.initialTowers.IsNullOrEmpty())
                {
                    Debug.Log("TOO EMPTY");
                    return;
                }
                foreach (GameObject initialTower in shop.initialTowers)
                {
                    if (initialTower == null)
                    {
                        Debug.Log("initial tower = null");
                        continue;
                    }
                    TowerAI towerAI = initialTower.GetComponent<TowerAI>();
                    Debug.Log($"Initial tower: {towerAI.name}");

                    TowerCostData towerCostDataClone = (TowerCostData) towerAI.towerCostData.Clone();

                    if (towerAI.isAura) continue;

                
                    LockPackage lockPackage =
                        new LockPackage(towerAI.lockedRange, towerAI.lockedFireRate, towerAI.lockedDamage);



                    UpdateStatData(towerAI, lockPackage, towerCostDataClone, true);
                }
        }
        #endif
    }

    private void UpdateTowerValues(TowerAI tower, TowerCostData towerCostData)
    {
        Debug.Log($"{tower.name} damage: {towerCostData.totalDamageCost}, range: {towerCostData.totalRangeCost}, Fire Rate: {towerCostData.totalFireRateCost}");
        tower.startDamage = towerCostData.totalDamage + tower.lockedDamage + tower.freeDamage;
        tower.startingFireRate = towerCostData.totalFireRate + tower.lockedFireRate + tower.freeFireRate;
        tower.startRange = towerCostData.totalRange + tower.lockedRange + tower.freeRange;
    }

    private float SetRangeFromTable(float statCost)
    {
        for (int i = 1; i < _rangeTotalCostMap.Count-1; i++)
        {
            if (_rangeTotalCostMap[i] <= statCost && _rangeTotalCostMap[i + 1] > statCost)
            {
                if (i == 1)
                {
                    return startRangeToken;
                }
                
                return i * rangeStepDistance + startRangeToken;
            }
        }

        return 0f;
    }
    
    private float SetFireRateFromTable(float statCost)
    {
        for (int i = 1; i < _fireRateTotalCostMap.Count-1; i++)
        {
            if (_fireRateTotalCostMap[i] <= statCost && _fireRateTotalCostMap[i + 1] > statCost)
            {
                return i * fireRateStepDistance + startFireRateToken;
            }
        }

        return 0f;
    }
    
    private int SetDamageFromTable(float statCost)
    {
        for (int i = 1; i < _damageTotalCostMap.Count-1; i++)
        {
            if (_damageTotalCostMap[i] <= statCost && _damageTotalCostMap[i + 1] > statCost)
            {
                return i;
            }
        }

        return 0;
    }

    private void UpdateStatData(TowerAI tower, LockPackage lockPackage, TowerCostData evolvingTowerCostData, bool initialTower = false)
    {
       

        tower.lockedDamage = lockPackage.lockedDamage;
        tower.lockedRange = lockPackage.lockedRange;
        tower.lockedFireRate = lockPackage.lockedAttackSpeed;
        
        if (!initialTower)
        {
            evolvingTowerCostData.totalDamageCost += tower.towerCostData.totalDamageCost;
            evolvingTowerCostData.totalRangeCost += tower.towerCostData.totalRangeCost;
            evolvingTowerCostData.totalFireRateCost += tower.towerCostData.totalFireRateCost;
        }
        
        evolvingTowerCostData.totalFireRate = SetFireRateFromTable(evolvingTowerCostData.totalFireRateCost);
        evolvingTowerCostData.totalDamage = SetDamageFromTable(evolvingTowerCostData.totalDamageCost);
        evolvingTowerCostData.totalRange = SetRangeFromTable(evolvingTowerCostData.totalRangeCost);

        tower.towerCostData.totalDamage = evolvingTowerCostData.totalDamage;
        tower.towerCostData.totalRange = evolvingTowerCostData.totalRange;
        tower.towerCostData.totalFireRate = evolvingTowerCostData.totalFireRate;
        
        
        UpdateTowerValues(tower, evolvingTowerCostData);
        if (tower.upgradeVariants.IsNullOrEmpty()) return;

        ContinueUpgrades(tower, lockPackage, evolvingTowerCostData);
    }

    private void ContinueUpgrades(TowerAI tower, LockPackage lockPackage, TowerCostData evolvingTowerCostData)
    {
        if (tower.upgradeVariants.IsNullOrEmpty()) return;
        
        foreach (UpgradeVariant towerAIUpgradeVariant in tower.upgradeVariants)
        {
            if (towerAIUpgradeVariant == null) continue;
            Debug.Log(towerAIUpgradeVariant.name);
            TowerCostData checkPointData = (TowerCostData) evolvingTowerCostData.Clone(); 
            if (towerAIUpgradeVariant.nextUpgrades != null && !towerAIUpgradeVariant.nextUpgrades.IsNullOrEmpty())
            {
                UpdateStatData(towerAIUpgradeVariant, lockPackage,  checkPointData);
            }

            if (towerAIUpgradeVariant.towerUpgrades != null && !towerAIUpgradeVariant.towerUpgrades.IsNullOrEmpty())
            {
                foreach (TowerVariant towerUpgrade in towerAIUpgradeVariant.towerUpgrades)
                {
                    UpdateStatData(towerUpgrade.tower.GetComponent<TowerAI>(), lockPackage, checkPointData);
                }
            }
        }
    }

    private void ContinueUpgrades(UpgradeVariant upgradeVariant, LockPackage lockPackage, TowerCostData towerCostData)
    {
        if (!upgradeVariant.nextUpgrades.IsNullOrEmpty())
            foreach (UpgradeVariant nextUpgrade in upgradeVariant.nextUpgrades.Where(nextUpgrade => nextUpgrade != null))
            {
                TowerCostData checkPointData = (TowerCostData) towerCostData.Clone(); 
                UpdateStatData(nextUpgrade, lockPackage, checkPointData);
            }

        if (!upgradeVariant.towerUpgrades.IsNullOrEmpty())
        {
            foreach (TowerVariant towerUpgrade in upgradeVariant.towerUpgrades)
            {
                TowerCostData checkPointData = (TowerCostData) towerCostData.Clone(); 
                UpdateStatData(towerUpgrade.tower.GetComponent<TowerAI>(), lockPackage, checkPointData);
            }
        }
    }

    
    
    private void UpdateStatData(UpgradeVariant upgrade, LockPackage lockPackage, TowerCostData towerCostData)
    {
        foreach (Stats statList in upgrade.statList)
        {
            if (statList.intStatName == Stats.IntStatName.Damage)
            {
                statList.statValue = SetDamageFromTable(towerCostData.totalDamageCost +  statList.statCost)
                                     - SetDamageFromTable(towerCostData.totalDamageCost);
                towerCostData.totalDamageCost += statList.statCost;
                continue;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (statList.floatStatName == Stats.FloatStatName.Range)
            {
                statList.statValue = SetRangeFromTable(towerCostData.totalRangeCost +  statList.statCost)
                                     - SetRangeFromTable(towerCostData.totalRangeCost);
                towerCostData.totalRangeCost += statList.statCost;
                continue;
            }
            
            if (statList.floatStatName == Stats.FloatStatName.AttackSpeed)
            {
                statList.statValue = SetFireRateFromTable(towerCostData.totalFireRateCost + statList.statCost)
                                     - SetFireRateFromTable(towerCostData.totalFireRateCost);
                towerCostData.totalFireRateCost += statList.statCost;
                continue;
            }
        
        }
        
        ContinueUpgrades(upgrade, lockPackage, towerCostData);
    }
}