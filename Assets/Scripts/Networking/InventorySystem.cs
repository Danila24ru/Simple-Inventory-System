using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class ItemOperationEvent : UnityEvent<ItemData> { }

    public class InventorySystem : MonoBehaviour
    {
        [SerializeField]
        private TriggerHandler openInventoryTrigger; // trigger for checking mouse click
        private TriggerHandler itemTrigger; // trigger for items falling into inventory
        private List<ItemData> items;

        private List<Slot> itemSlots; // UI slots.

        [SerializeField]
        private GameObject inventoryCanvas;

        public ItemOperationEvent itemAddedEvent;
        public ItemOperationEvent itemRemovedEvent;

        private void Awake()
        {
            itemTrigger = GetComponentInChildren<TriggerHandler>();

            //Open inventory canvas when mouse down on model
            openInventoryTrigger?.onMouseDownEvent.AddListener(() => { inventoryCanvas.SetActive(true); });

            ////Close inventory canvas when mouse up. Also we should check if mouse on slot, because this event fires before slot become free
            openInventoryTrigger?.onMouseUpEvent.AddListener(() =>
            {
                if (!IsMouseOverSlotUI())
                    inventoryCanvas.SetActive(false);
            });

            items = new List<ItemData>();
            itemSlots = new List<Slot>();

            itemAddedEvent = new ItemOperationEvent();
            itemRemovedEvent = new ItemOperationEvent();

            var slots = GetComponentsInChildren<Slot>(true);
            if (slots != null)
            {
                itemSlots.AddRange(slots);
            }
        }

        public void Start()
        {
            if (itemTrigger)
            {
                // Subscribing to event when Item falls to inventory
                itemTrigger.enterTriggerEvent.AddListener(OnTriggerEnterObject);
            }
            else
            {
                Debug.LogWarning("Trigger Handler not exists!");
            }

            // Subscribing to an event that clears the slot
            foreach (var slot in itemSlots)
            {
                slot.freeSlotEvent.AddListener(OnFreeSlot);
            }

            itemAddedEvent.AddListener(SendStatisticOnAddItem);
            itemRemovedEvent.AddListener(SendStatisticOnClearSlot);
        }

        // Called when slot free.
        private void OnFreeSlot(Slot slot)
        {
            var itemObject = Instantiate(slot.itemData.itemPrefab, slot.gameObject.transform.position, Quaternion.identity);

            var rigidbody = itemObject.GetComponent<Rigidbody>();

            rigidbody.AddForce(new Vector3(-10, 10, 0));

            inventoryCanvas.SetActive(false);

            itemRemovedEvent?.Invoke(slot.itemData);
        }

        // Called when object falls into inventory
        private void OnTriggerEnterObject(GameObject itemObject)
        {
            var item = itemObject.GetComponent<Item>();

            if (item)
            {
                var itemData = item.itemData;

                var slotToSetItem = GetAvailableSlotForItem(itemData);

                if (slotToSetItem != null)
                {
                    slotToSetItem.SetItem(itemData);

                    items.Add(itemData);
                    itemAddedEvent?.Invoke(itemData);

                    Debug.Log("Item added!");

                    Destroy(itemObject);
                }
                else
                {
                    // Throw item away, if no slot available
                    itemObject.GetComponent<Rigidbody>().AddForce(new Vector3(-7, 30, 0));
                }

            }
        }

        // Looking for free slot othewise return null
        private Slot GetAvailableSlotForItem(ItemData itemData)
        {
            return itemSlots.Where(slot => slot.isEmpty && slot.slotType == itemData.itemType).FirstOrDefault();
        }

        //Check if mouse over not empty slot
        private bool IsMouseOverSlotUI()
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var slot = hit.collider.gameObject.GetComponent<Slot>();

                if (slot && !slot.isEmpty)
                    return true;
            }

            return false;
        }


        private void SendStatisticOnAddItem(ItemData itemData)
        {
            WWWForm form = new WWWForm();

            form.AddField("itemId", itemData.id);
            form.AddField("event", "SetItemToInventory");

            WebRequests.Instance.Post("https://dev3r02.elysium.today/inventory/status", form, (result) => { Debug.Log(result); });
        }

        private void SendStatisticOnClearSlot(ItemData itemData)
        {
            WWWForm form = new WWWForm();

            form.AddField("itemId", itemData.id);
            form.AddField("event", "RemoveItemFromInventory");

            WebRequests.Instance.Post("https://dev3r02.elysium.today/inventory/status", form, (result) => { Debug.Log(result); });
        }

    }

}
