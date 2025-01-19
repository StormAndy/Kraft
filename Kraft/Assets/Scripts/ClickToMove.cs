using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private Mover mover;
    [SerializeField] private LayerMask groundMask;

    
    
    void Start()
    {
        if(mover!= null)
            mover = GetComponent<Mover>();

        Cursor.lockState = CursorLockMode.None;
    }

    
    void Update()
    {     
        //If mouse over UI, ignore click
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        //Left Mouse -> to Move
        if(Input.GetMouseButtonDown(0))
        {                               
            Debug.Log("click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, groundMask))
            {
                if (Game.Instance.state == EGameState.FreeRoam)
                {
                    foreach (Character c in Game.Instance.charactersSelected)
                    {
                        c.MoveTo(hit.point);
                        Debug.Log("move to :" + hit.point);
                    }
                }
            }
        }
        

        
    }
}
