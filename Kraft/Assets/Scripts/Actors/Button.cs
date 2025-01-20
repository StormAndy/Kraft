using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;  // Make sure to include this for UnityEvents

public class Button : MonoBehaviour, IInteractable
{
    public float interactionRange = 3f;  // Range at which the player can interact with the button
    public bool isActivated = false;     // Whether the button is activated or not
    public UnityEvent onActivate, onDeactivate;           // UnityEvent to trigger when the button is pressed

    //Graphics/Animation Objects
    [SerializeField] private bool useStaticGraphics = true; //Toggle static graphic states instead of playing an animation
    [SerializeField] private GameObject staticGraphicOn, staticGraphicOff;

    private Animator animator;

    /// <summary> Checks if the player is within range to interact with the button. </summary>
    public bool CanInteract(Vector3 interactorPosition)
    {
        return Vector3.Distance(interactorPosition, transform.position) <= interactionRange;
    }

    /// <summary> Handles state change, graphic or animations and triggered unity events for button </summary>
    public void Interact()
    {
        isActivated = !isActivated; // Toggle button activation state
        Debug.Log(isActivated ? "Button pressed, action triggered" : "Button reset");
        if (useStaticGraphics && staticGraphicOn && staticGraphicOff)
            if (isActivated)
            {
                staticGraphicOff.SetActive(false);
                staticGraphicOn.SetActive(true);

                onActivate.Invoke();
            }
            else
            {
                staticGraphicOff.SetActive(true);
                staticGraphicOn.SetActive(false);

                onDeactivate.Invoke(); 
            }
        else
        {
            if (animator == null)
                animator = this.GetComponent<Animator>();
            if (animator)
            {
                if (isActivated)
                {
                    animator.Play("TurnOn");
                    onActivate.Invoke();
                }
                else
                {
                    animator.Play("TurnOff");
                    onDeactivate.Invoke();
                }
            }

        }
    }


    /// <summary> Returns the interaction range for this button. </summary>
    public float GetInteractionRange() => interactionRange;
}
