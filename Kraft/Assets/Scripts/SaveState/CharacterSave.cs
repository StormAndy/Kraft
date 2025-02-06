using System;
using System.Collections.Generic;

/// <summary> Serializable class that stores character save data including character data, inventory, and skills. </summary>
[Serializable]
public class CharacterSave
{
    public CharacterData characterData;                     // Character's base data.
    public SerializableItemSlotContainer inventoryData;     // Converted inventory data.
    public List<SerializableSkill> skills;                  // List of character skills.
}

/// <summary> Serializable representation of a skill. </summary>
[Serializable]
public class SerializableSkill
{
    public string skillId;         // Identifier for the skill.
    public float totalExperience;  // Total accumulated experience for the skill.
}