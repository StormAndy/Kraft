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
