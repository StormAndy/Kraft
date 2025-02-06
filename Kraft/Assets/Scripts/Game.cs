using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState
{
    FreeRoam,
    Crafting,
    Dialogue
}

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public CinemachineVirtualCamera mainCamera;

    public Character activeCharacter;

    //Singletons
    public DatabaseManager databaseManager;
    public SkillManager skillManager;
    public OfflineProgressionSystem offlineProgressionSystem;

    //Gameplay prefabs to instiate on the fly
    public GameObject prefabPickup;

    //global states
    public EGameState state;

    /// <summary> Dictionary lookup for characters by their ID.</summary>
    private Dictionary<int, Character> characters = new(); 


    private void Awake()
    {
        #region Singleton Pattern
        if (Game.Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this.gameObject);
        #endregion
    }


    void Start()
    {
        //Database setup
        if(databaseManager == null)
            databaseManager = GetComponent<DatabaseManager>();

        databaseManager.LoadData();
        databaseManager.LogData();
        //databaseManager.SaveTestData();

        //Skills
        if (skillManager == null)
            skillManager = GetComponent<SkillManager>();

        if (databaseManager == null)
            offlineProgressionSystem = GetComponent<OfflineProgressionSystem>();


        //Create new ones if missing here?
    }

    
    void Update()
    {

    }


    #region Character Management

    /// <summary>Adds a character to the game's lookup table.</summary> <param name="character">The Character instance to add.</param>
    public void AddCharacter(Character character)
    {
        int id = character.Data.id;
        if (!characters.ContainsKey(id))
            characters.Add(id, character);
    }

    /// <summary> Removes a character from the game's lookup table and cleans up its associated skills. </summary> <param name="character">The Character instance to remove.</param>
    public void RemoveCharacter(Character character)
    {
        int id = character.Data.id;
        if (characters.ContainsKey(id))
        {
            characters.Remove(id);
            // Automatically remove associated skills.
            if (skillManager != null)
                skillManager.RemoveCharacterSkills(id);
        }
    }

    /// <summary>Gets a character by its ID.</summary> <param name="characterId">The ID of the character.</param>
    /// <returns>The Character instance, or null if not found.</returns>
    public Character GetCharacter(int characterId) =>
        characters.TryGetValue(characterId, out Character character) ? character : null;

    #endregion
}
