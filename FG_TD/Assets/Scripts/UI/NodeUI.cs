using System.Collections;
using System.Collections.Generic;
using System.Text;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    private Node targetNode;
    private TowerAI selectedTower;
    private List<UpgradeVariants> turretUpgrades;
    private List<GameObject> buttons;

    public GameObject sellButton;

    BuildManager buildManager;

    public GameObject buttonPrefab;

    private void Awake()
    {
        HideSellButton();
        CalculateSellPercentageText();
        buttons = new List<GameObject>();
        gameObject.SetActive(false);
        buildManager = BuildManager.instance;
    }

    private void CalculateSellPercentageText()
    {
        TextMeshProUGUI text = sellButton.GetComponentInChildren<TextMeshProUGUI>();

        text.text = $"Sell ({PlayerStats.instance.sellingPercentage}%)";
    }

    public void UIDeselect()
    {
        ui.SetActive(false);
        buildManager.DeselectNode();
        buildManager.DeselectTurret();
        HideSellButton();
    }

    public void SetTarget(Node node)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        ShowSellButton();
        buildManager.DeselectNode();
        ClearButtons();
        targetNode = node;
        selectedTower = node.turret.GetComponent<TowerAI>();
        
        if (selectedTower.GetComponent<Inventory>() != null)
            ItemManager.instance.InvokeInventory(selectedTower.GetComponent<Inventory>());

        if (selectedTower.upgradeVariants != null && selectedTower.upgradeVariants.Count > 0)
            turretUpgrades = selectedTower.upgradeVariants;
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

            if (!upgradeVariant.statList.IsNullOrEmpty())
            {
                GameObject newButton = Instantiate(buttonPrefab, panel.transform, false);

                Button buttonComponent = newButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(delegate { node.UpgradeTower(upgradeVariant); });

                Text text = newButton.GetComponentInChildren<Text>();

                StringBuilder sb = new StringBuilder();
                sb.Append(upgradeVariant.cost + "G|| ");
                foreach (Stats stat in upgradeVariant.statList)
                {
                    sb.Append(stat.intStatName + "+" + stat.statValue + "|| ");
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
                    GameObject newButton = Instantiate(buttonPrefab, panel.transform, false) as GameObject;

                    Button buttonComponent = newButton.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(delegate
                    {
                        node.ReplaceTowerFromPrefab(tower.tower, tower.cost);
                    });

                    StringBuilder sb = new StringBuilder();

                    Text text = newButton.GetComponentInChildren<Text>();

                    TowerAI towerAttr = tower.tower.GetComponent<TowerAI>();

                    //Button text
                    sb.Append("/"
                              + tower.tower.name + " "
                              + tower.cost + "G|| "
                              + towerAttr.startDamage + " damage|| "
                              + towerAttr.startingFireRate + " attackspeed|| "
                              + towerAttr.aoe
                              + " AOE/");


                    text.text = sb.ToString();
                    buttons.Add(newButton);
                }
            }
        }

        ui.SetActive(true);
        targetNode.OnMouseExit();
    }

    public void Hide()
    {
        ui.SetActive(false);
        ItemManager.instance.HideInventory();
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

    private void ShowSellButton()
    {
        CalculateSellPercentageText();
        sellButton.GetComponent<Button>().enabled = true;
    }

    private void HideSellButton()
    {
        sellButton.GetComponent<Button>().enabled = false;
    }

    public void SellTower()
    {
        //Debug.Log($"blurp { targetNode.turret.GetComponent<TowerAI>().totalCost *( (float) PlayerStats.instance.sellingPercentage / 100)}");
        PlayerStats.instance.SpendMoney((int) -(targetNode.turret.GetComponent<TowerAI>().totalCost *
                                                ((float) PlayerStats.instance.sellingPercentage / 100)));
        targetNode.DestroyTurretGroundShit();
        Destroy(targetNode.turret);
        targetNode.turret = null;
        UIDeselect();
    }
}