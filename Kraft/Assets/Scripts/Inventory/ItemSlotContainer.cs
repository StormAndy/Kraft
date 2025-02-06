using System.Collections.Generic;
using UnityEngine;


/// <summary> Serializable version of an ItemSlotContainer. </summary>
[System.Serializable]
public class SerializableItemSlotContainer
{
    public int maxSlots;
    public List<SerializableItemStack> slots;
}

/// <summary> Serializable representation of an item stack. </summary>
[System.Serializable]
public class SerializableItemStack
{
    public int slotIndex;
    public string itemUniqueID;
    public int quantity;
}


/// <summary> Simple slot-based storage without game object slots, to be used for storage containers and item containers like bags</summary>
public class ItemSlotContainer : IContainer
{
    private Dictionary<int, ItemStack> slots = new();  
    private int maxSlots = -1;  // 0 or less means unlimited slots

    public ItemSlotContainer(int maxSlots = -1) { this.maxSlots = maxSlots;}

    public bool HasInventorySpace(ItemData item, int quantity)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots.Values)
                if (slot.itemID == item.uniqueID && slot.quantity < item.maxStackSize)
                    return true;
        }
        return maxSlots <= 0 || slots.Count < maxSlots;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots.Values)
            {
                if (slot.itemID == item.uniqueID && slot.quantity < item.maxStackSize)
                {
                    int spaceLeft = item.maxStackSize - slot.quantity;
                    int addAmount = Mathf.Min(spaceLeft, amount);
                    slot.quantity += addAmount;
                    amount -= addAmount;
                    if (amount <= 0) return;
                }
            }
        }

        while (amount > 0 && (maxSlots == -1 || slots.Count < maxSlots))
        {
            int addAmount = Mathf.Min(amount, item.maxStackSize);
            slots[slots.Count] = new ItemStack(item.uniqueID, addAmount);
            amount -= addAmount;
        }

        if (amount > 0)
            Debug.LogWarning("Not enough space to add all items.");
    }

    public void RemoveItem(ItemData item)
    {
        foreach (var slot in slots)
        {
            if (slot.Value.itemID == item.uniqueID)
            {
                slots.Remove(slot.Key);
                return;
            }
        }
    }

    ///<summary> Converts this container to a serializable data format for saving.</summary>
    public SerializableItemSlotContainer ConvertToSerializable()
    {
        SerializableItemSlotContainer data = new SerializableItemSlotContainer
        {
            maxSlots = maxSlots,
            slots = new List<SerializableItemStack>()
        };

        foreach (var kvp in slots)
        {
            data.slots.Add(new SerializableItemStack
            {
                slotIndex = kvp.Key,
                itemUniqueID = kvp.Value.itemID,
                quantity = kvp.Value.quantity
            });
        }
        return data;
    }


}


