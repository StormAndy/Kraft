#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>Editor window to load, view, and edit CharacterSave data.</summary>
public class CharacterSaveEditor : EditorWindow
{
    private Vector2 leftScrollPos;
    private Vector2 rightScrollPos;
    private List<string> characterSaveFolders = new List<string>(); // Full folder paths for each character save.
    private int selectedIndex = -1;
    private CharacterSave loadedSave = null;
    private string saveFolderBase;

    // Fields for new character creation.
    private string newCharName = "NewCharacter";
    private int newCharId = 0;

    /// <summary>Opens the Character Save Editor window.</summary>
    [MenuItem("Tools/Character Save Editor")]
    public static void ShowWindow() => GetWindow<CharacterSaveEditor>("Character Save Editor");

    /// <summary>Initializes the window and refreshes character saves.</summary>
    private void OnEnable()
    {
        saveFolderBase = Path.Combine(Application.dataPath, "Saves", "Characters");
        RefreshCharacterSaves();
    }

    /// <summary>Refreshes the list of character save folders.</summary>
    private void RefreshCharacterSaves()
    {
        characterSaveFolders.Clear();
        if (Directory.Exists(saveFolderBase))
        {
            // Each subdirectory is assumed to be named "CharacterName_hashID"
            string[] dirs = Directory.GetDirectories(saveFolderBase);
            foreach (string dir in dirs)
            {
                string folderName = Path.GetFileName(dir);
                // The main save file is assumed to be "FolderName.charsave"
                string saveFilePath = Path.Combine(dir, folderName + ".charsave");
                if (File.Exists(saveFilePath))
                    characterSaveFolders.Add(dir);
            }
        }
        else
        {
            Debug.LogWarning("Save folder base does not exist: " + saveFolderBase);
        }
    }

    /// <summary>Draws the complete window GUI including left/right panes and new character creation section.</summary>
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawLeftPane();
        DrawRightPane();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        DrawNewCharacterSection();
    }

    /// <summary>Draws the left pane containing the list of character saves and a refresh button.</summary>
    private void DrawLeftPane()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Character Saves", EditorStyles.boldLabel);
        leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos);
        for (int i = 0; i < characterSaveFolders.Count; i++)
        {
            string folderName = Path.GetFileName(characterSaveFolders[i]);
            if (GUILayout.Button(folderName))
            {
                selectedIndex = i;
                LoadSelectedCharacterSave();
            }
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Refresh"))
            RefreshCharacterSaves();
        EditorGUILayout.EndVertical();
    }

    /// <summary>Draws the right pane with editable fields for the selected CharacterSave.</summary>
    private void DrawRightPane()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Character Save Data", EditorStyles.boldLabel);
        rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos);
        if (loadedSave != null && loadedSave.characterData != null)
        {
            EditorGUILayout.LabelField("Character Name:");
            loadedSave.characterData.name = EditorGUILayout.TextField(loadedSave.characterData.name);
            EditorGUILayout.LabelField("ID:");
            loadedSave.characterData.id = EditorGUILayout.IntField(loadedSave.characterData.id);
            EditorGUILayout.LabelField("Hash ID:");
            loadedSave.characterData.hashID = EditorGUILayout.IntField(loadedSave.characterData.hashID);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Skills", EditorStyles.boldLabel);
            if (loadedSave.skills != null)
            {
                for (int i = 0; i < loadedSave.skills.Count; i++)
                {
                    var skill = loadedSave.skills[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Skill ID:", GUILayout.Width(70));
                    skill.skillId = EditorGUILayout.TextField(skill.skillId);
                    EditorGUILayout.LabelField("Experience:", GUILayout.Width(70));
                    skill.totalExperience = EditorGUILayout.FloatField(skill.totalExperience);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space();
            if (loadedSave.inventoryData != null)
            {
                EditorGUILayout.LabelField("Inventory Slots: " + loadedSave.inventoryData.slots.Count);
                // Extend this section to edit individual slots if needed.
            }
        }
        else
        {
            EditorGUILayout.LabelField("Select a character save from the left to edit.");
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Save Changes"))
            SaveCurrentCharacterSave();
        EditorGUILayout.EndVertical();
    }

    /// <summary>Draws the new character creation section beneath the left and right panes.</summary>
    private void DrawNewCharacterSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Create New Character Save", EditorStyles.boldLabel);
        newCharName = EditorGUILayout.TextField("Character Name:", newCharName);
        newCharId = EditorGUILayout.IntField("Character ID:", newCharId);
        if (GUILayout.Button("Create Character File"))
            CreateNewCharacterFile();
        EditorGUILayout.EndVertical();
    }

    /// <summary>Loads the selected CharacterSave from the chosen folder.</summary>
    private void LoadSelectedCharacterSave()
    {
        if (selectedIndex < 0 || selectedIndex >= characterSaveFolders.Count)
            return;
        string folder = characterSaveFolders[selectedIndex];
        string folderName = Path.GetFileName(folder);
        string filePath = Path.Combine(folder, folderName + ".charsave");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            loadedSave = JsonUtility.FromJson<CharacterSave>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
            loadedSave = null;
        }
    }

    /// <summary>Saves the currently loaded CharacterSave back to disk.</summary>
    private void SaveCurrentCharacterSave()
    {
        if (loadedSave == null || loadedSave.characterData == null)
            return;
        // Determine folder from loadedSave data.
        string folderName = loadedSave.characterData.name + "_" + loadedSave.characterData.hashID;
        string folderPath = Path.Combine(saveFolderBase, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, folderName + ".charsave");
        string jsonData = JsonUtility.ToJson(loadedSave, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved changes to: " + filePath);
        RefreshCharacterSaves();
    }

    /// <summary>Creates a new CharacterSave file with the specified name and ID.</summary>
    private void CreateNewCharacterFile()
    {
        CharacterSave newSave = new CharacterSave();
        newSave.characterData = new CharacterData
        {
            name = newCharName,
            id = newCharId,
            hashID = DateTime.Now.GetHashCode() // Generate a unique hash.
        };
        newSave.skills = new List<SerializableSkill>();
        newSave.inventoryData = new SerializableItemSlotContainer { maxSlots = 0, slots = new List<SerializableItemStack>() };

        string folderName = newSave.characterData.name + "_" + newSave.characterData.hashID;
        string folderPath = Path.Combine(saveFolderBase, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, folderName + ".charsave");
        string jsonData = JsonUtility.ToJson(newSave, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Created new character save at: " + filePath);

        // Refresh the list and load the new save.
        RefreshCharacterSaves();
        loadedSave = newSave;
    }
}
#endif
