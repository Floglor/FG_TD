﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    private Node target;
    private TowerAI tower;
    List<UpgradeVariants> turretUpgrades;
    List<GameObject> buttons;

    BuildManager buildManager;

    public GameObject buttonPrefab;

    private void Awake()
    {
        buttons = new List<GameObject>();
        gameObject.SetActive(false);
        buildManager = BuildManager.instance;
    }
    
    public void UIDeselect()
    {
        ui.SetActive(false);
        buildManager.DeselectNode();
        buildManager.DeselectTurret();
    }
    public void SetTarget(Node node)
    {
        buildManager.DeselectNode();
        ClearButtons();
        target = node;
        tower = node.turret.GetComponent<TowerAI>();

        if (tower.upgradeVariants != null && tower.upgradeVariants.Count > 0)
            turretUpgrades = tower.upgradeVariants;
        else
        {
            Debug.Log("No buttons in tower " + node.turret.name);
            return;
        }

        Canvas canvas = ui.GetComponentInChildren<Canvas>();
        HorizontalLayoutGroup panel = canvas.GetComponentInChildren<HorizontalLayoutGroup>();


       
        //Upgrade Buttons
        foreach (UpgradeVariants upgradeVariant in turretUpgrades)
        {
            if (upgradeVariant == null) return;

            if (upgradeVariant.statList.Count > 0)
            {
                GameObject newButton = Instantiate(buttonPrefab) as GameObject;

                newButton.transform.SetParent(panel.transform, false);

                Button buttonComponent = newButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(delegate { node.UpgrageTower(upgradeVariant); });

                Text text = newButton.GetComponentInChildren<Text>();

                StringBuilder sb = new StringBuilder();
                sb.Append(upgradeVariant.cost + "G|| ");
                foreach (Stats stat in upgradeVariant.statList)
                {
                    sb.Append(stat.statName + "+" + stat.statValue + "|| ");
                    text.text = sb.ToString();
                }

                buttons.Add(newButton);
            }

        }
       
        //New Tower Buttons
        foreach (UpgradeVariants upgradeVariant in turretUpgrades)
        {
            if (upgradeVariant.towerUpgrades.Count > 0)
            {
                foreach (TowerVariant tower in upgradeVariant.towerUpgrades)
                {
                    GameObject newButton = Instantiate(buttonPrefab) as GameObject;

                    newButton.transform.SetParent(panel.transform, false);

                    Button buttonComponent = newButton.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(delegate { node.ReplaceTowerFromPrefab(tower.tower, tower.cost); });

                    StringBuilder sb = new StringBuilder();

                    Text text = newButton.GetComponentInChildren<Text>();

                    TowerAI towerAttr = tower.tower.GetComponent<TowerAI>();

                    //Button text
                    sb.Append("/" 
                        + tower.tower.name + " " 
                        + tower.cost + "G|| "
                        + towerAttr.damage + " damage|| "
                        + towerAttr.fireRate + " attackspeed|| "
                        + towerAttr.aOE 
                        + " AOE/");

                    

                    text.text = sb.ToString();
                    buttons.Add(newButton);


                }

            }
            

        }

        ui.SetActive(true);
        target.OnMouseExit();
    }

    public void Hide()
    {
        ui.SetActive(false);
        ClearButtons();
    }

    void ClearButtons()
    {
        if (buttons.Count > 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            buttons.Clear();
        }
       
    }
}
