using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemData itemData { get; private set; }
    public bool isPickedUp = false;
    public float interactionRange = 3f;  // Default range for pickups
    public float bobbingHeight = 0.14f;  // Height of bobbing
    public float bobbingSpeed = 2f;     // Speed of bobbing

    private Vector3 originalPosition;    // To store the original position of the object

    void Start()
    {
        originalPosition = transform.position;  // Save the original position of the object
    }
    public void SetItemData(ItemData data)
    {
        this.itemData = data;
        //Update Graphic of pickup here
    }



    void Update()
    {
        // Create vertical bobbing effect using sine wave
        float newY = originalPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);  // Update position with new Y value
    }

    public bool CanInteract(Vector3 interactorPosition)
    {
        return !isPickedUp && Vector3.Distance(interactorPosition, transform.position) <= interactionRange;  // Can interact if not picked up
    }

    public void Interact()
    {
        if (!isPickedUp)
        {
            isPickedUp = true;
            // Add item to inventory logic here
            Destroy(gameObject);  // Remove item from scene
        }
    }

    public float GetInteractionRange() => interactionRange;  // Return the interaction range for this item
}