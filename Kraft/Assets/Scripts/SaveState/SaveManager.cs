using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// SaveManager is responsible for converting game state into serializable data,
/// such as creating CharacterSave objects. In the future, it will also handle full GameSave data.
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary> Creates a CharacterSave from the given Character instance. </summary>     <param name="character">The Character to save.</param>
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


    /// <summary> Saves the given character’s data to a file in Assets/Saves/Characters. </summary>     <param name="character">The Character to save.</param>
    /// <param name="fileName">The file name to save as (e.g., "Player1.json").</param>
    public void SaveCharacter(Character character, string fileName)
    {
        CharacterSave save = CreateCharacterSave(character);
        string jsonData = JsonUtility.ToJson(save, true);
        SaveCharacterToAssetsFolder(fileName, jsonData);
    }

    /// <param name="fileName">The file name to load (e.g., "Player1.json").</param>
    public void LoadCharacterSave(string fileName)
    {
        string jsonData = LoadCharacterFromAssetsFolder(fileName);
        if (!string.IsNullOrEmpty(jsonData))
        {
            CharacterSave save = JsonUtility.FromJson<CharacterSave>(jsonData);

            InstantiateCharacterSave(save);
        }
    }

    /// <summary> Instantiate a character using a CharacterSave. </summary>  <param name="save">The CharacterSave containing saved data.</param>
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

    /// <summary> Saves the provided JSON data to a file in Assets/Saves/Characters. </summary>      <param name="fileName">The file name to save as.</param>
    /// <param name="jsonData">The JSON data to write.</param>
    private void SaveCharacterToAssetsFolder(string fileName, string jsonData)
    {
        string folderPath = Path.Combine(Application.dataPath, "Saves", "Characters");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Saved data to: " + filePath);
    }

    /// <summary> Loads JSON data from a file in Assets/Saves/Characters. </summary>    <param name="fileName">The file name to load.</param>
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


}