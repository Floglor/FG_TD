using System;
using System.Linq;
using Items;
using Managers;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ItemDragNDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
        IDropHandler
    {
        public Canvas mainCanvas;
        public Transform savedParent;
        private RectTransform rectTransform;
        public Canvas parentCanvas;
        private CanvasGroup canvasGroup;
        private Vector3 position;
        public Item item;

        public ItemSlot itemSlot;

        public GameObject itemDescription;

        private int timesClicked;
        private float clickTime;
        private const float ClickDelay = .4f;

        private void Start()
        {
            Vector3 position = gameObject.GetComponent<RectTransform>().localPosition;
            position.z = 0;

            gameObject.GetComponent<RectTransform>().localPosition = position;
        }

        private void Update()
        {
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                //do stuff here
                Debug.Log("Right click");
                timesClicked = 0;
                NodeUI.instance.mainItemDescription.Show();
                if (item == null)
                    Debug.LogError("SELECTED ITEM IS NULL! FUCK!");
                NodeUI.instance.mainItemDescription.SelectedItem = item;
            }
        }

        private void OnMouseDown()
        {
            //Debug.Log("click");
            timesClicked++;

            if (timesClicked == 1)
                clickTime = Time.time;

            if (timesClicked > 1 && Time.time - clickTime < ClickDelay)
            {
                Debug.Log("double Click First shit");
                clickTime = 0;
                timesClicked = 0;

                if (itemSlot.isInventorySlot)
                {
                    MoveItemToGlobalInventory();
                }
                else
                if (ItemManager.instance.currentInventory != null)
                {
                    Inventory inventory = ItemManager.instance.currentInventory;
                    ItemSlot itemSlotLocal = inventory.FindFirstFreeSlot();

                    if (itemSlotLocal != null)
                    {
                        MoveItemTo(itemSlotLocal);
                    }
                }
                
            }    
        }

        private void MoveItemToGlobalInventory()
        {
            MoveItemTo(ItemManager.instance.FindFirstFreeItemSlot().GetComponent<ItemSlot>(), true);
        }

        private void MoveItemTo(ItemSlot targetItemSlot, bool toGlobal = false)
        {
            itemSlot.RemoveItem();
            itemSlot = targetItemSlot;
            Transform thisTransform = transform;
            if (toGlobal)
                thisTransform.localScale /= 2;
            else
                thisTransform.localScale *= 2;
            
            thisTransform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 6;
            thisTransform.GetComponent<SpriteRenderer>().sortingOrder = 6;
            thisTransform.SetParent(targetItemSlot.transform);
            targetItemSlot.item = item;
            
            if (!toGlobal)
            {
                targetItemSlot.inventory.items[targetItemSlot.index] = item;
                targetItemSlot.inventory.ApplyEffects(item);
            }
            //TODO: ORDER AND SCALE 
        }


        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            // item = GetComponent<Item>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("!!!");
            position = transform.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Transform thisTransform = transform;

            savedParent = thisTransform.parent;

            thisTransform.SetParent(mainCanvas.transform);

            canvasGroup.blocksRaycasts = false;

            itemSlot.RemoveItem();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            bool isHoveredOverItemSlot = false;

            foreach (GameObject o in eventData.hovered.Where(o => o.GetComponent<ItemSlot>() != null))
            {
                Debug.Log("!");
                ItemSlot itemSlot = o.GetComponent<ItemSlot>();
                isHoveredOverItemSlot = true;
                itemSlot.item = item;
            }

            if (!isHoveredOverItemSlot)
            {
                transform.SetParent(savedParent);
                savedParent.GetComponent<ItemSlot>().AddItem(gameObject);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
            bool isHoveredOverItemSlot = false;

            foreach (GameObject o in eventData.hovered.Where(o => o.GetComponent<ItemSlot>()))
            {
                isHoveredOverItemSlot = true;
            }

            if (!isHoveredOverItemSlot)
            {
                transform.SetParent(savedParent);
                savedParent.gameObject.GetComponent<ItemSlot>().AddItem(gameObject);
            }
        }
    }
}