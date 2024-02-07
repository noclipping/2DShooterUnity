using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f; // Time in seconds before the bullet gets destroyed
    public float knockbackForce = 500f; // Adjust the force as needed

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Optional: Check for a specific tag if needed
        // if (!other.CompareTag("Enemy")) return;

        Rigidbody2D rb = other.attachedRigidbody; // Get the Rigidbody2D of the object we collided with
        if (rb != null)
        {
            // Calculate the direction of the force
            Vector2 forceDirection = other.transform.position - transform.position;
            forceDirection.Normalize();

            // Apply the force
            rb.AddForce(forceDirection * knockbackForce);
        }

        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}
