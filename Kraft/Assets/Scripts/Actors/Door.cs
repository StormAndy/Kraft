using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    public float interactionRange = 3f;  // Default range for door interaction

    public float GetInteractionRange() => interactionRange;  // Return the interaction range for the door

    //Scene Transition?
    [SerializeField] private bool useAreaTransition = false;
    [SerializeField] private string sceneName = "ChangeMe";

    //Graphics/Animation Objects
    [SerializeField] private bool useStaticGraphics = true; //Toggle static graphic states instead of playing an animation
    [SerializeField] private GameObject staticGraphicClosed, staticGraphicOpen;

    private Animator animator;

    public bool CanInteract(Vector3 interactorPosition)
    {
        return Vector3.Distance(interactorPosition, transform.position) <= interactionRange;  // Can interact if within range
    }

    private void Start()
    {
        if (animator == null)
            animator = this.GetComponent<Animator>();

        //Warning logs for missing asset references
        if (useStaticGraphics && (!staticGraphicClosed || !staticGraphicOpen))
            Debug.LogWarning($"Static graphics not assigned for {name}");
        if (!useStaticGraphics && !animator)
            Debug.LogWarning($"Animator component missing on {name}");

    }

    public void Interact()
    {
        if (useAreaTransition)
            Debug.Log("Load Scene: " + sceneName);
        else
            ToggleState();

    }


    private void ToggleState()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Door opened" : "Door closed");  // Toggle door state
        if (useStaticGraphics && staticGraphicClosed && staticGraphicOpen)
            if(isOpen)
            {
                staticGraphicClosed.SetActive(false);
                staticGraphicOpen.SetActive(true);
            }
            else
            {
                staticGraphicClosed.SetActive(true);
                staticGraphicOpen.SetActive(false);
            }
        else
        {
            if (animator == null)
                animator = this.GetComponent<Animator>();
            if (animator)
            {
                if (isOpen)
                    animator.Play("Open");
                else
                    animator.Play("Close");
            }
           
        }
    }

   
}