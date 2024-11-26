using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using JetBrains.Annotations;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{

    public GameObject statList;
    public GameObject itemImage;
    
    public GameObject scrollView;
    public TextMeshProUGUI scrollText;
    
    public TextMeshProUGUI itemName;
    
    private List<GameObject> statNamesListLarge;
    private List<GameObject> statValuesListLarge;

    public Transform bigStatParentLeft;
    

    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            Hide();
        }
    }
    

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public Transform bigStatParentRight;

    [NotNull]
    public Item SelectedItem
    {
        get => selectedItem;
        set
        {
            if (value == null) throw new ArgumentNullException("SELECTED ITEM IS NULL!");
            selectedItem = value;
            SetItemDescription();
        }
    }

    private Item selectedItem;

    private void SetItemDescription()
    {
        List<GameObject> fullStatChildren = statList.gameObject.GetAllChildren();
        
        GameObject statNames = fullStatChildren[0];
        GameObject statValues = fullStatChildren[1];
        
        statNamesListLarge = statNames.GetAllChildren();

        itemName.text = selectedItem.name;
        itemImage.GetComponent<SpriteRenderer>().sprite = selectedItem.itemImage;
        
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

        foreach (FloatTowerBuff selectedItemFloatTowerBuff in selectedItem.floatTowerBuffs)
        {
            SetStatText(Enum.GetName(typeof(Stats.FloatStatName), selectedItemFloatTowerBuff.floatStat), $"+{selectedItemFloatTowerBuff.amount:0.00}{ReturnPercentageIfNeeded(selectedItemFloatTowerBuff)}");
        }

        foreach (FloatTowerDebuff selectedItemFloatTowerBuff in selectedItem.floatTowerDebuffs)
        {
            SetStatText(Enum.GetName(typeof(Stats.FloatStatName), selectedItemFloatTowerBuff.floatStat), $"-{selectedItemFloatTowerBuff.amount:0.00}{ReturnPercentageIfNeeded(selectedItemFloatTowerBuff)}");
        }

        foreach (IntegerTowerBuff selectedItemIntTowerBuff in selectedItem.integerTowerBuffs)
        {
            SetStatText(Enum.GetName(typeof(Stats.IntStatName), selectedItemIntTowerBuff.intStat), $"+##phys:{selectedItemIntTowerBuff.amount}{ReturnPercentageIfNeeded(selectedItemIntTowerBuff)}");
        }
        
        foreach (IntegerTowerDebuff selectedItemIntTowerBuff in selectedItem.integerTowerDebuffs)
        {
            SetStatText(Enum.GetName(typeof(Stats.IntStatName), selectedItemIntTowerBuff.intStat), $"-{selectedItemIntTowerBuff.amount}{ReturnPercentageIfNeeded(selectedItemIntTowerBuff)}");
        }

        if (!selectedItem.falseCrits.IsNullOrEmpty())
        {
            scrollView.gameObject.SetActive(true);
            scrollText.text = Utils.MakeFalseCritDescription(selectedItem.falseCrits);
        }
        else
        {
            scrollView.gameObject.SetActive(false);
        }
    }

    private static string ReturnPercentageIfNeeded(Modifier floatTowerBuff)
    {
        return floatTowerBuff.isPercentage ? "%" : "";
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

        GameObject newTMP = Instantiate(NodeUI.instance.TMPAsset, isLeft ? bigStatParentLeft : bigStatParentRight);
        newTMP.GetComponent<TextMeshProUGUI>().text = value;
        if (isLeft)
        {
            newTMP.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        }

        return newTMP;
    }
   
}
