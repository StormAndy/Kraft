using UnityEngine;
using System.Collections.Generic;

/// <summary>Manages skills for multiple characters.</summary>
public class SkillManager : MonoBehaviour
{
    // Dictionary mapping a character ID to their dictionary of skills.
    private Dictionary<int, Dictionary<string, Skill>> characterSkills = new Dictionary<int, Dictionary<string, Skill>>();

    ///<summary>Adds a skill for the specified character.</summary> <param name="characterId">The ID of the character.</param>  <param name="skillId">The ID of the skill.</param>  <param name="skill">The Skill instance.</param>
    public void AddSkill(int characterId, string skillId, Skill skill)
    {
        if (!characterSkills.ContainsKey(characterId))
            characterSkills[characterId] = new Dictionary<string, Skill>();
        characterSkills[characterId][skillId] = skill;
    }

    ///<summary>Increases experience for a specific skill for the specified character.</summary>    <param name="characterId">The ID of the character.</param>  <param name="skillId">The ID of the skill.</param>  <param name="amount">The experience amount to gain.</param>
    public void GainExperience(int characterId, string skillId, float amount)
    {
        if (characterSkills.TryGetValue(characterId, out var skills) && skills.TryGetValue(skillId, out var skill))
            skill.GainExperience(amount);
    }

    ///<summary>Retrieves a skill for the specified character.</summary>    <param name="characterId">The ID of the character.</param>  <param name="skillId">The ID of the skill.</param>
    ///<returns>The Skill instance, or null if not found.</returns>
    public Skill GetSkill(int characterId, string skillId) =>
        characterSkills.TryGetValue(characterId, out var skills)
            ? (skills.TryGetValue(skillId, out var skill) ? skill : null)
            : null;

    /// <summary>Retrieves all skills associated with the specified character.</summary>    <param name="characterId">The unique ID of the character.</param>
    /// <returns>A dictionary of skill IDs and Skill instances, or null if none exist.</returns>
    public Dictionary<string, Skill> GetSkillsForCharacter(int characterId) =>
        characterSkills.TryGetValue(characterId, out var skills) ? skills : null;

    ///<summary>Removes all skills associated with the specified character.</summary>   <param name="characterId">The ID of the character.</param>
    public void RemoveCharacterSkills(int characterId) => characterSkills.Remove(characterId);
}