using UnityEngine;

[CreateAssetMenu(fileName = "NewTickLODStatus", menuName = "TickLODStatus", order = 1)]
public class TickLODStatus : ScriptableObject
{
    // <summary> Distance and tick intervals for LOD categories </summary>
    [Header("Tick Interval Settings")]
    public float nearDistance = 30f;          
    public float nearTickInterval = 0.1f;     

    public float mediumDistance = 100f;       
    public float mediumTickInterval = 0.2f;   

    public float farDistance = 200f;          
    public float farTickInterval = 0.5f;      

    public float veryFarDistance = 400f;      
    public float veryFarTickInterval = 1f;    

    // <summary> Returns the appropriate tick interval based on the distance from the player </summary>
    public float GetTickInterval(float distance)
    {
        // Check distance ranges and return corresponding tick interval
        if (distance <= nearDistance)
        {
            return nearTickInterval;
        }
        else if (distance <= mediumDistance)
        {
            return mediumTickInterval;
        }
        else if (distance <= farDistance)
        {
            return farTickInterval;
        }
        else if (distance <= veryFarDistance)
        {
            return veryFarTickInterval;
        }
        else
        {
            // Default to the longest tick interval if the entity is too far
            return veryFarTickInterval;
        }
    }
}