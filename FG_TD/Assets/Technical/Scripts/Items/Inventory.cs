using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Managers;
using MyBox;
using Shooting;
using TMPro;
using UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [MinValue(1)]
    public int capacity;
    public List<Item> items;
    public List<ItemSlot> itemSlots;
   
    public TowerAI tower { get; set; }

    private List<Item> savedItems { get; set; }
    public List<Recipe> possibleRecipes { get; set; }
    private Canvas possibleItemsCanvas;
    private bool initialized;
    

    public int FindFirstFreeSlotIndex()
    {
        foreach (Item item in items.Where(item => item == null))
        {
           // Debug.Log($"returning index {items.IndexOf(item)}"); 
            return items.IndexOf(item);
        } 
        return -1;
    }

    public ItemSlot FindFirstFreeSlot()
    {
        return FindFirstFreeSlotIndex()!= -1 ? itemSlots[FindFirstFreeSlotIndex()] : null;
    }
    public void ApplyEffects(Item item)
    {
        if (!item.floatTowerBuffs.IsNullOrEmpty())
        {
            foreach (FloatTowerBuff itemFloatTowerBuff in item.floatTowerBuffs)
            {
                tower.floatTowerBuffs.Add(new FloatTowerBuff(item.GetInstanceID(), itemFloatTowerBuff.isTimed,
                    itemFloatTowerBuff.isPercentage,
                    itemFloatTowerBuff.isFromAura, itemFloatTowerBuff.floatStat, itemFloatTowerBuff.amount));

                tower.FloatStatRecalculate(itemFloatTowerBuff.floatStat);
            }
        }

        if (!item.integerTowerBuffs.IsNullOrEmpty())
        {
            foreach (IntegerTowerBuff integerTowerBuff in item.integerTowerBuffs)
            {
                tower.integerTowerBuffs.Add(new IntegerTowerBuff(item.GetInstanceID(), integerTowerBuff.isTimed,
                    integerTowerBuff.isPercentage,
                    integerTowerBuff.isFromAura, integerTowerBuff.intStat, integerTowerBuff.amount));

                tower.IntegerStatRecalculate(integerTowerBuff.intStat);
            }
        }

        if (!item.floatTowerDebuffs.IsNullOrEmpty())
        {
            foreach (FloatTowerDebuff itemFloatTowerDebuff in item.floatTowerDebuffs)
            {
                tower.floatTowerDebuffs.Add(new FloatTowerDebuff(item.GetInstanceID(), itemFloatTowerDebuff.isTimed,
                    itemFloatTowerDebuff.isPercentage,
                    itemFloatTowerDebuff.isFromAura, itemFloatTowerDebuff.floatStat, itemFloatTowerDebuff.amount));

                tower.FloatStatRecalculate(itemFloatTowerDebuff.floatStat);
            }
        }

        if (!item.integerTowerDebuffs.IsNullOrEmpty())
        {
            foreach (IntegerTowerDebuff itemIntegerTowerDebuff in item.integerTowerDebuffs)
            {
                tower.integerTowerDebuffs.Add(new IntegerTowerDebuff(item.GetInstanceID(),
                    itemIntegerTowerDebuff.isTimed,
                    itemIntegerTowerDebuff.isPercentage,
                    itemIntegerTowerDebuff.isFromAura, itemIntegerTowerDebuff.intStat,
                    itemIntegerTowerDebuff.amount));

                tower.IntegerStatRecalculate(itemIntegerTowerDebuff.intStat);
            }
        }

        if (!item.falseCrits.IsNullOrEmpty())
        {
            foreach (FalseCrit itemFalseCrit in item.falseCrits)
            {
                tower.falseCrits.Add(itemFalseCrit);
            }
        }
    }

    public void RemoveEffects(Item item)
    {
        if (item == null) return;
        if (!item.floatTowerBuffs.IsNullOrEmpty())
        {
            foreach (FloatTowerBuff itemFloatTowerBuff in item.floatTowerBuffs)
            {
                for (int i = 0; i < tower.floatTowerBuffs.Count; i++)
                {
                    if (tower.floatTowerBuffs[i].instanceId != item.GetInstanceID() ||
                        tower.floatTowerBuffs[i].floatStat != itemFloatTowerBuff.floatStat) continue;
                    
                    tower.floatTowerBuffs.RemoveAt(i);
                        
                    tower.FloatStatRecalculate(itemFloatTowerBuff.floatStat);
                }
            }
        }

        if (!item.integerTowerBuffs.IsNullOrEmpty())
        {
            foreach (IntegerTowerBuff integerItemBuff in item.integerTowerBuffs)
            {
                for (int i = 0; i < tower.integerTowerBuffs.Count; i++)
                {
                    
                    if (tower.integerTowerBuffs[i].instanceId != item.GetInstanceID() ||
                        tower.integerTowerBuffs[i].intStat != integerItemBuff.intStat) continue;
                    
                    tower.integerTowerBuffs.RemoveAt(i);
                    tower.IntegerStatRecalculate(integerItemBuff.intStat);
                }
            }
        }

        if (!item.floatTowerDebuffs.IsNullOrEmpty())
        {
            foreach (FloatTowerDebuff floatTowerDebuff in item.floatTowerDebuffs)
            {
                for (int i = 0; i < tower.floatTowerDebuffs.Count; i++)
                {
                    if (tower.floatTowerDebuffs[i].instanceId != item.GetInstanceID() ||
                        tower.floatTowerDebuffs[i].floatStat != floatTowerDebuff.floatStat) continue;
                    
                    tower.floatTowerDebuffs.RemoveAt(i);
                    tower.FloatStatRecalculate(floatTowerDebuff.floatStat);
                }
            }
        }

        if (!item.integerTowerDebuffs.IsNullOrEmpty())
        {
            foreach (IntegerTowerDebuff intItemDebuff in item.integerTowerDebuffs)
            {
                for (int i = 0; i < tower.integerTowerDebuffs.Count; i++)
                {
                    if (tower.integerTowerDebuffs[i].instanceId != item.GetInstanceID() ||
                        tower.integerTowerDebuffs[i].intStat != intItemDebuff.intStat) continue;

                    tower.integerTowerDebuffs.RemoveAt(i);
                        
                    tower.IntegerStatRecalculate(intItemDebuff.intStat);
                }
            }
        }

        if (!item.falseCrits.IsNullOrEmpty())
        {
            foreach (FalseCrit itemFalseCrit in item.falseCrits)
            {
                for (int i = 0; i < tower.falseCrits.Count; i++)
                {
                    if (itemFalseCrit.itemID == tower.falseCrits[i].itemID)
                    {
                        tower.falseCrits.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    public void Start()
    {
        if (initialized) return;
        itemSlots = new List<ItemSlot>();
        items = new List<Item>();
        possibleRecipes = new List<Recipe>();
        tower = transform.GetComponentInParent<TowerAI>();
        
        ItemManager.instance.allInventories.Add(this);

        for (int i = 0; i < capacity; i++)
        {
            items.Add(null);
        }

        possibleItemsCanvas = ItemManager.instance.possibleItemsCanvas;
        initialized = true;
    }

    public void Remove()
    {
        ItemManager.instance.allInventories.Remove(this);
    }

    public void CheckForPossibleRecipes()
    {
        ClearPossibleItems();
        //if (capacity <= GetItemsCount()) return;

        if (items.Count == 1) return;

        savedItems = new List<Item>();

        foreach (Recipe recipe in ItemManager.instance.recipes)
        {
            if (recipe.combination.Count <= items.Count)
            {
                bool combinationFound = true;
                foreach (Item recipeItem in recipe.combination)
                {
                    if (!items.Contains(recipeItem))
                    {
                        combinationFound = false;
                        break;
                    }
                }

                if (!combinationFound) continue;
                
                if (!possibleRecipes.Contains(recipe) && IsEnoughItems(recipe, items))
                    possibleRecipes.Add(recipe);
            }
        }


        //Debug.Log(possibleItems.Count);
        ItemManager.instance.GICreatePossibleItemsButtons();
    }

    private bool IsEnoughItems(Recipe recipe, List<Item> inventoryItems)
    {
        Dictionary<string, int> itemMap = new Dictionary<string, int>();
        foreach (Item recipeItem in recipe.combination)
        {
            if (itemMap.ContainsKey(recipeItem.name))
            {
                itemMap[recipeItem.name]++;
            }
            else
            {
                itemMap.Add(recipeItem.name, 1);
            }
        }


        return !(from keyValuePair in itemMap
            let count = inventoryItems.Where(inventoryItem => inventoryItem != null).Count(inventoryItem => inventoryItem.name == keyValuePair.Key)
            where keyValuePair.Value > count select keyValuePair).Any();
    }

    private int GetItemsCount()
    {
        return items.Count(item => item != null);
    }

    public void DeleteItem(int itemID)
    {
        //Debug.Log($"Item count: {GetItemsCount()}");

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) continue;
            if (itemID != items[i].itemID) continue;


           // Debug.Log($"*E*: {i}");

            GameObject instanceInventoryPanel = ItemManager.instance.inventoryPanels[i];

            ItemSlot itemSlot = instanceInventoryPanel.GetComponent<ItemSlot>();


            if (itemSlot.item != null && itemSlot.item.itemID == itemID)
            {
                if (instanceInventoryPanel.GetAllChildren().IsNullOrEmpty())
                {
                    Debug.LogError("DELETE ITEM: instanceInventoryPanel: NO CHILDREN");
                }

                //   Debug.Log(
                //       $"deleting item{instanceInventoryPanel.GetAllChildren()[0].GetComponent<ItemDragNDrop>().item}");
                foreach (Transform child in instanceInventoryPanel.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            RemoveEffects(items[i]);
            items[i] = null;
            itemSlot.item = null;
            break;
        }
    }

    public void ClearPossibleItems()
    {
        possibleRecipes.Clear();
        ItemManager.instance.ClearPossibleItemsButtons();
    }
}