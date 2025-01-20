using UnityEditor.Overlays;
using UnityEngine;

/// <summary> Data for each harvesting increment on a Harvestable Actor </summary>
[System.Serializable]
public struct HarvestStateData
{
    public GameObject stateGraphic;  // Graphic associated with this state
    public int itemsToHarvest;       // Number of items to harvest from this state increment
}

public class Harvestable : MonoBehaviour, IInteractable
{
    public string itemName;  // The item name that will be given to the player
    public float interactionRange = 3f;  // Interaction range for harvesting

    //Growing and harvet states
    public int startingState = 0;
    public HarvestStateData[] states; //First state (State 0) is not harvestable
    [SerializeField]private int currentStateIndex = 0; 

    //Regen
    [SerializeField] private bool isRegenerating = true; 
    [SerializeField] private float regenerationTimer = 0f; 
    public float regenerationTime = 10f; 


    void Start()
    {
        if (startingState < 0 || startingState > states.Length)
            startingState = 0;

        currentStateIndex = startingState;  // Start at the first state (Depleted, so 0)
        UpdateGraphics();  // Set initial graphics based on the current state
    }

    void Update()
    {
        if (isRegenerating && currentStateIndex < states.Length - 1)
        {
            regenerationTimer += Time.deltaTime;
            if (regenerationTimer >= regenerationTime) RegenerateState();
        }
    }

    /// <summary> Checks if the player is within range to interact with the harvestable object. </summary>
    public bool CanInteract(Vector3 interactorPosition)
    {
        // Don't allow interaction if the state 0 (Empty)
        return currentStateIndex > 0 && Vector3.Distance(interactorPosition, transform.position) <= interactionRange;
    }

    /// <summary> Harvests the item and triggers state changes. </summary>
    public void Interact()
    {
        if (currentStateIndex > 0)  // Don't allow harvesting in the Depleted state (state 0)
        {
            // Give the player the item(s) based on the current state
            Debug.Log($"Harvested {itemName} - {states[currentStateIndex].itemsToHarvest} items!");

            // Decrement state only if it's not at the "Depleted" state (state 0)
            if (currentStateIndex > 0)
            {
                currentStateIndex--;  // Move to previous state (partially harvested -> unharvested)
                isRegenerating = true;  // Start regeneration process
                regenerationTimer = 0f;
            }

            UpdateGraphics();  // Update the state graphic
        }
        else
        {
            Debug.Log("This item cannot be harvested yet, it needs to regenerate.");
        }
    }

    /// <summary> Regenerates the harvestable state if possible. </summary>
    private void RegenerateState()
    {
        // If we haven't reached the first state (Unharvested), regenerate to the next state
        if (currentStateIndex < states.Length - 1)
        {
            currentStateIndex++;
            Debug.Log($"Harvestable item regenerated to state: {currentStateIndex}");
            regenerationTimer = 0f;
            UpdateGraphics();  // Update graphics after state change
        }
        else
        {
            isRegenerating = false;  // Stop regeneration when it's fully regenerated (Unharvested state)
            Debug.Log("Harvestable item is fully regenerated.");
        }
    }

    /// <summary> Updates the graphic based on the current state. </summary>
    private void UpdateGraphics()
    {
        // Disable all graphics first
        for (int i = 0; i < states.Length; i++)
            states[i].stateGraphic.SetActive(false);

        // Enable the graphic for the current state
        if (currentStateIndex >= 0 && currentStateIndex < states.Length)
            states[currentStateIndex].stateGraphic.SetActive(true);
    }

    /// <summary> Returns the interaction range for this object. </summary>
    public float GetInteractionRange() => interactionRange;
}