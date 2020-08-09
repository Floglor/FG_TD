using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Items;
using MyBox;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ItemManager : MonoBehaviour
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public const int FreeItemsCapacity = 45;
        public Canvas allItemsCanvas;


        public GameObject allItemsPanel;
        private List<GameObject> freeItemParents;
        public List<GameObject> freeItemSlots;
        public List<Item> freeItems;
        public List<Recipe> recipes;
        public GameObject itemPrefab;
        public GameObject itemSlotParentPrefab;

        private Dictionary<int, Item> m_ItemMap = new Dictionary<int, Item>();
        public List<Item> allItems;

        public static ItemManager instance;

        [Header("Inventory Stuff")] 
        public Canvas inventoryCanvas;
        public Inventory currentInventory;
        public List<GameObject> inventoryPanels;
        public Canvas possibleItemsCanvas;
        public Button possibleItemBTNPrefab;
        public List<Button> possibleItemsButtons;

        public void InvokeInventory(Inventory inventory)
        {
            //TODO: more of a good optimization 

            ClearInventoryPanels();
            
            if (inventory == null) return;
            if (inventory.capacity == 0) return;

            inventoryCanvas.enabled = true;
            
            currentInventory = inventory;


            for (int i = 0; i < currentInventory.capacity; i++)
            {
               GameObject parent = Instantiate(itemSlotParentPrefab, inventoryCanvas.transform);
               GameObject inventoryPanel = parent.transform.GetChild(0).gameObject;
               inventoryPanels.Add(inventoryPanel);

               ItemSlot itemSlot = inventoryPanel.GetComponent<ItemSlot>();
               itemSlot.inventory = inventory;
               itemSlot.index = i;
               itemSlot.isInventorySlot = true;
               if (currentInventory.items[i] != null)
               {
                   CreateItemInSlot(currentInventory.items[i], inventoryPanel);
               }
            }
        }

        public void ClearInventoryPanels()
        {
            if (currentInventory != null)
            {
                foreach (GameObject child in inventoryCanvas.transform.gameObject.GetAllChildren())
                {
                    Destroy(child);
                }
            }
            
            inventoryPanels.Clear();
        }

        public void CreatePossibleItemsButtons()
        {
            if (currentInventory == null) return;
            
            foreach (Recipe possibleItem in currentInventory.possibleRecipes)
            {
                Button button = Instantiate(possibleItemBTNPrefab, possibleItemsCanvas.transform);
                button.image.sprite = possibleItem.result.itemImage;
                possibleItemsButtons.Add(button);
                button.onClick.AddListener(delegate { CraftItem(possibleItem); });
                
            }
        }

        public void ClearPossibleItemsButtons()
        {
            foreach (Button button in possibleItemsButtons)
            {
                Destroy(button.gameObject);
            }
            
            possibleItemsButtons.Clear();
        }

        private void CraftItem(Recipe possibleItem)
        {
            foreach (Item item in possibleItem.combination)
            {
                currentInventory.DeleteItem(item.itemID);
            }

            //Debug.Log(FindFirstFreeInventorySlot());
            CreateItemInSlot(possibleItem.result, FindFirstFreeInventorySlot());

            currentInventory.ClearPossibleItems();
            currentInventory.CheckForPossibleRecipes();
        }


        public void HideInventory()
        {
            inventoryCanvas.enabled = false;
        }

        private void Awake()
        {
            instance = this;
            //allItems = new List<Item>();
            recipes = new List<Recipe>();
            freeItems = new List<Item> {Capacity = FreeItemsCapacity};
            possibleItemsButtons = new List<Button>();

            inventoryPanels = new List<GameObject>();
            for (int j = 0; j < FreeItemsCapacity; j++)
            {
                freeItems.Add(null);
            }


            //Debug.Log(foundObjects.Length);

            foreach (Item item in allItems)
            {
                //Debug.Log(item.name);
                item.itemID = item.GetInstanceID();
            }


            foreach (Item item in allItems)
            {
                m_ItemMap.Add(item.itemID, item);
            }

            foreach (Item item in allItems)
            {
                if (item.isBasic) continue;
                if (item.combination.IsNullOrEmpty()) continue;

                Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
                recipe.combination = item.combination;
                recipe.result = item;
                recipes.Add(recipe);
            }

            freeItemParents = allItemsPanel.GetAllChildren();

            freeItemSlots = new List<GameObject>();

            foreach (GameObject freeItemParent in freeItemParents)
            {
                freeItemSlots.Add(freeItemParent.GetAllChildren()[0]);
            }


            int i = 0;

            foreach (GameObject freeItemSlot in freeItemSlots)
            {
                freeItemSlot.GetComponent<ItemSlot>().index = i;
                i++;
            }
        }


        public void GetItem(int i)
        {
           // Debug.Log("getting random item");
            GameObject itemSlot = FindFirstFreeItemSlot();

            if (itemSlot != null)
            {
                Item item = allItems[i];

                CreateItemInSlot(item, itemSlot);
            }
            else
            {
                Debug.Log("itemSlot = null");
                return;
            }
        }

        private void CreateItemInSlot(Item item, GameObject itemSlot)
        {
            GameObject itemPanel = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);

            //Debug.Log(itemPanel.name);

            ItemDragNDrop itemDragNDrop = itemPanel.GetComponent<ItemDragNDrop>();
            itemDragNDrop.item = item;

            itemPanel.transform.SetParent(itemSlot.transform);
            itemDragNDrop.itemSlot = itemSlot.GetComponent<ItemSlot>();
            itemDragNDrop.mainCanvas = allItemsCanvas;
            itemDragNDrop.parentCanvas = allItemsCanvas;

            itemPanel.GetAllChildren()[0].GetComponent<Image>().sprite = itemDragNDrop.item.itemImage;

            Vector3 transformLocalScale = itemPanel.GetComponent<RectTransform>().localScale;

            transformLocalScale.y = 1;
            transformLocalScale.x = 1;
            itemPanel.GetComponent<RectTransform>().localScale = transformLocalScale;

            if (!itemSlot.GetComponent<ItemSlot>().isInventorySlot)
            {
                int index = itemSlot.GetComponent<ItemSlot>().index;
                freeItems[index] = item;
                freeItemSlots[index].GetComponent<ItemSlot>().item = item;
            }
            else
            {
                //Debug.Log("INVENTORY SLOT CREATION");
                int index = itemSlot.GetComponent<ItemSlot>().index;
                inventoryPanels[index].GetComponent<ItemSlot>().item = item;
                currentInventory.items[index] = item;
                currentInventory.ApplyEffects(item);
            }


            // Debug.Log(itemPanel.GetComponent<RectTransform>().localScale);
        }

        private GameObject FindFirstFreeItemSlot()
        {
            return freeItemSlots.FirstOrDefault(freeItemSlot => freeItemSlot.GetComponent<ItemSlot>().item == null);
        }
        
        private GameObject FindFirstFreeInventorySlot()
        {
            foreach (GameObject freeItemSlot in inventoryPanels)
            {
                if (freeItemSlot.GetComponent<ItemSlot>().item == null) return freeItemSlot;
            }

            Debug.LogError("NO FREE ITEM SLOTS");
            return null;
        }

        public void ReverseSynchronizeItemSlots()
        {
            int i = 0;
            foreach (GameObject freeItemSlot in freeItemSlots)
            {
                if (allItems[i] == null) continue;

                GameObject item = Instantiate(itemPrefab, freeItemSlot.transform.position, Quaternion.identity);
                item.transform.parent = freeItemSlot.transform.GetChild(0);

                item.transform.GetChild(0).GetComponent<Image>().sprite = allItems[i].itemImage;
                item.GetComponent<ItemDragNDrop>().parentCanvas = allItemsCanvas;
                i++;
            }
        }

        public void SynchronizeItemSlots()
        {
            int i = 0;

            foreach (ItemSlot itemSlot in freeItemSlots.Select(freeItemSlot => freeItemSlot.GetComponent<ItemSlot>()))
            {
                if (itemSlot.item != null)
                {
                    allItems[i] = itemSlot.item;
                }

                i++;
            }
        }

        public void OpenCloseAllItemsMenu()
        {
            allItemsCanvas.enabled = !allItemsCanvas.isActiveAndEnabled;
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                GetItem(i);  
                GetItem(i);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}