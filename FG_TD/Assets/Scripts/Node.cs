using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Node : MonoBehaviour
{

    public Color hoverColor;
    private SpriteRenderer rend;
    private Color startColor;

    public Color notEnoughMoneyColor;

    public Color selectedColor;

    private GameObject ui;
    private static string uiTag = "NodeUI";

    [Header("Optional")]
    public GameObject turret;

    BuildManager buildManager;

    private void Awake()
    {
        ui = GameObject.FindGameObjectsWithTag(uiTag)[0];
    }

    private void Start()
    {
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



        // buildManager.BuildOn(this);

    }

    private void OnMouseEnter()
    {
        // if (!buildManager.CanBuild)
        //     return;

        //  if (buildManager.HasMoney)
        //  {
        if (rend.material.color != selectedColor)
            rend.material.color = hoverColor;

        // }
        // else
        // {
        //     rend.material.color = notEnoughMoneyColor;
        // }


    }

    public Vector3 GetBuildPosition()
    {
        return transform.position;
    }

    public void OnMouseExit()
    {
        if (rend.material.color != selectedColor)
            rend.material.color = startColor;
    }

    public void SetNormalColor()
    {
        rend.material.color = startColor;
    }



    public void SetSelected()
    {
        rend.material.color = selectedColor;
    }

    public void UpgrageTower(UpgradeVariants variant)
    {
        if (PlayerStats.Money < variant.cost)
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
            switch (stat.statName)
            {
                case Stats.StatsNames.AttackSpeed:
                    towerAI.fireRate += stat.statValue;
                    break;
                case Stats.StatsNames.AOE:
                    towerAI.aOE += stat.statValue;
                    break;
                case Stats.StatsNames.Damage:
                    towerAI.damage += (int) stat.statValue;
                    break;
                case Stats.StatsNames.range:
                    towerAI.range += stat.statValue;
                    break;
            }
        }

        if (variant.nextUpgrades != null)
            towerAI.upgradeVariants = variant.nextUpgrades;
        Debug.Log("Tower Upgraded～");

        PlayerStats.Money -= variant.cost;

        ui.SetActive(false);
    }

    public void ReplaceTowerFromPrefab(GameObject prefab)
    {
        Destroy(turret.gameObject);
        GameObject currentTurret = Instantiate(prefab, new Vector3(this.transform.position.x, this.transform.position.y, -2), Quaternion.identity);
        turret = currentTurret;
        ui.SetActive(false);
    }

}
