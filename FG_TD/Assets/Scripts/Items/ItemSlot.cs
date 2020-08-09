using System.Collections;
using System.Collections.Generic;
using Items;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private ItemManager itemManager;
    public Inventory inventory;
    public bool isInventorySlot;
    private GameObject itemPrefab;
    public Item item;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        itemManager = ItemManager.instance;
        itemPrefab = itemManager.itemPrefab;


        if (itemManager.freeItemSlots.Contains(gameObject))
        {
            isInventorySlot = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        AddItem(eventData.pointerDrag);

        eventData.pointerDrag.transform.SetParent(transform);

        eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
            GetComponent<RectTransform>().anchoredPosition;

        eventData.pointerDrag.GetComponent<ItemDragNDrop>().itemSlot = this;

        if (inventory != null && isInventorySlot)
        {
            inventory.CheckForPossibleRecipes();
        }
    }


    public void AddItem(GameObject _item)
    {
        item = _item.GetComponent<ItemDragNDrop>().item;
        if (!isInventorySlot)
        {
            ItemManager.instance.freeItems[index] = item;
        }
        else
        {
            if (inventory != null)
            {
                inventory.items[index] = item;
                inventory.ApplyEffects(item);
            }
        }

        //   Debug.Log("item added");
    }

    public void RemoveItem()
    {
        if (!isInventorySlot)
        {
            itemManager.freeItems[index] = null;
        }
        else
        {
            inventory.RemoveEffects(item);
            inventory.items[index] = null;
        }
        
        item = null;

        // Debug.Log("item removed");

        if (!isInventorySlot) return;

        inventory.ClearPossibleItems();
        inventory.CheckForPossibleRecipes();
    }
}