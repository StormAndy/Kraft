using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeData
{
    public string uniqueID;
    public string recipeName;
    public Sprite icon;

    public string outputItemID;
    public int outputQuantity = 1;
    public List<Ingredient> ingredients;
    public List<string> requiredTags;
}

[System.Serializable]
public class Ingredient
{
    public string itemID;
    public int quantity;
}
