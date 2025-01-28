using System.Collections.Generic;


/// <summary> Simplest form of container as a list, item ID storage with no capacity limits</summary>
public class ItemListContainer : IContainer
{
    private Dictionary<string, int> items = new();  // ItemID -> Quantity

    public bool HasInventorySpace(ItemData item, int quantity)
    {
        return true;  // Always true for list containers since there's no max size
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (items.ContainsKey(item.uniqueID))
            items[item.uniqueID] += amount;
        else
            items[item.uniqueID] = amount;
    }

    public void RemoveItem(ItemData item)
    {
        if (items.ContainsKey(item.uniqueID))
            items.Remove(item.uniqueID);
    }
}
