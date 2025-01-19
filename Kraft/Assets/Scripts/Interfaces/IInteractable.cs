using UnityEngine;

public interface IInteractable
{
    bool CanInteract(Vector3 interactorPosition);  // Determines if interaction is possible based on the interactor's position
    void Interact();                               // Executes the interaction (e.g. pick up, open, talk)
    float GetInteractionRange();                   // Returns the interaction range of the object
}

