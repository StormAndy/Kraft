using UnityEngine;

public interface IContainer
{
    bool HasInventorySpace(ItemData item, int quantity);
    void AddItem(ItemData item, int amount = 1);
    void RemoveItem(ItemData item);
}
