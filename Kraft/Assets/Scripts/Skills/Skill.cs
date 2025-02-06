using System;
using UnityEngine;

/// <summary>Represents a skill with experience and computed level and experience to next level.</summary>
[System.Serializable]
public class Skill
{   
    public string Name { get; private set; } /// <summary>The name of the skill.</summary>  
    public float TotalExperience { get; private set; } /// <summary>Total accumulated experience.</summary>

    private readonly float baseExperience = 100f;
    private readonly float multiplier = 1.1f;

    /// <summary>Initializes a new instance of the <see cref="Skill"/> class with the specified name.</summary> 
    public Skill(string name) => Name = name;

    /// <summary>Gets the current level, computed from the total experience.</summary>
    public int Level
    {
        get
        {
            double n = Math.Log((TotalExperience * (multiplier - 1) / baseExperience) + 1, multiplier);
            return (int)Math.Floor(n) + 1;
        }
    }

    /// <summary>Gets the experience required to reach the next level.</summary>
    public float ExperienceToNextLevel
    {
        get
        {
            float requiredForNext = baseExperience * ((float)Math.Pow(multiplier, Level) - 1) / (multiplier - 1);
            return requiredForNext - TotalExperience;
        }
    }

    /// <summary>Increases total experience.</summary>  
    public void GainExperience(float amount) => TotalExperience += amount;
}