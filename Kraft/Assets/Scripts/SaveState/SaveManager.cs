using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary> SaveManager is responsible for converting game state into serializable data, 
/// such as creating CharacterSave objects. In the future, it will also handle full GameSave data.</summary>
public class SaveManager : MonoBehaviour
{
    ///<summary> Number of backup files to keep</summary>
    [SerializeField] private int backupCount = 10; 


    /// <summary> Creates a CharacterSave from the given Character instance. </summary>     
    /// <returns>A CharacterSave containing character data, inventory, and skills.</returns>
    public CharacterSave CreateCharacterSave(Character character)
    {
        CharacterSave save = new CharacterSave();
        save.characterData = character.Data; // Reference to CharacterData

        // Convert the character's inventory into a serializable container.
        Inventory inv = character.GetComponent<Inventory>();
        if (inv != null)
        {
            ItemSlotContainer container = inv.ConvertToItemSlotContainer();
            save.inventoryData = container.ConvertToSerializable();
        }

        // Convert skills from the SkillManager into serializable data.
        // Assume SkillManager has a method to retrieve skills for a character by its ID.
        List<SerializableSkill> skillList = new List<SerializableSkill>();
        Dictionary<string, Skill> charSkills = Game.Instance.skillManager.GetSkillsForCharacter(character.Data.id);
        if (charSkills != null)
        {
            foreach (var kvp in charSkills)
            {
                SerializableSkill sSkill = new SerializableSkill
                {
                    skillId = kvp.Key,
                    totalExperience = kvp.Value.TotalExperience
                };
                skillList.Add(sSkill);
            }
        }
        save.skills = skillList;

        return save;
    }


    /// <summary> Saves character’s data to a file in a unique subdirectory under Assets/Saves/Characters. 
    /// Implements a backup system that shifts existing backups. </summary>
    public void SaveCharacter(Character character)
    {
        // Determine the unique folder for this character save using its name and hashID.
        string saveFolder = GetSaveFolder(character);
        // The uniqueID for naming files is the folder name (e.g., "ExampleName_123456789").
        string uniqueID = Path.GetFileName(saveFolder);
        string mainSaveFile = Path.Combine(saveFolder, uniqueID + ".charsave");

        // Convert the character to save data.
        CharacterSave save = CreateCharacterSave(character);
        string jsonData = JsonUtility.ToJson(save, true);

        // If a main save already exists, create backups by shifting the existing ones.
        if (File.Exists(mainSaveFile))
        {
            ShiftBackups(saveFolder, uniqueID);
            // Rename the current main save to backup _1.
            string backupFile = Path.Combine(saveFolder, uniqueID + "_1.charbackup");
            File.Move(mainSaveFile, backupFile);
        }

        // Write the new main save file.
        File.WriteAllText(mainSaveFile, jsonData);
        Debug.Log("Saved data to: " + mainSaveFile);
    }

    /// <summary> Loads a character’s data from a file in the character's unique save folder. </summary>
    public void LoadCharacterSave(Character character, string fileName)
    {
        string saveFolder = GetSaveFolder(character);
        string filePath = Path.Combine(saveFolder, fileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Debug.Log("Loaded data from: " + filePath);
            CharacterSave save = JsonUtility.FromJson<CharacterSave>(jsonData);
            InstantiateCharacterSave(save);
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }
    }

    /// <summary> Instantiate a character using a CharacterSave. </summary>  
    /// <param name="save">The CharacterSave containing saved data.</param>
    public void InstantiateCharacterSave(CharacterSave save)
    {
        // TODO: Implement loading logic to update/create a Character from the save.
        Debug.Log("Character save loaded");
        Debug.Log("TODO: Implement loading logic on InstantiateCharacterSave() ");
    }

    /// <summary> Saving the game state, including world progress. </summary>
    public void SaveGame()
    {

        // TODO: Create and persist a GameSave that includes CharacterSaves and world state.
    }

    /// <summary> Loading the game state, including world progress.</summary>
    public void LoadGame()
    {
        // TODO: Retrieve and apply GameSave data to restore game progress.
    }

    /// <summary> Saves the provided JSON data to a file in Assets/Saves/Characters. </summary>      
    /// <param name="fileName">The file name to save as.</param>    <param name="jsonData">The JSON data to write.</param>
    private void SaveCharacterToAssetsFolder(string fileName, string jsonData)
    {
        string folderPath = Path.Combine(Application.dataPath, "Saves", "Characters");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved data to: " + filePath);
    }

    /// <summary> Loads JSON data from a file in Assets/Saves/Characters. </summary>
    /// <returns>The JSON string if found; otherwise, null.</returns>
    private string LoadCharacterFromAssetsFolder(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "Saves", "Characters", fileName);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Debug.Log("Loaded data from: " + filePath);
            return jsonData;
        }
        Debug.LogWarning("File not found: " + filePath);
        return null;
    }


    #region File Helper Methods

    /// <summary> Determines and returns the save folder for a given character. The folder is based on the character's name and a hash 
    /// from the time the folder was created. If a folder already exists for the character, it is reused. </summary>
    /// <returns>The full path to the character's save folder.</returns>
    private string GetSaveFolder(Character character)
    {
        string baseFolder = Path.Combine(Application.dataPath, "Saves", "Characters");
        if (!Directory.Exists(baseFolder))
            Directory.CreateDirectory(baseFolder);

        // Use the character's name and hashID to create a unique folder name.
        string folderName = character.Data.name + "_" + character.Data.hashID;
        string folderPath = Path.Combine(baseFolder, folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    /// <summary> Shifts existing backup files in the specified folder. The highest-numbered backup is deleted then all backups are renamed </summary>
    /// <param name="folder">The folder containing the save and backup files.</param>   <param name="uniqueID">The base unique identifier (folder name) for the save files.</param>
    private void ShiftBackups(string folder, string uniqueID)
    {
        // Delete the highest-numbered backup if it exists.
        string highestBackup = Path.Combine(folder, uniqueID + "_" + backupCount + ".charbackup");
        if (File.Exists(highestBackup))
            File.Delete(highestBackup);

        // Shift backups from backupCount-1 down to 1.
        for (int i = backupCount - 1; i >= 1; i--)
        {
            string currentBackup = Path.Combine(folder, uniqueID + "_" + i + ".charbackup");
            string newBackup = Path.Combine(folder, uniqueID + "_" + (i + 1) + ".charbackup");
            if (File.Exists(currentBackup))
                File.Move(currentBackup, newBackup);
        }
    }

    #endregion

}