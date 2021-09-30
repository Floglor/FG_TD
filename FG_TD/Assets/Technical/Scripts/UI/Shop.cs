using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public GameObject selectedTowerRune;
    public GameObject runeChoiceParent;
    public GameObject lines;
    public GameObject deactivableSeparator;
    public GameObject fullStatList;
    public GameObject shop;

    public Transform bigStatParentLeft;
    public Transform bigStatParentRight;

    private List<GameObject> statNamesListLarge;
    private List<GameObject> statValuesListLarge;
    private List<GameObject> runeChoiceChildren;
    private GameObject selectedTower;

    public List<Sprite> choiceLinesSprites;
    public List<GameObject> initialTowers;

    public GameObject TMPAsset;

    BuildManager buildManager;

    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            Hide();
        }
    }
    
    
    private void Awake()
    {
        instance = this;
        buildManager = BuildManager.instance;
    }

    private void Start()
    {
        buildManager = BuildManager.instance;
        HideRunesAndStats();
        runeChoiceChildren = runeChoiceParent.GetAllChildren();
        List<GameObject> shopChildren = shop.GetAllChildren();


        for (int i = 0; i < shopChildren.Count; i++)
        {
            int i1 = i;
            shopChildren[i].GetComponent<Button>().onClick.AddListener(delegate { SelectPinar(initialTowers[i1]); });
        }
    }


    public void ShowShop()
    {
        NodeUI.instance.HideNodeUI();
        NodeUI.instance.HideTime();
        Show();
        shop.SetActive(true);
        HideRunesAndStats();
    }

    private void ShowRunesAndStats()
    {
        selectedTowerRune.SetActive(true);
        runeChoiceParent.SetActive(true);
        lines.SetActive(true);
        deactivableSeparator.SetActive(true);
    }

    private void HideRunesAndStats()
    {
        selectedTowerRune.SetActive(false);
        runeChoiceParent.SetActive(false);
        lines.SetActive(false);
        deactivableSeparator.SetActive(false);
    }

    public List<TowerVariant> SearchForTowerUpgrades(List<UpgradeVariant> towerUpgradeVariants)
    {
        List<TowerVariant> towerVariants = new List<TowerVariant>();

        foreach (UpgradeVariant t in towerUpgradeVariants)
        {
            FindAllUpgrades(t, towerVariants);
        }

        return towerVariants;
    }

    private void FindAllUpgrades(UpgradeVariant towerUpgradeVariant, List<TowerVariant> towerVariants)
    {
        UpgradeVariant currentVariant = towerUpgradeVariant;

        if (currentVariant == null) return;

        if (currentVariant.towerUpgrades.Count > 0)
        {
            towerVariants.AddRange(currentVariant.towerUpgrades);
        }

        if (!currentVariant.nextUpgrades.IsNullOrEmpty())
        {
            foreach (UpgradeVariant currentVariantNextUpgrade in currentVariant.nextUpgrades)
            {
                FindAllUpgrades(currentVariantNextUpgrade, towerVariants);
            }
        }
    }

    public void SelectPinar(GameObject tower)
    {
        if (tower == null)
        {
            Debug.Log("SelectPinar: Tower is null");
            return;
        }

        selectedTower = tower;
        ShowRunesAndStats();
        CloseShop();
        TowerAI towerAI = tower.GetComponent<TowerAI>();
        if (towerAI == null) return;

        List<UpgradeVariant> towerAIUpgradeVariants = towerAI.upgradeVariants;
        List<TowerVariant> towerVariants = new List<TowerVariant>();

        towerVariants = SearchForTowerUpgrades(towerAIUpgradeVariants);

        // LogTowers(towerVariants);


        switch (towerVariants.Count)
        {
            case 0:
                HideTowerChoice();
                break;
            case 1:
                ShowTowerChoice();
                lines.GetComponent<SpriteRenderer>().sprite = choiceLinesSprites[0];
                SetDeactivatedRuneChildren(3);
                break;
            case 2:
                ShowTowerChoice();
                lines.GetComponent<SpriteRenderer>().sprite = choiceLinesSprites[1];
                SetDeactivatedRuneChildren(2);
                break;
            case 3:
                ShowTowerChoice();
                lines.GetComponent<SpriteRenderer>().sprite = choiceLinesSprites[2];
                SetDeactivatedRuneChildren(1);
                break;
            case 4:
                ShowTowerChoice();
                lines.GetComponent<SpriteRenderer>().sprite = choiceLinesSprites[3];
                SetDeactivatedRuneChildren(0);
                break;
        }


        selectedTowerRune.GetComponentInChildren<Image>().sprite = tower.GetComponent<SpriteRenderer>().sprite;
        // selectedTowerRune.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
        selectedTowerRune.GetComponentInChildren<TextMeshProUGUI>().text = tower.name;


        if (towerVariants.Count > 0)
            for (int i = 0; i < towerVariants.Count; i++)
            {
                GameObject runeChoiceChild = runeChoiceChildren[i];

                int i1 = i;
                if (towerVariants[i1].tower == null) continue;
                runeChoiceChild.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    SelectPinar(towerVariants[i1].tower);
                });

                if (runeChoiceChild.gameObject.activeInHierarchy)
                {
                    runeChoiceChild.transform.GetChild(1).GetComponent<Image>().sprite =
                        towerVariants[i].tower.GetComponent<SpriteRenderer>().sprite;

                    // runeChoiceChild.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;

                    runeChoiceChild.GetComponentInChildren<TextMeshProUGUI>().text =
                        towerVariants[i].tower.name;
                }
            }
    }

    private static void LogTowers(List<TowerVariant> towerVariants)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (TowerVariant towerVariant in towerVariants)
        {
            stringBuilder.Append(towerVariant.tower.name);
        }

        Debug.Log(stringBuilder.ToString());
    }

    private void SetDeactivatedRuneChildren(int howMany)
    {
        foreach (GameObject runeChoiceChild in runeChoiceChildren)
        {
            runeChoiceChild.SetActive(true);
        }

        if (howMany == 0) return;
        int negativeChildrenIterator = 3;
        for (int i = howMany; i > 0; i--)
        {
            runeChoiceChildren[negativeChildrenIterator].SetActive(false);
            negativeChildrenIterator--;
        }
    }

    private void HideTowerChoice()
    {
        //selectedTowerRune.SetActive(false);
        runeChoiceParent.SetActive(false);
        lines.SetActive(false);
    }

    private void ShowTowerChoice()
    {
        selectedTowerRune.SetActive(true);
        runeChoiceParent.SetActive(true);
        lines.SetActive(true);
    }

    private void CloseShop()
    {
        shop.SetActive(false);
    }

    public void Buy()
    {
        if (selectedTower == null)
        {
            Debug.LogError("No tower selected");
            return;
        }


        buildManager.SetTurretToBuild(new TurretBlueprint(selectedTower,
            selectedTower.GetComponent<TowerAI>().totalCostOnStart));
        Hide();
    }


    public void Hide()
    {
        NodeUI.instance.ShowTime();
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
        if (buildManager != null)
            buildManager.DeselectNode();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(true);
    }


    private void SetStatsPanels(TowerAI towerAI)
    {
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
}