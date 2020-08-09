using System;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ItemDragNDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public Canvas mainCanvas;
        public Transform savedParent;
        private RectTransform rectTransform;
        public Canvas parentCanvas;
        private CanvasGroup canvasGroup;
        private Vector3 position;

        public Item item;

        public ItemSlot itemSlot;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
           // item = GetComponent<Item>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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
            rectTransform.anchoredPosition += eventData.delta/parentCanvas.scaleFactor;
            
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
