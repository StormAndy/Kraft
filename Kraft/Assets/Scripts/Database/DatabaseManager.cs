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

    /// <summary> Update an item in the database. </summary>
    public void UpdateItemData(ItemData item)
    {
        itemDataDictionary[item.uniqueID] = item;
    }

    /// <summary> Update a recipe in the database. </summary>
    public void UpdateRecipeData(RecipeData recipe)
    {
        recipeDataDictionary[recipe.uniqueID] = recipe;
    }

    /// <summary> Save all items in the database to a JSON file. </summary>
    public void SaveItems(string path)
    {
        ItemCollection items = new ItemCollection { items = itemDataDictionary.Values.ToList() };
        string json = JsonUtility.ToJson(items, true);
        File.WriteAllText(path, json);
    }

    /// <summary> Save all recipes in the database to a JSON file. </summary>
    public void SaveRecipes(string path)
    {
        List<RecipeData> recipes = recipeDataDictionary.Values.ToList();
        string json = JsonUtility.ToJson(new { recipes = recipes }, true);
        File.WriteAllText(path, json);
    }


}


