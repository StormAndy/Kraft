using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string name;
    public int id;
    public int hashID;
}

public class Character : MonoBehaviour
{
    //Local Stats for this instance of Character

    public Mover mover;

    public int inititive;
    [SerializeField] private Transform moveTargetGraphic;

    public bool isPlayerControlled;


    // Private character data; exposed via a public getter.
    [SerializeField] private CharacterData data = new CharacterData();
    public CharacterData Data => data; // Public accessor for CharacterData.




    private void Update()
    {
        //Update move graphic Visibility
        if (Vector3.Distance(this.transform.position, moveTargetGraphic.position) <= 0.25)
            moveTargetGraphic.gameObject.SetActive(false);
        else
            moveTargetGraphic.gameObject.SetActive(true);
    
    }

    public void MoveTo(Vector3 pos)
    {
        if (moveTargetGraphic != null)
            moveTargetGraphic.position = pos;
        mover.MoveTo(pos);
    }

    private void Awake()
    {

    }

    private void Start()
    {
        if (mover == null)
            mover = GetComponent<Mover>();

        // Set a unique hashID based on the current date/time if it hasn’t been set yet, used for Save/Load
        if (data.hashID == 0)
            data.hashID = DateTime.Now.GetHashCode();

        // Set the unique ID using the prefab instance ID, used in runetime lookup queries
        data.id = gameObject.GetInstanceID();

        // Automatically add this character to the Game's lookup table.
        if (Game.Instance != null)
            Game.Instance.AddCharacter(this);
    }

    private void OnDestroy()
    {
        // Automatically remove this character when destroyed.
        if (Game.Instance != null)
            Game.Instance.RemoveCharacter(this);
    }

}
