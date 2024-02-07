using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    void Update()
    {
        // Convert the mouse position into world coordinates.
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.nearClipPlane;  // Set this to the distance of the player from the camera if it's not 0
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // Get the direction from the player to the mouse
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;

        // Calculate the angle to rotate towards.
        // Mathf.Atan2 gives the angle in radians, so we convert it to degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the player's rotation to this angle, adjusting for the sprite's orientation if necessary
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90)); // The "- 90" assumes the player sprite is facing up
    }
}