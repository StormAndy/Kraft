using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public bool allowMovement = false;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.state == EGameState.FreeRoam)
        {
            if (allowMovement)
            {
                if (moveTarget != null)
                    navMeshAgent.SetDestination(moveTarget.position);
                navMeshAgent.isStopped = false;
            }
            else
                navMeshAgent.isStopped = true;
        }
    }

    public void MoveTo(Vector3 pos)
    {
        moveTarget.position = pos;
    }

}
