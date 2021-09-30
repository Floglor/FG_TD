using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public static NodeUI instance;
    public GameObject ui;
    private Node targetNode;
    private TowerAI selectedTower;
    private List<UpgradeVariant> turretUpgrades;
    private List<GameObject> buttons;
    public GameObject timeControlCircles;
    public HorizontalLayoutGroup upgradesLayoutGroup;

    public GameObject sellButton;

    BuildManager buildManager;
    public GameObject towerInfo;
    public GameObject mainStatsGO;
    public GameObject buttonPrefab;
    public GameObject smallStatList;
    public GameObject fullStatList;
    public GameObject TMPAsset;

    public Transform bigStatParentLeft;
    public Transform bigStatParentRight;

    private List<GameObject> statNamesListLarge;
    private List<GameObject> statValuesListLarge;

    public ItemDescription mainItemDescription;

    public TooltipTextCancelArea textCancelArea;

    public bool hidingTime;

    private void Awake()
    {
        instance = this;
        timeControlCircles.SetActive(true);
        HideSellButton();
        CalculateSellPercentageText();
        buttons = new List<GameObject>();
        gameObject.SetActive(false);
        buildManager = BuildManager.instance;
        mainItemDescription.Hide();
    }

    private void CalculateSellPercentageText()
    {
        TextMeshProUGUI text = sellButton.GetComponentInChildren<TextMeshProUGUI>();

        text.text = PlayerStats.instance.sellPoints < 1 ? "Sell" : $"Sell ({PlayerStats.instance.sellingPercentage}%)";
    }

    public void UIDeselect()
    {
        HideNodeUI();
        buildManager.DeselectNode();
        buildManager.DeselectTurret();
        HideSellButton();
    }

    public void HideNodeUI()
    {
        ui.SetActive(false);
        towerInfo.SetActive(false);
    }

    public void UpdateTowerInfo(TowerAI tower)
    {
        GameObject statNames = mainStatsGO.transform.GetChild(0).gameObject;
        GameObject statValues = mainStatsGO.transform.GetChild(1).gameObject;
    }

    private void SetStatsPanels(TowerAI towerAI)
    {
        Utils.InitiateSmallStatList(towerAI, smallStatList);

        List<GameObject> fullStatChildren = fullStatList.gameObject.GetAllChildren();


        GameObject statNames = fullStatChildren[0];
        GameObject statValues = fullStatChildren[1];

        statNamesListLarge = statNames.GetAllChildren();

        if (statNamesListLarge.Count > 0)
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < statNamesListLarge.Count; i++)
            {
                Destroy(statNamesListLarge[i].gameObject);
            }

        statNamesListLarge.Clear();

        statValuesListLarge = statValues.GetAllChildren();

        if (statValuesListLarge.Count > 0)
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < statValuesListLarge.Count; i++)
            {
                Destroy(statValuesListLarge[i].gameObject);
            }

        statValuesListLarge.Clear();

        if (towerAI.typeSmallStatList == TowerTypeSmallStatList.ProjectileTower)
        {
            //Basic Stats
            SetStatText(StringNames.DamageModifier, towerAI.damageModifier.ToString());
            SetStatText(StringNames.Damage,
                $"{towerAI.damage}/<color=red>{towerAI.penetration}</color>/<color=blue>{towerAI.disruption}</color>");
            SetStatText(StringNames.AttackSpeed, towerAI.fireRate.ToString(Utils.FloatFormat));
            SetStatText(StringNames.BulletSpeed, towerAI.bulletSpeed.ToString(Utils.FloatFormat));
            SetStatText(StringNames.TargetPriority, Enum.GetName(typeof(TargetPriority), towerAI.targetPriority));
        }
    }

    private void SetStatText(string title, string value)
    {
        statNamesListLarge.Add(InitiateTMP(title, true));
        statValuesListLarge.Add(InitiateTMP(value, false));
    }

    private GameObject InitiateTMP(string value, bool isLeft)
    {
        if (value == null)
            return null;

        GameObject newTMP = Instantiate(TMPAsset, isLeft ? bigStatParentLeft : bigStatParentRight);
        newTMP.GetComponent<TextMeshProUGUI>().text = value;
        if (isLeft)
        {
            newTMP.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        }

        return newTMP;
    }


    public void SetTarget(Node node)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        ActivateUI();

        ShowSellButton();
        buildManager.DeselectNode();
        ClearButtons();
        targetNode = node;
        selectedTower = node.turret.GetComponent<TowerAI>();
        selectedTower.isTargeted = true;

        SetStatsPanels(selectedTower);
        ItemManager.instance.InvokeInventory(selectedTower.GetComponent<Inventory>() != null
            ? selectedTower.GetComponent<Inventory>()
            : null);

        if (selectedTower.upgradeVariants != null && selectedTower.upgradeVariants.Count > 0)
            turretUpgrades = selectedTower.upgradeVariants;
        else
        {
            Debug.Log("No buttons in tower " + node.turret.name);
            return;
        }

        // Canvas canvas = ui.GetComponentInChildren<Canvas>();
        // HorizontalLayoutGroup panel = canvas.GetComponentInChildren<HorizontalLayoutGroup>();


        //Upgrade Buttons
        foreach (UpgradeVariant upgradeVariant in turretUpgrades)
        {
            if (upgradeVariant == null) return;

            if (!upgradeVariant.statList.IsNullOrEmpty())
            {
                GameObject newButton = Instantiate(buttonPrefab, upgradesLayoutGroup.transform, false);

                Button buttonComponent = newButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(delegate { node.UpgradeTower(upgradeVariant); });

                Transform nameTransform = newButton.transform.GetChild(1);
                TextMeshProUGUI name = nameTransform.GetComponent<TextMeshProUGUI>();

                Transform priceTransform = newButton.transform.GetChild(2);
                TextMeshProUGUI price = priceTransform.GetComponent<TextMeshProUGUI>();

                StringBuilder nameSb = new StringBuilder();
                StringBuilder priceSb = new StringBuilder();

                priceSb.Append(upgradeVariant.cost + " es");
                foreach (Stats stat in upgradeVariant.statList)
                {
                    if (stat.intStatName != Stats.IntStatName.None)
                        nameSb.Append(stat.intStatName + " + " + stat.statValue + "\n");
                    if (stat.floatStatName != Stats.FloatStatName.None)
                        nameSb.Append(stat.floatStatName + " + " + stat.statValue + "\n");
                    if (stat.boolStatName != Stats.BoolStatName.None)
                        nameSb.Append(stat.boolStatName + " + " + stat.statValue + "\n");

                    name.text = nameSb.ToString();
                    price.text = priceSb.ToString();
                }

                buttons.Add(newButton);
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) towerInfo.transform);
            }
        }

        //New Tower Buttons
        foreach (UpgradeVariant upgradeVariant in turretUpgrades)
        {
            if (upgradeVariant.towerUpgrades.Count > 0)
            {
                foreach (TowerVariant tower in upgradeVariant.towerUpgrades)
                {
                    GameObject newButton =
                        Instantiate(buttonPrefab, upgradesLayoutGroup.transform, false) as GameObject;

                    Button buttonComponent = newButton.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(delegate
                    {
                        node.ReplaceTowerFromPrefab(tower.tower, tower.cost);
                    });

                    StringBuilder sb = new StringBuilder();

                    TextMeshProUGUI text = newButton.GetComponentInChildren<TextMeshProUGUI>();

                    TowerAI towerAttr = tower.tower.GetComponent<TowerAI>();

                    //Button text
                    sb.Append("/"
                              + tower.tower.name + " "
                              + tower.cost + "G || "
                              + towerAttr.startDamage + " damage || "
                              + towerAttr.startingFireRate + " attackspeed || "
                              + towerAttr.aoe
                              + " AOE/");


                    text.text = sb.ToString();
                    buttons.Add(newButton);
                }
            }
        }

        targetNode.OnMouseExit();
    }

    private void ActivateUI()
    {
        ui.SetActive(true);
        towerInfo.SetActive(true);
        Shop.instance.Hide();
        HideTime();
    }

    public void HideTime()
    {
        if (hidingTime)
            timeControlCircles.SetActive(false);
    }

    public void ShowTime()
    {
        timeControlCircles.SetActive(true);
    }

    public void Hide()
    {
        HideNodeUI();
        ShowTime();
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
        sellButton.GetComponent<Button>().interactable = true;
    }

    private void HideSellButton()
    {
        sellButton.GetComponent<Button>().interactable = false;
    }

    public void SellTower()
    {
        //Debug.Log($"blurp { targetNode.turret.GetComponent<TowerAI>().totalCost *( (float) PlayerStats.instance.sellingPercentage / 100)}");
        if (PlayerStats.instance.sellPoints < 1)
            PlayerStats.instance.SpendMoney((int) -(targetNode.turret.GetComponent<TowerAI>().totalCostRun *
                                                    ((float) PlayerStats.instance.sellingPercentage / 100)));
        else
        {
            PlayerStats.instance.SpendMoney(-targetNode.turret.GetComponent<TowerAI>().totalCostRun);
            PlayerStats.instance.sellPoints -= 1;
        }

        Inventory inventory = targetNode.turret.gameObject.GetComponent<Inventory>();
        if (inventory != null)
        {
        }

        targetNode.DestroyTurretGroundShit();
        Destroy(targetNode.turret);
        targetNode.turret = null;
        targetNode.SetNormalColor();
        // targetNode.ResetGraphics();
        targetNode.ResetGraphics();
        UIDeselect();
    }
}