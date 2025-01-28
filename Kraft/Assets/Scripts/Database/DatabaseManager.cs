using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using static UnityEditor.Progress;
using System.Linq.Expressions;


public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private Dictionary<string, ItemData> itemDataDictionary = new();
    [SerializeField] private Dictionary<string, RecipeData> recipeDataDictionary = new();

    /// <summary> Load items and recipes into the database. </summary>
    public void LoadData()
    {
        LoadAllItems();
        LoadAllRecipes();
    }

    /// <summary> Load all items from JSON files in the Resources/Data/Items folder. </summary>
    public void LoadAllItems()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Data/Items");
        foreach (var jsonFile in jsonFiles)
        {
            ItemCollection items = JsonUtility.FromJson<ItemCollection>(jsonFile.text);
            foreach (var item in items.items)
                if (!itemDataDictionary.ContainsKey(item.uniqueID))
                    itemDataDictionary[item.uniqueID] = item;
                else
                    Debug.LogWarning($"Duplicate item ID found: {item.uniqueID}");
        }
    }

    /// <summary> Load all recipes from JSON files in the Resources/Data/Recipes folder. </summary>
    public void LoadAllRecipes()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Data/Recipes");
        foreach (var jsonFile in jsonFiles)
        {
            RecipeData recipe = JsonUtility.FromJson<RecipeData>(jsonFile.text);
            if (!recipeDataDictionary.ContainsKey(recipe.uniqueID))
                recipeDataDictionary[recipe.uniqueID] = recipe;
            else
                Debug.LogWarning($"Duplicate recipe ID found: {recipe.uniqueID}");
        }
    }

    /// <summary> Debug log all items and recipes in the database. </summary>
    public void LogData()
    {
        Debug.Log("Logging all items:");
        foreach (var item in itemDataDictionary)
            Debug.Log($"Item: {item.Key}");

        Debug.Log("Logging all recipes:");
        foreach (var recipe in recipeDataDictionary)
            Debug.Log($"Recipe: {recipe.Key}");
    }

    /// <summary> Retrieve an item by its unique ID. </summary>
    public ItemData GetItemData(string id)
    {
        itemDataDictionary.TryGetValue(id, out ItemData item);
        return item;
    }

    /// <summary> Retrieve a recipe by its unique ID. </summary>
    public RecipeData GetRecipeData(string id)
    {
        recipeDataDictionary.TryGetValue(id, out RecipeData recipe);
        return recipe;
    }

}


