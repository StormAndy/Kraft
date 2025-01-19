using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour
{

    public bool isSingleClick = true;   // Flag to control single or separate click modes
    public float defaultInteractionRange = 3f;  // Default interaction range for generic objects
    private IInteractable currentInteractable;
    public GameObject interactionUI;

    private Vector3 interactorPosition;  // Store the position here to avoid allocation
    [SerializeField] private LayerMask groundMask;  // Mask for the ground layer (movement)
    [SerializeField] private LayerMask interactableMask;  // Mask for the interactable layer (actor)

    /// <summary> Handles user input for interactions and movement based on left and right clicks. </summary>
    void Update()
    {
        // If mouse is over UI, ignore click
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        interactorPosition = Game.Instance.activeCharacter.transform.position;  // Update interactor position

        if (isSingleClick)  // Handle single-click mode (both interact and move on left click)
        {
            if (Input.GetMouseButtonDown(0))  // Left Click to interact first, then move if no interaction
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                bool interacted = false;  // Track if interaction occurred

                // First, check for interactables
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableMask))
                {
                    currentInteractable = hit.collider.GetComponent<IInteractable>();
                    if (currentInteractable != null && Vector3.Distance(interactorPosition, hit.point) <= currentInteractable.GetInteractionRange() && currentInteractable.CanInteract(interactorPosition))
                    {
                        currentInteractable.Interact();  // Interact with object if within range
                        interacted = true;  // Mark interaction as successful
                    }
                }

                // Only move if no interaction was triggered
                if (!interacted && Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))  // Movement should check the ground layer
                {
                    if (Game.Instance.state == EGameState.FreeRoam)
                    {
                        Game.Instance.activeCharacter.MoveTo(hit.point);  // Move character to clicked point
                        Debug.Log("Move to: " + hit.point + "with character: " + Game.Instance.activeCharacter.name + " using Mover: " + Game.Instance.activeCharacter.mover.name);
                    }
                }
            }
        }
        else  // Handle separate actions (Left Click = Move, Right Click = Interact)
        {
            if (Input.GetMouseButtonDown(0))  // Left Click to move
            {
                Debug.Log("click");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Perform raycast to detect ground layer for movement
                //(Physics.Raycast(ray, out hit, 100, groundMask))
                if (Physics.Raycast(ray, out hit, 100, groundMask))
                {
                    if (Game.Instance.state == EGameState.FreeRoam)
                    {
                        Game.Instance.activeCharacter.MoveTo(hit.point);
                        Debug.Log("Move to: " + hit.point);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))  // Right Click to interact
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Perform raycast to detect interactables on the "Actor" layer
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableMask))
                {
                    currentInteractable = hit.collider.GetComponent<IInteractable>();
                    if (currentInteractable != null && Vector3.Distance(interactorPosition, hit.point) <= currentInteractable.GetInteractionRange() && currentInteractable.CanInteract(interactorPosition))
                        currentInteractable.Interact();  // Interact with object if within range
                }
            }
        }

        ShowInteractionPrompt();  // Show or hide the interaction prompt based on the cursor position
    }

    /// <summary> Displays the interaction prompt when the player hovers over an interactable object. </summary>
    private void ShowInteractionPrompt()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform raycast for interactables
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableMask))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
            if (currentInteractable != null && Vector3.Distance(interactorPosition, hit.point) <= currentInteractable.GetInteractionRange())
                interactionUI.SetActive(true);  // Show prompt if object is interactable
            else
                interactionUI.SetActive(false);  // Hide prompt if object is out of range
        }
        else
            interactionUI.SetActive(false);  // Hide prompt when no interactable object
    }
}
