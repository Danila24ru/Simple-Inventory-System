using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory
{
    public class FreeSlotEvent : UnityEvent<Slot> { }

    [RequireComponent(typeof(Outline))]
    public class Slot : MonoBehaviour
    {
        public ItemType slotType;
        public ItemData itemData;

        public bool isEmpty = true;

        [SerializeField]
        private Image itemImage;

        private Outline outline; // Highlight slot

        public FreeSlotEvent freeSlotEvent = new FreeSlotEvent();

        // We should use custom checking, because OnMouseUp fires only when OnMouseDown called
        private bool mouseIn = false;

        private void Awake()
        {
            outline = GetComponent<Outline>();
        }

        private void Update()
        {
            // Clear slot if mouseUp on it
            if (Input.GetMouseButtonUp(0) && mouseIn && isEmpty == false)
            {
                Debug.Log("Clear slot!");

                freeSlotEvent?.Invoke(this);
                isEmpty = true;
                itemImage.gameObject.SetActive(false);
            }
        }

        // Set item to this slot and update icon
        public void SetItem(ItemData itemData)
        {
            this.itemData = itemData;

            isEmpty = false;

            SetItemImage(itemData.icon);

            itemImage.gameObject.SetActive(true);
        }

        public void SetItemImage(Sprite itemSprite)
        {
            if (itemImage)
            {
                itemImage.sprite = itemSprite;
            }
        }

        private void OnMouseEnter()
        {
            //Highlight slot when mouseIn and slot not empty
            if (outline && !isEmpty)
            {
                outline.effectColor = Color.red;
            }

            mouseIn = true;
        }

        private void OnMouseExit()
        {
            ClearSlotHighlight();

            mouseIn = false;
        }

        private void OnDisable()
        {
            ClearSlotHighlight();
        }

        private void ClearSlotHighlight()
        {
            if (outline)
            {
                outline.effectColor = Color.white;
            }
        }
    }
}


