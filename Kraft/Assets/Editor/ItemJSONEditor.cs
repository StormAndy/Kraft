using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

[Serializable]
public class ItemCollection
{
    public List<ItemData> items = new List<ItemData>();
}

public class ItemJSONEditor : EditorWindow
{
    private const string itemsFolderPath = "Assets/Resources/Data/Items";
    private List<string> jsonFiles = new List<string>();
    private int selectedFileIndex = -1;
    private ItemCollection currentItemCollection;

    private Vector2 sidebarScroll;
    private Vector2 inspectorScroll;

    [MenuItem("Tools/Item JSON Editor")]
    public static void OpenWindow()
    {
        GetWindow<ItemJSONEditor>("Item JSON Editor");
    }

    private void OnEnable()
    {
        LoadJsonFiles();
    }

    private void LoadJsonFiles()
    {
        if (!Directory.Exists(itemsFolderPath))
            Directory.CreateDirectory(itemsFolderPath);

        jsonFiles = new List<string>(Directory.GetFiles(itemsFolderPath, "*.json"));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawSidebar();
        DrawInspector();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("JSON Files", EditorStyles.boldLabel);

        sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);

        for (int i = 0; i < jsonFiles.Count; i++)
        {
            if (GUILayout.Button(Path.GetFileName(jsonFiles[i]), selectedFileIndex == i ? EditorStyles.toolbarButton : EditorStyles.miniButton))
            {
                selectedFileIndex = i;
                LoadJsonFile(jsonFiles[i]);
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Refresh", GUILayout.Height(25)))
            LoadJsonFiles();

        EditorGUILayout.EndVertical();
    }

    private void DrawInspector()
    {
        EditorGUILayout.BeginVertical();

        if (currentItemCollection != null)
        {
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

            for (int i = 0; i < currentItemCollection.items.Count; i++)
            {
                var item = currentItemCollection.items[i];
                EditorGUILayout.BeginVertical("box");

                item.name = EditorGUILayout.TextField("Name", item.name);
                item.description = EditorGUILayout.TextField("Description", item.description);
                item.isStackable = EditorGUILayout.Toggle("Is Stackable", item.isStackable);
                item.uniqueID = EditorGUILayout.TextField("Unique ID", item.uniqueID);

                // Tags
                EditorGUILayout.LabelField("Tags");
                if (item.tags == null) item.tags = new List<string>();
                for (int t = 0; t < item.tags.Count; t++)
                {
                    EditorGUILayout.BeginHorizontal();
                    item.tags[t] = EditorGUILayout.TextField($"Tag {t + 1}", item.tags[t]);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        item.tags.RemoveAt(t);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Tag")) item.tags.Add("");

                // Image Sprite Path
                item.graphicPath = EditorGUILayout.TextField("Graphic Path", item.graphicPath);

                if (GUILayout.Button("Remove Item", GUILayout.Height(20)))
                {
                    currentItemCollection.items.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add New Item", GUILayout.Height(25)))
                currentItemCollection.items.Add(new ItemData());

            if (GUILayout.Button("Save", GUILayout.Height(25)))
                SaveJsonFile(jsonFiles[selectedFileIndex]);

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("Select a JSON file to edit.");
        }

        EditorGUILayout.EndVertical();
    }


    private void LoadJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        currentItemCollection = JsonUtility.FromJson<ItemCollection>(json) ?? new ItemCollection();
    }

    private void SaveJsonFile(string filePath)
    {
        string json = JsonUtility.ToJson(currentItemCollection, true);
        File.WriteAllText(filePath, json);
        AssetDatabase.Refresh();
    }
}

