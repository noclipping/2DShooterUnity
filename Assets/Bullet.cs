using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;        // Time in seconds before the bullet gets destroyed
    public float knockbackForce = 500f; // Knockback force applied on hit
    public int damage = 10;             // Amount of damage the bullet does

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the bullet after its lifetime expires
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object hit is on the "Floor" layer (assuming layer index 8 for "Floor")
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor")) return;

        // Apply knockback if the object has a Rigidbody2D
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector2 forceDirection = other.transform.position - transform.position;
            forceDirection.Normalize();
            rb.AddForce(forceDirection * knockbackForce);
        }

        // Apply damage if the object has a ZombieHealth component
        ZombieHealth zombieHealth = other.GetComponent<ZombieHealth>();
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(damage); // Apply the bullet's damage to the zombie
        }

        // Destroy the bullet on collision with other objects
        Destroy(gameObject);
    }
}
