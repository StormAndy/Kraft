using System.Collections.Generic;
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

    /// <summary> Instantiate a character using a CharacterSave. </summary>  <param name="save">The CharacterSave containing saved data.</param>
    public void LoadCharacterSave(CharacterSave save)
    {
        // TODO: Implement loading logic to update/create a Character from the save.
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
}