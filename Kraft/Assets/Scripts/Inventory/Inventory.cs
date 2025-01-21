using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

/// <summary> Manages a collection of items in the inventory. </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] public GameObject slotItemPrefab { get; private set; }

    public GameObject[] itemSlotsObjects; ///<summary> List of items in the inventory. </summary>
    [SerializeField]private Dictionary<int, InventorySlot> itemSlots = new Dictionary<int, InventorySlot>(); ///<summary> List of items in the inventory. </summary>


    private void Start()
    {
        PopulateItemSlotDictionary();
    }

    /// <summary> Populate inventory slot lookup dictionary from slot prefabs assigned in itemSlotsObjects </summary>
    public void PopulateItemSlotDictionary()
    {
        int _slotcounter = 1;
        foreach (GameObject obj in itemSlotsObjects)
        {
            InventorySlot _slot = obj.GetComponent<InventorySlot>();
            if (_slot != null)
            {
                itemSlots.Add(_slotcounter, _slot);
                _slotcounter++;
            }
        }
    }

    /// <summary> Adds an item to the inventory. </summary>
    public void AddItem(ItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem == null)
            {
                GameObject _newObj = Instantiate(slotItemPrefab, slot.Value.transform);
                InventorySlotItem _slotItem = _newObj.GetComponent<InventorySlotItem>();
                if (_slotItem != null)
                    _slotItem.SetItemData(item);

                return;
            }
        }

        Debug.Log("No slots available, add to Drop List?");
    }

    /// <summary> Removes an item from the inventory. </summary>
    public void RemoveItem(ItemData item)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Value.inventorySlotItem != null)
            {
                if (slot.Value.inventorySlotItem.itemData == item)
                {
                    Debug.Log(item.name + " removed item at slot: " + slot.Value);
                    Destroy(slot.Value.gameObject);
                }
            }
        }
    }

    /// <summary> Removes the items from the item slot specified </summary>
    public void RemoveItemAtSlot(InventorySlot slot)
    {

    }

    /// <summary> Retrieves all items in the inventory to manage as a List indenpendant of Slots </summary>
    public List<ItemData> GetItemList()
    {
        return null;
    }

    
}

