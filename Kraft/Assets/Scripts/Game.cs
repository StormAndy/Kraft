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

    //global states
    public EGameState state;


    private void Awake()
    {
        #region Singleton Pattern
        if (Game.Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }


    void Start()
    {
        
    }

    
    void Update()
    {

    }
}
