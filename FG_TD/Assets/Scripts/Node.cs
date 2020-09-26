using System;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using Items;
using Managers;
using Shooting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Alignment
{
    Vertical,
    Horizontal,
    Corner
}

public enum Corner
{
    Upper,
    Lower
}


public class Node : MonoBehaviour
{
    public Alignment alignment;

    [ConditionalField(nameof(alignment), false, Alignment.Corner)]
    public Corner cornerAlignment;

    [ConditionalField(nameof(alignment), false, Alignment.Corner)]
    public bool isSpecial;

    public bool isExternal;

    public Color towerColor;
    [ColorUsage(true, true)] public Color hoverColor;
    private SpriteRenderer rend;
    
    [ColorUsage(true, true)] private Color startColor;

    [ColorUsage(true, true)] public Color selectedColor;

    public bool isSelected;

    public GameObject ui;

    private NodeUI uiScript;
    //private static string uiTag = "NodeUI";

    [Header("Optional")] public GameObject turret;
    public GameObject upgradeMark;

    BuildManager buildManager;

    public bool towerColorSet;

    //private float standartIntensity = 3.2f;


    private void Awake()
    {
        //  ui = GameObject.FindGameObjectsWithTag(uiTag)[0];
        uiScript = ui.GetComponent<NodeUI>();
    }

    private void Start()
    {
        towerColorSet = false;
        buildManager = BuildManager.instance;
        rend = GetComponent<SpriteRenderer>();
        startColor = rend.material.color;
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (turret != null)
        {
            buildManager.SelectTurret(this);
            return;
        }
        else
        {
            buildManager.SelectNode(this);
        }
    }

    public void UpdateTowerNode()
    {
        TowerAI towerAI = turret.GetComponent<TowerAI>();
        towerAI.motherNode = this;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (rend.material.color != selectedColor)
        {
            ChangeColorTo(hoverColor);
        }
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position;
    }

    public void OnMouseExit()
    {
       SetNormalColor();
    }

    public void SetNormalColor()
    {
        if (isSelected) return;

        if (!towerColorSet)
        {
            ChangeColorTo(startColor);
        }
        else
        {
            ChangeColorTo(towerColor);
        }
    }

    private void ChangeColorTo(Color color)
    {
        rend.material.color = color;
        rend.color = color;
        Color rendColor = rend.color;
        rendColor.a = 100;
        // ReSharper disable once Unity.InefficientPropertyAccess
        rend.color = rendColor;
    }

    public void SetTowerColor()
    {
        towerColorSet = true;
        towerColor = turret.gameObject.GetComponent<Renderer>().material.color;
        ChangeColorTo(towerColor);
        //Debug.Log(rend.material.color);
    }


    public void SetSelected()
    {
        isSelected = true;
        rend.material.color = selectedColor;
    }

