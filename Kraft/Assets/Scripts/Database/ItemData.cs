using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string name;
    public string description;
    public bool isStackable;
    public int maxStackSize;
    public string uniqueID;
    public List<string> tags = new List<string>(); //Search/Filter Tags

    public Sprite graphicIcon;        // For UI (2D)
    public GameObject graphicPrefab;  // For world pickup (3D)

    public List<ItemType> itemType = new List<ItemType>(); 
}

[System.Serializable]
public enum ItemType
{
    Ingredient,
    Weapon,
    Tool,
    Axe,
    Pickaxe,
    Knife,
    Sword
}

[System.Serializable]
public class ItemCollection
{
    public List<ItemData> items = new List<ItemData>();
}

/// <summary> Represents a stack of items in a container class without a gameobject/slot item. Used for managing stacked items in simple slot-based and list-based containers. </summary> 
[System.Serializable]
public class ItemStack
{
    public string itemID;  // The item data associated with the stack
    public int quantity;       // The current quantity of the item in the stack

    public ItemStack(ItemData item, int quantity)
    {
        this.itemID = item.uniqueID;
        this.quantity = quantity;
    }
}
