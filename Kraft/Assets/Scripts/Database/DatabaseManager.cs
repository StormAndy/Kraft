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
            ItemCollection items = JsonUtility.FromJson<ItemCollection>(jsonFile.text);
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
        ItemCollection items = new ItemCollection { items = itemDataDictionary.Values.ToList() };
        string json = JsonUtility.ToJson(items, true);
        File.WriteAllText(path, json);
    }

    /// <summary> Retrieve an item by ID </summary>
    public ItemData GetItemData(string id)
    {
        itemDataDictionary.TryGetValue(id, out ItemData item);
        return item;
    }

    /// <summary> Update an item in the database </summary>
    public void UpdateItemData(ItemData item)
    {
        itemDataDictionary[item.uniqueID] = item;
    }


}