    public void UpgradeTower(UpgradeVariants variant)
    {
        if (!PlayerStats.instance.SpendMoney(variant.cost))
        {
            Debug.Log("No money");
            return;
        }

        TowerAI towerAI;
        if (turret.GetComponent<TowerAI>() != null)
            towerAI = turret.GetComponent<TowerAI>();
        else
        {
            Debug.LogError("ReplaceTower prefab is not tower!");
            return;
        }

        foreach (Stats stat in variant.statList)
        {
            switch (stat.intStatName)
            {
                case Stats.IntStatName.Damage:
                    towerAI.startDamage += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.Damage);
                    break;
                case Stats.IntStatName.PenetrateAmount:
                    towerAI.isPenetrative = true;
                    towerAI.startPenetration += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.PenetrateAmount);
                    break;
                case Stats.IntStatName.SplitshotTargetsCount:
                    towerAI.startSplitshotTargetCount += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.SplitshotTargetsCount);
                    break;
                case Stats.IntStatName.ChainLength:
                    towerAI.startChainLength += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.ChainLength);
                    break;
                case Stats.IntStatName.SnaringStrength:
                    towerAI.startSnaringStrength += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.SnaringStrength);
                    break;
                case Stats.IntStatName.DOTTickDamage:
                    towerAI.startDOTDamage += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.DOTTickDamage);
                    break;
                case Stats.IntStatName.DOTSlowPercentage:
                    towerAI.startDotSlowRate += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.DOTSlowPercentage);
                    break;
                case Stats.IntStatName.DOTPenetrativeDamage:
                    towerAI.startingDOTPenetration += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.DOTPenetrativeDamage);
                    break;
                case Stats.IntStatName.SlowingAuraPercentage:
                    towerAI.startDotSlowRate += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.SlowingAuraPercentage);
                    break;
                case Stats.IntStatName.Disruption:
                    towerAI.startDisruption += (int) stat.statValue;
                    towerAI.IntegerStatRecalculate(Stats.IntStatName.Disruption);
                    break;
                case Stats.IntStatName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (stat.floatStatName)
            {
                case Stats.FloatStatName.AttackSpeed:
                    towerAI.startingFireRate += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.AttackSpeed);
                    break;
                case Stats.FloatStatName.Aoe:
                    towerAI.startAOE += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.Aoe);
                    break;
                case Stats.FloatStatName.Range:
                    towerAI.startRange += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.Range);
                    break;
                case Stats.FloatStatName.AoeProjectileSpeed:
                    towerAI.startingLinearTravelSpeed += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.AoeProjectileSpeed);
                    break;
                case Stats.FloatStatName.AoeProjectileTravelRange:
                    towerAI.startLinearTravelDistance += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.AoeProjectileTravelRange);
                    break;
                case Stats.FloatStatName.ChainSeekRadius:
                    towerAI.startChainSeekRadius += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.ChainSeekRadius);
                    break;
                case Stats.FloatStatName.AuraRadius:
                    towerAI.startAuraRadius += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.AuraRadius);
                    break;
                case Stats.FloatStatName.DotDuration:
                    towerAI.startDotDuration += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.DotDuration);
                    break;
                case Stats.FloatStatName.DOTTickInterval:
                    towerAI.startDebuffTickFrequency += stat.statValue;
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.DOTTickInterval);
                    break;
                case Stats.FloatStatName.DOTChangingIdentifier:
                    towerAI.dOTUniqueIdentifier = string.Concat(towerAI.dOTUniqueIdentifier, stat.statValue);
                    towerAI.FloatStatRecalculate(Stats.FloatStatName.DOTChangingIdentifier);
                    break;
                case Stats.FloatStatName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (variant.nextUpgrades != null)
            towerAI.upgradeVariants = variant.nextUpgrades;
        Debug.Log("Tower Upgraded～");

        towerAI.totalCost += variant.cost;

        //Debug.Log(towerAI.totalCost);

        SpawnUpgradeMark();


        uiScript.UIDeselect();
    }


    private void SpawnUpgradeMark()
    {
        Canvas canvas = turret.GetComponentInChildren<Canvas>();
        HorizontalLayoutGroup panel = canvas.GetComponentInChildren<HorizontalLayoutGroup>();

        GameObject upgradeMark_ = Instantiate(upgradeMark, panel.transform, false) as GameObject;
    }

    public void ReplaceTowerFromPrefab(GameObject prefab, int cost)
    {
        if (!PlayerStats.instance.SpendMoney(cost))
        {
            Debug.Log("Not enough money");
            return;
        }

        TowerAI towerAi = turret.GetComponent<TowerAI>();
        Inventory inventory = towerAi.gameObject.GetComponent<Inventory>();

        DestroyTurretGroundShit();

        GameObject currentTurret = Instantiate(prefab,
            new Vector3(this.transform.position.x, this.transform.position.y, -2), Quaternion.identity);

        currentTurret.GetComponent<TowerAI>().totalCost = towerAi.totalCost + cost;
        
       
        if (inventory != null)
        {
            inventory.Remove();
            foreach (Item inventoryItem in inventory.items)
            {
                try
                {
                    if (currentTurret.GetComponent<Inventory>().items == null) continue;

                    currentTurret.GetComponent<Inventory>().Start();
                    int freeItemIndex = currentTurret.GetComponent<Inventory>().FindFirstFreeSlot();

                    if (freeItemIndex != -1)
                    {
                        currentTurret.GetComponent<Inventory>().items[freeItemIndex] = inventoryItem;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        else
        {
            Debug.Log("inventory is null");
        }

        ItemManager.instance.ClearInventoryPanels();

        PlayerStats.instance.towers.Remove(towerAi);
        Destroy(turret.gameObject);

        turret = currentTurret;

        //Debug.Log(turret.GetComponent<TowerAI>().totalCost);

        SetTowerColor();

        uiScript.UIDeselect();
        UpdateTowerNode();
    }

    public void DestroyTurretGroundShit()
    {
        TowerAI towerAi = turret.GetComponent<TowerAI>();
        if (turret.GetComponent<TowerAI>().isGroundType)
        {
            foreach (GroundProjectile t in towerAi.groundProjectiles)
            {
                Destroy(t.gameObject);
            }
        }
    }
}