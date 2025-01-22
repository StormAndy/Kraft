using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

/// <summary> Manages a collection of items in the inventory. </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] public GameObject slotItemPrefab;

    public GameObject[] itemSlotsObjects; ///<summary> List of items in the inventory. </summary>
    [SerializeField]private Dictionary<int, InventorySlot> itemSlots = new Dictionary<int, InventorySlot>(); ///<summary> List of items in the inventory. </summary>


    private void Start()
    {
        PopulateItemSlotDictionary();
        Invoke("AddTestItems", 3.0f);
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

    private void AddTestItems()
    {
        AddItem(new ItemData());
        AddItem(new ItemData());
        AddItem(new ItemData());
        AddItem(new ItemData());
    }

    /// <summary> Adds an item to the first available slot inventory, creates ItemSlotItem for ItemData and assigns to empty Itemslot comp </summary>
    public void AddItem(ItemData item)
    {
        foreach (var slot in itemSlots)
        {
            
            if (slot.Value.inventorySlotItem == null)
            {
                Debug.Log(slot.Value.name + " slot value exists, instantiate new item prefab");
                GameObject _newObj = Instantiate(slotItemPrefab, slot.Value.transform);

                slot.Value.inventorySlotItem = _newObj.GetComponent<InventorySlotItem>();
                if (slot.Value.inventorySlotItem != null)
                    slot.Value.inventorySlotItem.SetItemData(item);


                return;
            }
            Debug.Log("slot is not empty, check next slot");
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

