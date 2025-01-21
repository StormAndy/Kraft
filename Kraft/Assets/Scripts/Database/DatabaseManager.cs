using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using static UnityEditor.Progress;
using System.Linq.Expressions;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private Dictionary<string, ItemData> itemDataDictionary = new Dictionary<string, ItemData>();

    // Load data from a JSON file into the database
    public void LoadData()
    {
        LoadAllItems();
    }

    public void LoadAllItems()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Data/Items");
        foreach (var jsonFile in jsonFiles)
        {
            ItemList items = JsonUtility.FromJson<ItemList>(jsonFile.text);
            foreach (var item in items.items)
            {
                if (!itemDataDictionary.ContainsKey(item.uniqueID))
                {
                    itemDataDictionary[item.uniqueID] = item;
                }
                else
                {
                    Debug.LogWarning($"Duplicate item ID found: {item.uniqueID}");
                }
            }
        }
    }

    public void SaveTestData()
    {
        // Create a list of berry items
        ItemList itemList = new ItemList();


        itemList.items = new ItemData[]
        {
            new ItemData { name = "Strawberry", description = "A sweet red berry", isStackable = true, uniqueID = "berry_001" },
            new ItemData { name = "Blueberry", description = "A small round blue berry", isStackable = true, uniqueID = "berry_002" },
            new ItemData { name = "Raspberry", description = "A tangy red berry", isStackable = true, uniqueID = "berry_003" }
        };

        // Serialize the itemList to JSON
        string json = JsonUtility.ToJson(itemList, true);

        // Define the file path for Resources folder
        string filePath = Path.Combine(Application.dataPath, "Resources/Data/Items/Berries2.json");

        try
        {
            // Create directories if they do not exist
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Write JSON to file
            File.WriteAllText(filePath, json);
            Debug.Log($"Test data saved successfully to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving data: {e.Message}");
        }
    }

    /// <summary> Debug Log items </summary>
    public void LogData()
    {
        foreach (var item in itemDataDictionary)
        {
                Debug.Log($"Logging item: {item.Key}");
        }
    }


    /// <summary> Save the current database to a JSON file </summary>
    public void SaveData(string path)
    {
        ItemList items = new ItemList { items = itemDataDictionary.Values.ToArray() };
        string json = JsonUtility.ToJson(items, true);
        File.WriteAllText(path, json);
    }

    // Retrieve an item by ID
    public ItemData GetItemData(string id)
    {
        itemDataDictionary.TryGetValue(id, out ItemData item);
        return item;
    }

    // Update an item in the database
    public void UpdateItemData(ItemData item)
    {
        itemDataDictionary[item.uniqueID] = item;
    }

    // Nested container class for JSON serialization
    [System.Serializable]
    public class ItemList
    {
        public ItemData[] items;
    }
}


