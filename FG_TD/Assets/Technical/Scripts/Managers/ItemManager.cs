using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Items;
using MyBox;
using Shooting;
using TMPro;
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

        [Header("Inventory Stuff")] public Canvas inventoryCanvas;
        public TextMeshProUGUI inventoryText;
        public Inventory currentInventory;
        public List<GameObject> inventoryPanels;
        public Canvas possibleItemsCanvas;
        public Button possibleItemBTNPrefab;
        public List<Button> possibleItemsButtons;
        public List<Recipe> possibleRecipes { get; set; }
        private List<Item> savedItems { get; set; }
        public List<Inventory> allInventories;
        public int maxInventoryCapacity;
        public GameObject itemWheel;

        [Header("Inventory Graphics")] public List<GameObject> itemWheelGraphics;
        public List<ItemWheeSlotCoordsList> itemWheeSlotCoordsList;


        private const string Color15 = "#009999";
        private const string Color30 = "#FF8F6C";
        private const string Color45 = "#BF0B2C";

        public void CountItems()
        {
            int count = freeItems.Count(freeItem => freeItem != null);

            StringBuilder stringBuilder = new StringBuilder();
            string countString;
            if (count <= 15)
            {
                countString = stringBuilder.Append($"<color={Color15}>{count} / 45 </color>").ToString();
            }
            else if (count <= 30)
            {
                countString = stringBuilder.Append($"<color={Color30}>{count} / 45 </color>").ToString();
            }
            else if (count <= 45)
            {
                countString = stringBuilder.Append($"<color={Color45}>{count} / 45 </color>").ToString();
            }
            else
            {
                countString = stringBuilder.Append($"<color={Color45}>{count} / 45 </color>").ToString();
            }

            inventoryText.text = countString;
        }

        public void ClearPossibleItems()
        {
            possibleRecipes.Clear();
            ClearPossibleItemsButtons();
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
                let count = inventoryItems.Where(inventoryItem => inventoryItem != null)
                    .Count(inventoryItem => inventoryItem.name == keyValuePair.Key)
                where keyValuePair.Value > count
                select keyValuePair).Any();
        }

        public void SeekForBiggestInventory()
        {
            maxInventoryCapacity = 0;
            foreach (Inventory inventory in allInventories.Where(inventory => inventory.capacity > maxInventoryCapacity))
            {
                maxInventoryCapacity = inventory.capacity;
            }
        }

        public void GICheckForPossibleRecipes()
        {
            ClearPossibleItems();
            SeekForBiggestInventory();
            //if (capacity <= GetItemsCount()) return;

            if (freeItems.Count == 1) return;

            savedItems = new List<Item>();

            foreach (Recipe recipe in instance.recipes)
            {
                if (recipe == null) return;
                if (recipe.combination.Count > maxInventoryCapacity) return;

                if (recipe.combination.Count <= freeItems.Count)
                {
                    bool combinationFound = true;
                    foreach (Item recipeItem in recipe.combination)
                    {
                        if (!freeItems.Contains(recipeItem))
                        {
                            combinationFound = false;
                            break;
                        }
                    }

                    if (!combinationFound) continue;

                    if (!possibleRecipes.Contains(recipe) && IsEnoughItems(recipe, freeItems))
                        possibleRecipes.Add(recipe);
                }
            }


            //Debug.Log(possibleItems.Count);
            GICreatePossibleItemsButtons();
        }

        public void InvokeInventory(Inventory inventory)
        {
            //TODO: more of a good optimization 

            
            if (inventory == null)
            {
                itemWheel.SetActive(false);
                return;
            }

            if (inventory.capacity == 0) return;

            itemWheel.SetActive(true);


            inventoryCanvas.enabled = true;
            inventoryCanvas.gameObject.SetActive(true);

            currentInventory = inventory;
            SetItemWheelGraphics();
            //ClearInventoryPanels();


            for (int i = 0; i < currentInventory.capacity; i++)
            {
                GameObject inventoryPanel = inventoryCanvas.transform.GetChild(i).transform.GetChild(0).gameObject;

                inventoryPanels.Add(inventoryPanel);

                ItemSlot itemSlot = inventoryPanel.GetComponent<ItemSlot>();
                itemSlot.inventory = inventory;
                itemSlot.index = i;
                itemSlot.isInventorySlot = true;
                itemSlot.inventory.itemSlots.Add(itemSlot);
                if (currentInventory.items[i] != null)
                {
                    CreateItemInSlot(currentInventory.items[i], inventoryPanel);
                }
            }
        }

   
        private void SetItemWheelGraphics()
        {
            switch (currentInventory.capacity)
            {
                case 1:
                    itemWheel.GetComponent<SpriteRenderer>().sprite =
                        itemWheelGraphics[0].GetComponent<SpriteRenderer>().sprite;
                    break;
                case 2:
                    itemWheel.GetComponent<SpriteRenderer>().sprite =
                        itemWheelGraphics[1].GetComponent<SpriteRenderer>().sprite;
                    break;
                case 3:
                    itemWheel.GetComponent<SpriteRenderer>().sprite =
                        itemWheelGraphics[2].GetComponent<SpriteRenderer>().sprite;
                    break;
                case 4:
                    itemWheel.GetComponent<SpriteRenderer>().sprite =
                        itemWheelGraphics[3].GetComponent<SpriteRenderer>().sprite;
                    break;
                case 5:
                    itemWheel.GetComponent<SpriteRenderer>().sprite =
                        itemWheelGraphics[4].GetComponent<SpriteRenderer>().sprite;
                    break;
            }

            List<GameObject> slotList = itemWheel.transform.GetChild(0).gameObject.GetAllChildren();

            AlignItemWheelSlots(slotList);
        }

        private void AlignItemWheelSlots(List<GameObject> slotList)
        {
            for (int i = 0; i < currentInventory.capacity; i++)
            {
                Vector3 transformPosition = slotList[i].transform.GetComponent<RectTransform>().anchoredPosition;
                transformPosition.x = itemWheeSlotCoordsList[currentInventory.capacity - 1].slotList[i].x;
                transformPosition.y = itemWheeSlotCoordsList[currentInventory.capacity - 1].slotList[i].y;
                slotList[i].transform.GetComponent<RectTransform>().anchoredPosition = transformPosition;
            }
        }


        public void ClearInventoryPanels()
        {
            /* return;
            Debug.Log("clearing panels");
            if (currentInventory != null)
            {
                foreach (GameObject child in inventoryCanvas.transform.gameObject.GetAllChildren())
                {
                    Destroy(child);
                }
            }
            else
            {
                Debug.Log("currentInventory is null");
            }

            inventoryPanels.Clear(); */
        }

        public void GICreatePossibleItemsButtons()
        {
            foreach (Recipe possibleItem in possibleRecipes)
            {
                Button button = Instantiate(possibleItemBTNPrefab, possibleItemsCanvas.transform);
                button.image.sprite = possibleItem.result.itemImage;
                possibleItemsButtons.Add(button);
                button.onClick.AddListener(delegate { GICraftItem(possibleItem); });
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

        private void GICraftItem(Recipe possibleItem)
        {
            foreach (Item item in possibleItem.combination)
            {
                GIDeleteItem(item.itemID);
            }

            //Debug.Log(FindFirstFreeInventorySlot());
            CreateItemInSlot(possibleItem.result, GIFindFirstFreeInventorySlot());

            ClearPossibleItems();
            GICheckForPossibleRecipes();
        }

        public void GIDeleteItem(int itemID)
        {
            //Debug.Log($"Item count: {GetItemsCount()}");

            for (int i = 0; i < freeItems.Count; i++)
            {
                if (freeItems[i] == null) continue;
                if (itemID != freeItems[i].itemID) continue;


                // Debug.Log($"*E*: {i}");

                GameObject instanceInventoryPanel = freeItemSlots[i];

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

                freeItems[i] = null;
                itemSlot.item = null;
                break;
            }

            CountItems();
        }


        public void HideInventory()
        {
            inventoryCanvas.gameObject.SetActive(false);
            currentInventory = null;
        }

        private void Awake()
        {
            instance = this;
            //allItems = new List<Item>();
            recipes = new List<Recipe>();
            freeItems = new List<Item> {Capacity = FreeItemsCapacity};
            savedItems = new List<Item>();
            possibleRecipes = new List<Recipe>();
            allInventories = new List<Inventory>();
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

            CountItems();
        }


        public void GetItemByArgument(Item item)
        {
            GameObject itemSlot = FindFirstFreeItemSlot();
            CreateItemInSlot(item, itemSlot);
            GICheckForPossibleRecipes();
        }

        public void GetItemByNumber(int i)
        {
            // Debug.Log("getting random item");
            GameObject itemSlot = FindFirstFreeItemSlot();

            if (itemSlot != null)
            {
                Item item = allItems[i];

                CreateItemInSlot(item, itemSlot);
                GICheckForPossibleRecipes();
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

            itemPanel.GetAllChildren()[0].GetComponent<SpriteRenderer>().sprite = itemDragNDrop.item.itemImage;

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

            GICheckForPossibleRecipes();
            // Debug.Log(itemPanel.GetComponent<RectTransform>().localScale);
            CountItems();
        }

        public GameObject FindFirstFreeItemSlot()
        {
            return freeItemSlots.FirstOrDefault(freeItemSlot => freeItemSlot.GetComponent<ItemSlot>().item == null);
        }

        private GameObject GIFindFirstFreeInventorySlot()
        {
            foreach (GameObject freeItemSlot in freeItemSlots)
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

                item.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = allItems[i].itemImage;
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
            allItemsCanvas.gameObject.SetActive(allItemsCanvas.enabled);

            if (allItemsCanvas.isActiveAndEnabled)
            {
                GICheckForPossibleRecipes();
            }
            else
            {
                ClearPossibleItemsButtons();
            }
        }


        void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                GetItemByNumber(i);
                GetItemByNumber(i);
            }
            
            GetItemByNumber(26);
        }        
    }
}