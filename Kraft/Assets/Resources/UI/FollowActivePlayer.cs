using UnityEngine;

public class FollowActivePlayer : MonoBehaviour
{
    public bool useRelativePos;

    public Vector3 offset = new Vector3(0, 10, -10); // Default position offset
    public Vector3 rotationOffset = new Vector3(0, 0, 0); // Default rotation offset
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
            if (useRelativePos)
            {
                // Calculate the offset position using the player's rotation
                Vector3 rotatedOffset = playerTransform.rotation * offset;

                // Apply the rotation offset to the camera or object's position
                transform.position = playerTransform.position + rotatedOffset;

                // Adjust the rotation relative to the player's rotation, considering the rotation offset
                Quaternion targetRotation = playerTransform.rotation * Quaternion.Euler(rotationOffset);
                transform.rotation = targetRotation;
            }
            else
            {
                // Update the camera position to follow the player with the specified offset
                transform.position = playerTransform.position + offset;
            }
        }
    }
}