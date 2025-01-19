using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public string name;
}

public class Character : MonoBehaviour
{
    //Local Stats for this instance of Character

    public Mover mover;

    public int inititive;
    [SerializeField] private Transform moveTargetGraphic;

    public bool isPlayerControlled;
    

    //Character Data
    private CharacterData data = new CharacterData();



   
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

  
    private void Start()
    {
        if (mover == null)
            mover = GetComponent<Mover>();
        //Game.Instance.AddCharacter(this);
    }

}
