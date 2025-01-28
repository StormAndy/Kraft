using System.Collections.Generic;
using UnityEngine;

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
                if (slot.itemData.uniqueID == item.uniqueID && slot.quantity < item.maxStackSize)
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
                if (slot.itemData.uniqueID == item.uniqueID && slot.quantity < item.maxStackSize)
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
            slots[slots.Count] = new ItemStack(item, addAmount);
            amount -= addAmount;
        }

        if (amount > 0)
            Debug.LogWarning("Not enough space to add all items.");
    }

    public void RemoveItem(ItemData item)
    {
        foreach (var slot in slots)
        {
            if (slot.Value.itemData.uniqueID == item.uniqueID)
            {
                slots.Remove(slot.Key);
                return;
            }
        }
    }
}

