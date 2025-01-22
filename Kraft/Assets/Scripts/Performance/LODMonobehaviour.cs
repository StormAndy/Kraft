using Unity.VisualScripting;
using UnityEngine;

public class LODMonobehaviour : MonoBehaviour
{
    private float lastTickTime;

    [SerializeField] private TickLODStatus tickLODStatus;// <summary> Reference to the TickLODStatus class for managing LOD distances and tick intervals </summary>

    [SerializeField] private float tickIntervalModifier = 1.0f; //<summary> A modifier to further adjust the tick interval</summary>
    private float tickInterval;/// <summary> The current tick interval </summary>
    [SerializeField] private float tickOffsetRange = 0.02f; // <summary> The range of random offset for the initial tick (default 0.02f) </summary>

    /// <summary> Use instead of deltaTime for custom tick </summary>
    private float GetTimeSinceLastTick() {  return Time.time - lastTickTime;  }

    void OnEnable()
    {
        // Initialize the tick interval to the nearest distance category (default)
        tickInterval = tickLODStatus.nearTickInterval;

        // Apply the tickIntervalModifier to adjust the tick interval
        tickInterval *= tickIntervalModifier;

        // Randomize the initial offset within the range of tickOffsetRange
        float randomOffset = Random.Range(0f, tickOffsetRange);

        // Start invoking the Tick method repeatedly at the initial interval with the random offset
        InvokeRepeating("Tick", randomOffset, tickInterval); // Randomized start time, then repeat at tickInterval
    }

    /// <summary> Less frequent alternative to Update Invoked on a LOD based tickInterval, Use GetTimeSinceLastTick() instead of deltaTime</summary>
    protected virtual void Tick()
    {

        // Calculate the distance to the main camera
        float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);

        // Get the appropriate tick interval based on the distance
        float newTickInterval = tickLODStatus.GetTickInterval(distanceToCamera);

        // If the tick interval has changed, cancel and restart InvokeRepeating
        if (!Mathf.Approximately(tickInterval, newTickInterval))
        {     
            CancelInvoke("Tick"); // Stop the current InvokeRepeating
            tickInterval = newTickInterval; // Update the tick interval and restart InvokeRepeating with the new interval

            // Apply the modifier again
            tickInterval *= tickIntervalModifier;

            // Randomize the new start offset again within the range
            float randomOffset = Random.Range(0f, tickOffsetRange);
            InvokeRepeating("Tick", randomOffset, tickInterval);
        }

        // Perform the update (e.g., movement, behavior, etc.)
        //Debug.Log("Ticking at interval: " + tickInterval);

        lastTickTime = Time.time;
        Debug.Log("Test LOD log, time since last tick is " + lastTickTime);
    }

    // Stop the repeating invocation when the object is disabled
    void OnDisable()
    {
        CancelInvoke("Tick");
    }
}
