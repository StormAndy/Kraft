using UnityEngine;

public class FollowActivePlayer : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, -10); // Default offset
    private Transform playerTransform;

    private void Start()
    {
        // Get the transform of the active player
        if (Game.Instance != null && Game.Instance.activeCharacter != null)
        {
            playerTransform = Game.Instance.activeCharacter.transform;
        }
        else
        {
            Debug.LogError("Active Player or game.Instance not found!");
        }
    }

    private void Update()
    {
        // Ensure the player is still assigned
        if (playerTransform != null)
        {
            // Update the camera position to follow the player with the specified offset
            transform.position = playerTransform.position + offset;
        }
    }
}